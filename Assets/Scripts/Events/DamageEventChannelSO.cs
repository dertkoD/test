using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/UnityEvent Channel/Damage Applied")]
public class DamageEventChannelSO : ScriptableObject
{
    [Serializable] public class DamageUnityEvent : UnityEvent<DamageInfo> {}

    [SerializeField] private DamageUnityEvent onEventRaised = new();

    public void Raise(DamageInfo info) => onEventRaised.Invoke(info);

    public void Register(UnityAction<DamageInfo> listener) => onEventRaised.AddListener(listener);
    public void Unregister(UnityAction<DamageInfo> listener) => onEventRaised.RemoveListener(listener);
}
