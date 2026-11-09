using UnityEngine;
using System;
using System.Collections;

namespace MiniRPG
{
    /// <summary>
    /// 玩家生命系统：管理血量、无敌帧、击退、防御、死亡。
    /// 实现 IDamageable 接口，被攻击时由攻击方调用 TakeDamage。
    /// </summary>
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        #region ===== Inspector =====
        [Header("生命设置")]
        public float maxHP = 100;
        [Header("击退设置")]
        public float knockBackForce = 1f;

        #endregion

        #region ===== Runtime =====
        private float currentHP;
        private bool isInvincible;                  // 是否处于无敌时间
        private float invincibleTimer;              // 无敌时间计时器
        public float invincibleDuration = 0.8f;     // 无敌持续时间
        public event Action OnPlayerDead;           // 玩家死亡事件，UI 等系统订阅
        /// <summary>血量变化事件（参数：当前血量、最大血量），HUD 订阅后刷新血条</summary>
        public event Action<float, float> OnHPChanged;
        private bool isDefending;                   // 防御中，免疫伤害

        #endregion

        #region ===== Component =====
        private Rigidbody2D rb;
        private PlayerState playerState;
        private PlayerAnimation playerAnimation;
        private PlayerAttack playerAttack;

        #endregion


        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerState = GetComponent<PlayerState>();
            playerAnimation = GetComponent<PlayerAnimation>();
            playerAttack = GetComponent<PlayerAttack>();

            // 血量在 Awake 就初始化，保证后续其他组件的 Start 中应用强化时数值已是满血
            currentHP = maxHP;
        }

        private void Start()
        {
            // 通知 HUD 首次显示
            OnHPChanged?.Invoke(currentHP, maxHP);
        }

        void Update()
        {
            UpdateInvincible();
        }

        /// <summary>
        /// IDamageable 实现：处理受伤、无敌帧、击退、死亡判定
        /// </summary>
        public void TakeDamage(float damage, Vector2 attackDirection)
        {
            // 死亡后不再受伤
            if (playerState.IsState(PlayerState.State.Dead))
            {
                return;
            }
            // 无敌帧内免疫伤害
            if (isInvincible)
            {
                Logger.Log("无敌状态");
                return;
            }
            // 防御中免疫伤害
            if (isDefending)
            {
                Logger.Log("防御成功，抵挡伤害");
                // 播放格挡成功音效
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayBlock();
                }
                return;
            }
            currentHP -= damage;

            OnHPChanged?.Invoke(currentHP, maxHP);

            StartInvincible();
            // 受伤时强制取消当前攻击
            playerAttack.CancelAttack();
            if (currentHP <= 0)
            {
                Dead();
            }
            else
            {
                Hurt();
                ApplyKnockBack(attackDirection);
            }
        }

        private void StartInvincible()
        {
            isInvincible = true;
            invincibleTimer = 0;
            Logger.Log("进入无敌状态");
        }

        private void UpdateInvincible()
        {
            if (!isInvincible)
            {
                return;
            }
            invincibleTimer += Time.deltaTime;
            if (invincibleTimer >= invincibleDuration)
            {
                isInvincible = false;
                invincibleTimer = 0;
                Logger.Log("无敌结束");
            }
        }

        private Coroutine hurtRecoverRoutine;

        private void Hurt()
        {
            playerState.ChangeState(PlayerState.State.Hurt);
            playerAnimation.PlayHurt();

            // 0.5秒后从受伤状态恢复，用协程代替基于字符串的 Invoke
            if (hurtRecoverRoutine != null)
            {
                StopCoroutine(hurtRecoverRoutine);
            }
            hurtRecoverRoutine = StartCoroutine(RecoverFromHurtRoutine());
        }

        private IEnumerator RecoverFromHurtRoutine()
        {
            yield return new WaitForSeconds(0.5f);
            hurtRecoverRoutine = null;

            if (playerState.IsState(PlayerState.State.Hurt))
            {
                playerState.ChangeState(PlayerState.State.Idle);
            }
        }

        private void ApplyKnockBack(Vector2 direction)
        {
            rb.AddForce(direction.normalized * knockBackForce, ForceMode2D.Impulse);
        }

        private void Dead()
        {
            playerState.ChangeState(PlayerState.State.Dead);
            playerAnimation.PlayDead();

            OnPlayerDead?.Invoke();
        }

        /// <summary>
        /// 开始防御，由 PlayerDefense 调用
        /// </summary>
        public void StartDefend()
        {
            isDefending = true;
            Logger.Log("进入防御状态");
        }

        /// <summary>
        /// 结束防御，由 PlayerDefense 调用
        /// </summary>
        public void EndDefend()
        {
            isDefending = false;
            Logger.Log("结束防御状态");
        }

        public float CurrentHP
        {
            get { return currentHP; }
        }

        /// <summary>
        /// 当前是否处于防御状态，供攻击方判断伤害是否被格挡
        /// </summary>
        public bool IsDefending => isDefending;

        /// <summary>
        /// 提升最大生命值并同步回血（强化商店生命强化调用）
        /// </summary>
        public void IncreaseMaxHP(float amount)
        {
            if (amount <= 0) return;
            maxHP += amount;
            currentHP += amount;
            OnHPChanged?.Invoke(currentHP, maxHP);
            Logger.Log("最大生命值提升 +" + amount + " 当前上限:" + maxHP);
        }
    }
}
