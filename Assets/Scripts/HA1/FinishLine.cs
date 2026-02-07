using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private bool raceFinished = false;
    [SerializeField] private GameMenuManager menuManager;

    private void OnTriggerEnter(Collider other)
    {
        if (raceFinished) return;

  
        if (other.CompareTag("Player"))
        {
            raceFinished = true;

            if (menuManager != null)
                menuManager.OnAgentReachedGoal(other.name);
        }
    }
}