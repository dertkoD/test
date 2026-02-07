using UnityEngine;

public class WeaponPickupTrigger : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private int weaponId = 1;
    [SerializeField] private WeaponView weaponPrefab;

    [Header("Filter")]
    [SerializeField] private LayerMask allowedPickerMask; // AgentBody

    [Header("Channel")]
    [SerializeField] private WeaponPickedEventChannelSO weaponPickedChannel;

    private bool _consumed;

    private void OnTriggerEnter(Collider other)
    {
        // Лог для проверки, что событие реально приходит
        Debug.Log($"[Pickup] Enter: other={other.name}, other.isTrigger={other.isTrigger}, layer={LayerMask.LayerToName(other.gameObject.layer)}");

        if (_consumed) return;

        // Если хотите, чтобы сенсоры не подбирали — оставляем.
        // Эта строка НЕ мешает обычному коллайдеру тела (isTrigger=false) подбирать.
        if (other.isTrigger) return;

        int bit = 1 << other.gameObject.layer;
        if ((allowedPickerMask.value & bit) == 0) return;

        if (!weaponPickedChannel)
        {
            Debug.LogWarning("[Pickup] weaponPickedChannel is NULL");
            return;
        }

        weaponPickedChannel.Raise(new WeaponPickupData
        {
            eventType = WeaponPickupEventType.Entered,
            pickerCollider = other,
            pickupTrigger = this,
            weaponId = weaponId,
            weaponPrefab = weaponPrefab
        });

        Debug.Log("[Pickup] Raised Entered");
    }

    public bool TryConsume(int pickerAgentId)
    {
        Debug.Log($"[Pickup] TryConsume by agentId={pickerAgentId}, consumed={_consumed}");

        if (_consumed) return false;
        _consumed = true;

        if (weaponPickedChannel)
        {
            weaponPickedChannel.Raise(new WeaponPickupData
            {
                eventType = WeaponPickupEventType.Picked,
                pickerAgentId = pickerAgentId,
                weaponId = weaponId,
                weaponPrefab = weaponPrefab
            });
            Debug.Log("[Pickup] Raised Picked");
        }

        Destroy(gameObject);
        return true;
    }
}