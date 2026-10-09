using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 玩家血量 UI：实时更新血条滑块和文字，玩家死亡时隐藏
/// </summary>
public class PlayerHPUI : MonoBehaviour
{
    [Header("UI")]
    public Slider slider;
    public TMP_Text hpText;

    [Header("Player")]
    public PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth.OnPlayerDead += Hide;
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        float current = playerHealth.CurrentHP;
        float max = playerHealth.maxHP;
        slider.value = current / max;
        hpText.text = $"{current}/{max}";
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // 取消订阅避免空引用
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDead -= Hide;
        }
    }

}