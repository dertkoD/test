using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Action Channel/Health Changed")]
public class HealthChangedActionChannelSO : ScriptableObject
{
    public event Action<int, int, int> OnEvent; // agentId, currentHp, maxHp
    public void Raise(int agentId, int currentHp, int maxHp) => OnEvent?.Invoke(agentId, currentHp, maxHp);
}
