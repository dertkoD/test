using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private AgentRoot agentRoot;

    [Header("Stats")]
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int currentHp;
    [SerializeField] private string deathTrigger = "Death";
    [SerializeField] private string hasWeaponParam = "HasWeapon";

    [Header("Events In (UnityEvent Channel)")]
    [SerializeField] private DamageEventChannelSO damageEventChannel;

    [Header("Events Out (Action Channel)")]
    [SerializeField] private HealthChangedActionChannelSO healthChangedAction;

    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;
    public bool IsDead => currentHp <= 0;

    private bool deathTriggered;

    private void Awake()
    {
        if (currentHp <= 0)
            currentHp = maxHp;
    }

    private void OnEnable()
    {
        if (damageEventChannel) damageEventChannel.Register(OnDamageReceived);

        deathTriggered = false;

        if (agentRoot)
            healthChangedAction?.Raise(agentRoot.AgentId, currentHp, maxHp);
    }

    private void OnDisable()
    {
        if (damageEventChannel) damageEventChannel.Unregister(OnDamageReceived);
    }

    private void OnDamageReceived(DamageInfo info)
    {
        if (!agentRoot) return;
        if (IsDead) return;

        // agency check
        if (info.targetAgentId != agentRoot.AgentId) return;

        int before = currentHp;
        int after = Mathf.Max(0, before - info.damage);
        currentHp = after;
        
        healthChangedAction?.Raise(agentRoot.AgentId, currentHp, maxHp);

        if (IsDead)
        {
            TriggerDeathAnimation();
            DisableDeadComponents();
        }
    }

    private void TriggerDeathAnimation()
    {
        if (deathTriggered) return;
        if (string.IsNullOrEmpty(deathTrigger)) return;

        var animator = agentRoot.Animator;
        if (!animator) return;

        deathTriggered = true;
        if (!string.IsNullOrEmpty(hasWeaponParam))
            animator.SetBool(hasWeaponParam, false);
        animator.SetTrigger(deathTrigger);
    }

    private void DisableDeadComponents()
    {
        if (agentRoot == null) return;

        var navAgent = agentRoot.NavAgent;
        if (navAgent)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
            navAgent.speed = 0f;
            navAgent.acceleration = 0f;
            navAgent.angularSpeed = 0f;
        }

        var shooter = agentRoot.Shooter;
        if (shooter)
        {
            shooter.StopShooting();
            shooter.enabled = false;
        }

        var range = agentRoot.WeaponRange;
        if (range)
            range.enabled = false;
    }
}
