using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Action Channel/Game Over")]
public class GameOverActionChannelSO : ScriptableObject
{
    public event Action<int> OnEvent;
    public void Raise(int winnerAgentId) => OnEvent?.Invoke(winnerAgentId);
}
