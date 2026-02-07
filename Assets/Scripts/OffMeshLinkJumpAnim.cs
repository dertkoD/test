using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class OffMeshLinkJumpAnim : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private string jumpTrigger = "Jump";

    private bool wasOnLink;

    void Update()
    {
        if (!agent || !animator) return;

        bool onLink = agent.isOnOffMeshLink;

        if (onLink && !wasOnLink)
        {
            animator.ResetTrigger(jumpTrigger);
            animator.SetTrigger(jumpTrigger);
        }

        wasOnLink = onLink;
    }
}
