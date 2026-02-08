using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ShooterController : MonoBehaviour
{
    [SerializeField] private AgentRoot agentRoot;

    [Header("Weapon")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int weaponDamage = 10;
    [SerializeField] private float shotsPerSecond = 2f;
    [SerializeField] private Transform shotOrigin;
    [SerializeField] private Transform aimPivot;
    [SerializeField] private NavMeshAgent agentNav;
    [SerializeField] private bool instantTurn = true;
    [SerializeField] private float turnSpeed = 720f;
    [SerializeField] private float aimAngleThreshold = 5f;
    [SerializeField] private float moveSpeedThreshold = 0.05f;
    [SerializeField] private float muzzleForwardOffset = 0.05f;

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

        if (!aimPivot && agentRoot)
            aimPivot = agentRoot.transform;
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

    private void Update()
    {
        if (_currentTargetId == -1 || !_currentTarget) return;

        Vector3 targetPos = GetTargetPosition(_currentTarget);

        AimAtTarget(targetPos);

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

        if (_currentTarget)
        {
            Vector3 targetPos = GetTargetPosition(_currentTarget);
            AimAtTarget(targetPos);
        }

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

        var targetHealth = _currentTarget.Health;
        if (targetHealth && targetHealth.IsDead)
        {
            StopShooting();
            return;
        }

        if (IsMoving())
            return;

        Vector3 targetPos = GetTargetPosition(_currentTarget);

        if (!AimAtTarget(targetPos))
            return;

        Transform origin = shotOrigin ? shotOrigin : agentRoot.transform;
        Vector3 originPos = origin.position;

        if (_currentTarget.PickupBodyCollider)
            targetPos = _currentTarget.PickupBodyCollider.ClosestPoint(originPos);

        Vector3 dir = targetPos - originPos;
        if (dir.sqrMagnitude < 0.0001f)
            dir = (aimPivot ? aimPivot.forward : agentRoot.transform.forward);

        dir = dir.normalized;
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        float offset = Mathf.Max(0f, muzzleForwardOffset);
        Vector3 spawnPos = originPos + (dir * offset);

        Projectile proj = Instantiate(projectilePrefab, spawnPos, rotation);
        proj.Initialize(
            attackerId: agentRoot.AgentId, 
            targetId: targetId,
            damage: weaponDamage,
            damageEventChannel: damageEventChannel,
            attackerCollider: agentRoot.PickupBodyCollider
            );
    }

    public void SetShotOrigin(Transform origin)
    {
        if (origin)
            shotOrigin = origin;
        else if (agentRoot)
            shotOrigin = agentRoot.HandSocket;
    }

    private bool AimAtTarget(Vector3 targetPos)
    {
        if (!agentRoot) return false;

        Transform pivot = aimPivot ? aimPivot : agentRoot.transform;
        Vector3 toTarget = targetPos - pivot.position;
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude < 0.0001f) return false;

        Quaternion desired = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        if (instantTurn || turnSpeed <= 0f)
        {
            agentRoot.transform.rotation = desired;
            return true;
        }

        agentRoot.transform.rotation = Quaternion.RotateTowards(
            agentRoot.transform.rotation,
            desired,
            turnSpeed * Time.deltaTime
        );

        float angle = Vector3.Angle(pivot.forward, toTarget.normalized);
        return angle <= aimAngleThreshold;
    }

    private bool IsMoving()
    {
        if (!agentNav) return false;

        if (agentNav.pathPending)
            return true;

        if (agentNav.velocity.sqrMagnitude > moveSpeedThreshold * moveSpeedThreshold)
            return true;

        if (agentNav.hasPath && agentNav.remainingDistance > agentNav.stoppingDistance + 0.01f)
            return true;

        return false;
    }

    private Vector3 GetTargetPosition(AgentRoot target)
    {
        if (!target) return Vector3.zero;
        if (target.PickupBodyCollider) return target.PickupBodyCollider.bounds.center;
        return target.transform.position;
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
        return AgentRoot.TryGetById(targetId, out var target) ? target : null;
    }
}
