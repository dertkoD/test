using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Action Channel/Entered Weapon Range")]
public class EnteredWeaponRangeActionChannelSO : ScriptableObject
{
    public event Action<int, int> OnEvent; // attackerId, targetId
    public void Raise(int attackerId, int targetId) => OnEvent?.Invoke(attackerId, targetId);
}
