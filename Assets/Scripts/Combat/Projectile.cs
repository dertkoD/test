using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 18f;
    [SerializeField] private float lifetime = 3f;

    private int _attackerId;
    private int _targetId;
    private int _damage;
    private DamageEventChannelSO _damageEventChannel;
    private Collider _attackerCollider;

    private float _alive;

    public void Initialize(int attackerId, int targetId, int damage, DamageEventChannelSO damageEventChannel, Collider attackerCollider)
    {
        _attackerId = attackerId;
        _targetId = targetId;
        _damage = damage;
        _damageEventChannel = damageEventChannel;
        _attackerCollider = attackerCollider;
    }

    private void Update()
    {
        transform.position += transform.forward * (speed * Time.deltaTime);

        _alive += Time.deltaTime;
        if (_alive >= lifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        if (_attackerCollider && other == _attackerCollider) return;

        var otherAgent = other.GetComponentInParent<AgentRoot>();
        if (otherAgent != null && otherAgent.AgentId == _attackerId)
            return;

        if (otherAgent != null && otherAgent.AgentId != _targetId)
        {
            Destroy(gameObject);
            return;
        }

        if (otherAgent == null)
        {
            Destroy(gameObject);
            return;
        }

        var info = new DamageInfo
        {
            attackerAgentId = _attackerId,
            targetAgentId   = _targetId,
            damage          = _damage,
            hpBefore        = -1,
            hpAfter         = -1,
            hitPoint        = transform.position
        };

        _damageEventChannel?.Raise(info);
        Destroy(gameObject);
    }
}
