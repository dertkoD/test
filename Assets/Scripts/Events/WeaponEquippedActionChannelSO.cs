using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Action Channel/Weapon Equipped")]
public class WeaponEquippedActionChannelSO : ScriptableObject
{
    public event Action<int, int> OnEvent; // agentId, weaponId
    public void Raise(int agentId, int weaponId) => OnEvent?.Invoke(agentId, weaponId);
}
