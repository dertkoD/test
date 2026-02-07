using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private bool raceFinished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (raceFinished) return;

  
        if (other.CompareTag("Player"))
        {
            raceFinished = true;
            
            
            GameMenuManager manager = FindObjectOfType<GameMenuManager>();
            if (manager != null)
            {
                manager.OnAgentReachedGoal(other.name);
            }
        }
    }
}