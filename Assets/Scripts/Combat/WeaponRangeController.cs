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

    private int _currentTargetId = -1;

    private void Awake()
    {
        if (targetMask.value == 0)
        {
            int layer = LayerMask.NameToLayer("AgentBody");
            if (layer >= 0)
                targetMask = 1 << layer;
        }

        // Скрываем радиус при старте
        ToggleRange(false);
    }

    private void OnEnable()
    {
        if (weaponEquippedAction) 
            weaponEquippedAction.OnEvent += OnWeaponEquipped; // Action channel (C# event style)
    }

    private void OnDisable()
    {
        if (weaponEquippedAction) 
            weaponEquippedAction.OnEvent -= OnWeaponEquipped;
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
            // Сообщаем ShooterController'у начать огонь
            _currentTargetId = otherAgent.AgentId;
            enteredRangeAction.Raise(agentRoot.AgentId, _currentTargetId);
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

        if (otherAgent.AgentId == _currentTargetId)
        {
            _currentTargetId = -1;
            enteredRangeAction.Raise(agentRoot.AgentId, -1);
        }
    }
}