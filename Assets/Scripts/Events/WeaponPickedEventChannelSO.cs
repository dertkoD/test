using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/UnityEvent Channel/Weapon Picked")]
public class WeaponPickedEventChannelSO : ScriptableObject
{
    [Serializable] public class WeaponPickedUnityEvent : UnityEvent<WeaponPickupData> {}

    [SerializeField] private WeaponPickedUnityEvent onEventRaised = new();

    public void Raise(WeaponPickupData data) => onEventRaised.Invoke(data);

    public void Register(UnityAction<WeaponPickupData> listener) => onEventRaised.AddListener(listener);
    public void Unregister(UnityAction<WeaponPickupData> listener) => onEventRaised.RemoveListener(listener);
}
