using UnityEngine;
using System.Collections;

public class ShooterController : MonoBehaviour
{
    [SerializeField] private AgentRoot agentRoot;

    [Header("Weapon")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int weaponDamage = 10;
    [SerializeField] private float shotsPerSecond = 2f;

    [Header("Events In (Action Channel)")]
    [SerializeField] private EnteredWeaponRangeActionChannelSO enteredRangeAction;

    [Header("Events Out (UnityEvent Channel)")]
    [SerializeField] private DamageEventChannelSO damageEventChannel;

    private int _currentTargetId = -1;
    private Coroutine _shootRoutine;

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

        _currentTargetId = targetId;

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

        Projectile proj = Instantiate(projectilePrefab);
        proj.Initialize(
            attackerId: agentRoot.AgentId, 
            targetId: targetId,
            damage: weaponDamage,
            damageEventChannel: damageEventChannel
            );
    }

    public void StopShooting()
    {
        _currentTargetId = -1;

        if (_shootRoutine != null)
        {
            StopCoroutine(_shootRoutine);
            _shootRoutine = null;
        }
    }
}
