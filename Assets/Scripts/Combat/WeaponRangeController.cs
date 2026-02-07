using UnityEngine;

public class WeaponRangeController : MonoBehaviour
{
    [SerializeField] private AgentRoot agentRoot;

    [Header("Range Components")]
    [SerializeField] private SphereCollider rangeCollider; // Триггер радиуса
    [SerializeField] private GameObject rangeVisual;       // Визуальный круг

    [Header("Listening To")]
    [SerializeField] private WeaponEquippedActionChannelSO weaponEquippedAction;

    [Header("Broadcasting To")]
    [SerializeField] private EnteredWeaponRangeActionChannelSO enteredRangeAction;

    private void Awake()
    {
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

        // Проверяем, кто вошел в радиус
        var otherAgent = other.GetComponentInParent<AgentRoot>();

        // Если это агент и это НЕ я
        if (otherAgent != null && otherAgent.AgentId != agentRoot.AgentId)
        {
            // Сообщаем ShooterController'у начать огонь
            enteredRangeAction.Raise(agentRoot.AgentId, otherAgent.AgentId);
        }
    }
}