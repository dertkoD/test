using System.Collections.Generic;
using UnityEngine;

public class WeaponRangeController : MonoBehaviour
{
    [SerializeField] private AgentRoot agentRoot;

    [Header("Range Components")]
    [SerializeField] private SphereCollider rangeCollider; // Триггер радиуса
    [SerializeField] private GameObject rangeVisual;       // Визуальный круг
    [SerializeField] private LayerMask targetMask;

    [Header("Listening To")]
    [SerializeField] private WeaponEquippedActionChannelSO weaponEquippedAction;

    [Header("Broadcasting To")]
    [SerializeField] private EnteredWeaponRangeActionChannelSO enteredRangeAction;
    [SerializeField] private string aimParam = "Aim";
    [SerializeField] private bool controlUpperBodyLayer = true;
    [SerializeField] private string upperBodyLayerName = "UpperBody";
    [SerializeField] private float upperBodyWeightWhenAiming = 0f;
    [SerializeField] private float upperBodyWeightWhenIdle = 1f;

    private int _currentTargetId = -1;
    private readonly HashSet<int> targetsInRange = new HashSet<int>();
    private int upperBodyLayerIndex = -1;

    private void Awake()
    {
        if (targetMask.value == 0)
        {
            int layer = LayerMask.NameToLayer("AgentBody");
            if (layer >= 0)
                targetMask = 1 << layer;
        }

        CacheUpperBodyLayer();

        // Скрываем радиус при старте
        ToggleRange(false);
    }

    private void OnEnable()
    {
        if (weaponEquippedAction) 
            weaponEquippedAction.OnEvent += OnWeaponEquipped; // Action channel (C# event style)

        CacheUpperBodyLayer();
    }

    private void OnDisable()
    {
        if (weaponEquippedAction) 
            weaponEquippedAction.OnEvent -= OnWeaponEquipped;

        targetsInRange.Clear();
        _currentTargetId = -1;
        SetAim(false);
    }

    private void OnWeaponEquipped(int agentId, int weaponId)
    {
        if (!agentRoot) return;

        // Если событие касается МЕНЯ
        if (agentId == agentRoot.AgentId)
        {
            // Включаем "боевой режим"
            ToggleRange(true);
            
            // Здесь можно добавить логику изменения размера радиуса в зависимости от weaponId
            // if (weaponId == 2) rangeCollider.radius = 10f;
        }
    }

    private void ToggleRange(bool state)
    {
        if (rangeCollider) rangeCollider.enabled = state;
        if (rangeVisual) rangeVisual.SetActive(state);

        if (!state)
        {
            targetsInRange.Clear();
            _currentTargetId = -1;
            SetAim(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!agentRoot || !enteredRangeAction) return;
        if (other.isTrigger) return;
        if (other == agentRoot.PickupBodyCollider) return;

        int bit = 1 << other.gameObject.layer;
        if ((targetMask.value & bit) == 0) return;

        if (!AgentRoot.TryGetByCollider(other, out var otherAgent)) return;

        // Если это агент и это НЕ я
        if (otherAgent != null && otherAgent.AgentId != agentRoot.AgentId)
        {
            if (targetsInRange.Add(otherAgent.AgentId))
            {
                SetAim(true);

                if (_currentTargetId == -1)
                {
                    _currentTargetId = otherAgent.AgentId;
                    enteredRangeAction.Raise(agentRoot.AgentId, _currentTargetId);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!agentRoot || !enteredRangeAction) return;
        if (other.isTrigger) return;
        if (other == agentRoot.PickupBodyCollider) return;

        int bit = 1 << other.gameObject.layer;
        if ((targetMask.value & bit) == 0) return;

        if (!AgentRoot.TryGetByCollider(other, out var otherAgent)) return;

        if (targetsInRange.Remove(otherAgent.AgentId))
        {
            if (otherAgent.AgentId == _currentTargetId)
            {
                if (targetsInRange.Count > 0)
                {
                    foreach (var id in targetsInRange)
                    {
                        _currentTargetId = id;
                        enteredRangeAction.Raise(agentRoot.AgentId, _currentTargetId);
                        break;
                    }
                }
                else
                {
                    _currentTargetId = -1;
                    enteredRangeAction.Raise(agentRoot.AgentId, -1);
                }
            }

            if (targetsInRange.Count == 0)
                SetAim(false);
        }
    }

    private void SetAim(bool state)
    {
        if (!agentRoot) return;

        var animator = agentRoot.Animator;
        if (!animator || string.IsNullOrEmpty(aimParam)) return;

        animator.SetBool(aimParam, state);

        if (!controlUpperBodyLayer) return;

        if (upperBodyLayerIndex < 0)
            CacheUpperBodyLayer();

        if (upperBodyLayerIndex >= 0)
            animator.SetLayerWeight(upperBodyLayerIndex, state ? upperBodyWeightWhenAiming : upperBodyWeightWhenIdle);
    }

    private void CacheUpperBodyLayer()
    {
        upperBodyLayerIndex = -1;
        if (!controlUpperBodyLayer || !agentRoot) return;

        var animator = agentRoot.Animator;
        if (!animator || string.IsNullOrEmpty(upperBodyLayerName)) return;

        upperBodyLayerIndex = animator.GetLayerIndex(upperBodyLayerName);
    }
}