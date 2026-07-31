using UnityEngine;
using UnityEngine.Playables;

public class DefenseSkill : SkillBase
{
    [SerializeField]
    private SkillData data;

    private PlayerHealth playerHealth;
    private PlayerAnimation playerAnimation;
    private PlayerState playerState;
    public float defendTime = 1f;

    protected override SkillData Data
    {
        get { return data; }
    }

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerState = GetComponent<PlayerState>();
    }

    public override void Use()
    {
        if (!CanUse())
        {
            Debug.Log("防御冷却中");
            return;
        }
        StartDefend();
        StartCooldown();
    }

    private void StartDefend()
    {
        playerHealth.StartDefend();
        playerAnimation.PlayDefend();
        playerState.ChangeState(PlayerState.State.Defend);
        Invoke(nameof(EndDefend), defendTime);
    }

    private void EndDefend()
    {
        playerHealth.EndDefend();
        playerAnimation.StopDefend();
        if (playerState.IsState(PlayerState.State.Defend))
        {
            playerState.ChangeState(PlayerState.State.Idle);
        }
    }

}