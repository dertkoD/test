using TMPro;
using UnityEngine;

public class UIHealth : MonoBehaviour
{
    [SerializeField] private HealthChangedActionChannelSO healthChangedAction; // AC_HealthChanged

    [SerializeField] private int agent1Id = 1;
    [SerializeField] private int agent2Id = 2;

    [SerializeField] private TMP_Text agent1Text;
    [SerializeField] private TMP_Text agent2Text;

    private void OnEnable()
    {
        if (healthChangedAction) healthChangedAction.OnEvent += OnHealthChanged;
    }

    private void OnDisable()
    {
        if (healthChangedAction) healthChangedAction.OnEvent -= OnHealthChanged;
    }

    private void OnHealthChanged(int agentId, int currentHp, int maxHp)
    {
        if (agentId == agent1Id && agent1Text) agent1Text.text = $"Swat (agent {agent1Id}): {currentHp}/{maxHp}";

        if (agentId == agent2Id && agent2Text) agent2Text.text = $"Anime (agent {agent2Id}): {currentHp}/{maxHp}";
    }
}
