using UnityEngine;

public class WeaponEquipper : MonoBehaviour
{
    [SerializeField] private AgentRoot agentRoot;
    [SerializeField] private Transform handSocket;
    [SerializeField] private string muzzleName = "Muzzle";

    [Header("Channel")]
    [SerializeField] private WeaponPickedEventChannelSO weaponPickedChannel;

    [Header("Broadcasting To")]
    [SerializeField] private WeaponEquippedActionChannelSO weaponEquippedAction;

    private GameObject _currentWeaponInstance;

    private bool HasWeapon => _currentWeaponInstance != null;

    private void Awake()
    {
        if (agentRoot != null && handSocket == null)
            handSocket = agentRoot.HandSocket;
    }

    private void OnEnable()
    {
        if (weaponPickedChannel)
            weaponPickedChannel.Register(OnWeaponEvent);
    }

    private void OnDisable()
    {
        if (weaponPickedChannel)
            weaponPickedChannel.Unregister(OnWeaponEvent);
    }

    private void OnWeaponEvent(WeaponPickupData data)
    {
        if (!agentRoot) return;

        if (data.eventType == WeaponPickupEventType.Entered)
        {
            if (HasWeapon) return;

            if (data.pickerCollider != agentRoot.PickupBodyCollider) return;

            if (data.pickupTrigger == null) return;

            data.pickupTrigger.TryConsume(agentRoot.AgentId);
            return;
        }

        if (data.eventType == WeaponPickupEventType.Picked)
        {
            if (HasWeapon) return;
            if (data.pickerAgentId != agentRoot.AgentId) return;

            _currentWeaponInstance = Instantiate(data.weaponPrefab, handSocket);
            _currentWeaponInstance.transform.localPosition = Vector3.zero;
            _currentWeaponInstance.transform.localRotation = Quaternion.identity;

            var shooter = agentRoot.GetComponent<ShooterController>();
            if (shooter)
            {
                var muzzle = FindMuzzle(_currentWeaponInstance.transform);
                shooter.SetShotOrigin(muzzle ? muzzle : handSocket);
            }

            if (weaponEquippedAction)
                weaponEquippedAction.Raise(agentRoot.AgentId, data.weaponId);
        }
    }

    private Transform FindMuzzle(Transform weaponRoot)
    {
        if (!weaponRoot) return null;

        var transforms = weaponRoot.GetComponentsInChildren<Transform>(true);
        foreach (var t in transforms)
        {
            if (t.name == muzzleName)
                return t;
        }

        return null;
    }
}