using UnityEngine;
using System.Collections;

public class ShooterController : MonoBehaviour
{
    [SerializeField] private AgentRoot agentRoot;

    [Header("Weapon")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int weaponDamage = 10;
    [SerializeField] private float shotsPerSecond = 2f;
    [SerializeField] private Transform shotOrigin;

    [Header("Events In (Action Channel)")]
    [SerializeField] private EnteredWeaponRangeActionChannelSO enteredRangeAction;

    [Header("Events Out (UnityEvent Channel)")]
    [SerializeField] private DamageEventChannelSO damageEventChannel;

    private int _currentTargetId = -1;
    private AgentRoot _currentTarget;
    private Coroutine _shootRoutine;

    private void Awake()
    {
        if (!shotOrigin && agentRoot)
            shotOrigin = agentRoot.HandSocket;
    }

    private void OnEnable()
    {
        if (enteredRangeAction) enteredRangeAction.OnEvent += OnEnteredRange;
    }

    private void OnDisable()
    {
        if (enteredRangeAction) enteredRangeAction.OnEvent -= OnEnteredRange;

        StopShooting();
    }

    private void OnEnteredRange(int attackerId, int targetId)
    {
        if (!agentRoot) return;
        if (attackerId != agentRoot.AgentId) return;

        if (targetId < 0)
        {
            StopShooting();
            return;
        }

        _currentTargetId = targetId;
        _currentTarget = ResolveTarget(targetId);

        _shootRoutine ??= StartCoroutine(ShootLoop());
    }

    private IEnumerator ShootLoop()
    {
        float interval = Mathf.Max(0.05f, 1f / Mathf.Max(0.1f, shotsPerSecond));
        var wait = new WaitForSeconds(interval);

        while (_currentTargetId != -1)
        {
            FireOnce(_currentTargetId);
            yield return wait;
        }

        _shootRoutine = null;
    }

    private void FireOnce(int targetId)
    { 
        if (!projectilePrefab || !damageEventChannel || !agentRoot) return;

        if (_currentTarget == null || _currentTarget.AgentId != targetId)
            _currentTarget = ResolveTarget(targetId);

        if (_currentTarget == null)
            return;

        var targetHealth = _currentTarget.GetComponent<Health>();
        if (targetHealth && targetHealth.IsDead)
        {
            StopShooting();
            return;
        }

        Transform origin = shotOrigin ? shotOrigin : agentRoot.transform;
        Vector3 originPos = origin.position;
        Vector3 targetPos = _currentTarget.PickupBodyCollider
            ? _currentTarget.PickupBodyCollider.bounds.center
            : _currentTarget.transform.position;

        Vector3 dir = targetPos - originPos;
        if (dir.sqrMagnitude < 0.0001f)
            dir = origin.forward;

        Quaternion rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);

        Projectile proj = Instantiate(projectilePrefab, originPos, rotation);
        proj.Initialize(
            attackerId: agentRoot.AgentId, 
            targetId: targetId,
            damage: weaponDamage,
            damageEventChannel: damageEventChannel,
            attackerCollider: agentRoot.PickupBodyCollider
            );
    }

    public void StopShooting()
    {
        _currentTargetId = -1;
        _currentTarget = null;

        if (_shootRoutine != null)
        {
            StopCoroutine(_shootRoutine);
            _shootRoutine = null;
        }
    }

    private AgentRoot ResolveTarget(int targetId)
    {
        if (AgentRoot.TryGetById(targetId, out var target))
            return target;

        var roots = FindObjectsOfType<AgentRoot>();
        foreach (var root in roots)
        {
            if (root && root.AgentId == targetId)
                return root;
        }

        return null;
    }
}
