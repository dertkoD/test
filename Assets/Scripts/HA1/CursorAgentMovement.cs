using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class CursorAgentMovement : MonoBehaviour
{
    [Serializable]
    public class AgentAnimLink
    {
        public NavMeshAgent agent;
        public Animator animator;
    }

    [Header("Scene References")]
    [SerializeField] private Camera cam;

    [Header("Agents (set in Inspector)")]
    [SerializeField] private List<AgentAnimLink> links = new();

    [Header("Input")]
    [Tooltip("Left click moves this agent index in Links list")]
    [SerializeField] private int leftClickAgentIndex = 0;

    [Tooltip("Right click moves this agent index in Links list")]
    [SerializeField] private int rightClickAgentIndex = 1;

    [Header("Animator Params")]
    [Tooltip("Must match Animator parameter name used by your 1D Blend Tree")]
    [SerializeField] private string speedParam = "Speed";

    [Tooltip("Smoothing for SetFloat (damp time)")]
    [SerializeField] private float speedDamp = 0.1f;

    private bool isGameActive = true;
    private bool hasClicked = false;

    private readonly HashSet<NavMeshAgent> arrivedAgents = new HashSet<NavMeshAgent>();

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Start()
    {
        foreach (var l in links)
        {
            if (l == null) continue;
            if (l.animator != null) l.animator.applyRootMotion = false;
        }
    }

    private void Update()
    {
        if (!isGameActive) return;
        if (cam == null) return;
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (TryGetMouseHitPoint(out var p))
                SetAgentDestination(leftClickAgentIndex, p);
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (TryGetMouseHitPoint(out var p))
                SetAgentDestination(rightClickAgentIndex, p);
        }

        if (hasClicked)
            CheckArrivalLogs();
    }

    private void LateUpdate()
    {
        foreach (var l in links)
        {
            if (l == null || l.agent == null || l.animator == null) continue;

            float speed01 = l.agent.velocity.magnitude / Mathf.Max(l.agent.speed, 0.01f);

            if (!l.agent.pathPending &&
                l.agent.remainingDistance <= l.agent.stoppingDistance &&
                (!l.agent.hasPath || l.agent.velocity.sqrMagnitude < 0.01f))
            {
                speed01 = 0f;
            }

            l.animator.SetFloat(speedParam, speed01, speedDamp, Time.deltaTime);
        }
    }

    private bool TryGetMouseHitPoint(out Vector3 point)
    {
        point = default;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.value);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            point = hit.point;
            return true;
        }

        return false;
    }

    private void SetAgentDestination(int linkIndex, Vector3 destination)
    {
        if (linkIndex < 0 || linkIndex >= links.Count) return;

        var l = links[linkIndex];
        if (l == null || l.agent == null) return;
        if (!l.agent.enabled || !l.agent.gameObject.activeInHierarchy) return;

        hasClicked = true;
        arrivedAgents.Clear();

        l.agent.SetDestination(destination);
    }

    private void CheckArrivalLogs()
    {
        foreach (var l in links)
        {
            if (l == null || l.agent == null) continue;

            var ag = l.agent;
            if (arrivedAgents.Contains(ag)) continue;

            if (!ag.pathPending &&
                ag.remainingDistance <= ag.stoppingDistance &&
                (!ag.hasPath || ag.velocity.sqrMagnitude < 0.01f))
            {
                arrivedAgents.Add(ag);
            }
        }
    }

    public void SetGameActive(bool value) => isGameActive = value;
    public bool IsGameActive() => isGameActive;

    public void MoveAgentByIndex(int linkIndex, Vector3 destination) => SetAgentDestination(linkIndex, destination);
}
