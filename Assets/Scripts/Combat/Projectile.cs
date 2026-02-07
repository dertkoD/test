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

    private float _alive;

    public void Initialize(int attackerId, int targetId, int damage, DamageEventChannelSO damageEventChannel)
    {
        _attackerId = attackerId;
        _targetId = targetId;
        _damage = damage;
        _damageEventChannel = damageEventChannel;
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
