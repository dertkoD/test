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
        if (_consumed) return;

        // Если хотите, чтобы сенсоры не подбирали — оставляем.
        // Эта строка НЕ мешает обычному коллайдеру тела (isTrigger=false) подбирать.
        if (other.isTrigger) return;

        int bit = 1 << other.gameObject.layer;
        if ((allowedPickerMask.value & bit) == 0) return;

        if (!weaponPickedChannel) return;

        weaponPickedChannel.Raise(new WeaponPickupData
        {
            eventType = WeaponPickupEventType.Entered,
            pickerCollider = other,
            pickupTrigger = this,
            weaponId = weaponId,
            weaponPrefab = weaponPrefab
        });

    }

    public bool TryConsume(int pickerAgentId)
    {
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
        }

        Destroy(gameObject);
        return true;
    }
}