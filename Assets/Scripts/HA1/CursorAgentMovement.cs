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

    [Header("Raycast")]
    [SerializeField] private LayerMask clickMask = ~0;

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
    [SerializeField] private string xVelocityParam = "xVelocity";
    [SerializeField] private string yVelocityParam = "yVelocity";
    [SerializeField] private bool writeVelocityParams = true;
    [SerializeField] private bool normalizeVelocityParams = true;

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
            if (!IsAgentNavReady(l.agent))
            {
                l.animator.SetFloat(speedParam, 0f, speedDamp, Time.deltaTime);
                continue;
            }

            float speed01 = l.agent.velocity.magnitude / Mathf.Max(l.agent.speed, 0.01f);

            if (!l.agent.pathPending &&
                l.agent.remainingDistance <= l.agent.stoppingDistance &&
                (!l.agent.hasPath || l.agent.velocity.sqrMagnitude < 0.01f))
            {
                speed01 = 0f;
            }

            l.animator.SetFloat(speedParam, speed01, speedDamp, Time.deltaTime);

            if (writeVelocityParams)
            {
                Vector3 localVel = l.agent.transform.InverseTransformDirection(l.agent.velocity);
                float denom = normalizeVelocityParams ? Mathf.Max(l.agent.speed, 0.01f) : 1f;
                float xVal = localVel.x / denom;
                float yVal = localVel.z / denom;
                l.animator.SetFloat(xVelocityParam, xVal, speedDamp, Time.deltaTime);
                l.animator.SetFloat(yVelocityParam, yVal, speedDamp, Time.deltaTime);
            }
        }
    }

    private bool TryGetMouseHitPoint(out Vector3 point)
    {
        point = default;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.value);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickMask))
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
        if (!IsAgentNavReady(l.agent)) return;

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
            if (!IsAgentNavReady(ag)) continue;

            if (!ag.pathPending &&
                ag.remainingDistance <= ag.stoppingDistance &&
                (!ag.hasPath || ag.velocity.sqrMagnitude < 0.01f))
            {
                arrivedAgents.Add(ag);
            }
        }
    }

    public void SetGameActive(bool value) => isGameActive = value;

    private static bool IsAgentNavReady(NavMeshAgent agent)
    {
        if (!agent) return false;
        if (!agent.enabled || !agent.gameObject.activeInHierarchy) return false;
        return agent.isOnNavMesh;
    }
}
