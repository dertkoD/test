using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private AgentRoot agentRoot;

    [Header("Stats")]
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int currentHp;

    [Header("Events In (UnityEvent Channel)")]
    [SerializeField] private DamageEventChannelSO damageEventChannel;

    [Header("Events Out (Action Channel)")]
    [SerializeField] private HealthChangedActionChannelSO healthChangedAction;

    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;
    public bool IsDead => currentHp <= 0;

    private void Awake()
    {
        if (currentHp <= 0)
            currentHp = maxHp;
    }

    private void OnEnable()
    {
        if (damageEventChannel) damageEventChannel.Register(OnDamageReceived);

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
    }
}
