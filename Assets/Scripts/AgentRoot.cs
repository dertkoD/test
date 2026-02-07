using System.Collections.Generic;
using UnityEngine;

public class AgentRoot : MonoBehaviour
{
    private static readonly Dictionary<int, AgentRoot> Registry = new();

    [SerializeField] private int agentId;
    [SerializeField] private Transform handSocket;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider pickupBodyCollider;
    public int AgentId => agentId; 
    public Transform HandSocket => handSocket; 
    public Animator Animator => animator;
    public Collider PickupBodyCollider => pickupBodyCollider;

    public static bool TryGetById(int id, out AgentRoot root) => Registry.TryGetValue(id, out root);

    private void OnEnable()
    {
        if (agentId == 0) return;

        if (Registry.TryGetValue(agentId, out var existing) && existing != this)
            Debug.LogWarning($"[AgentRoot] Duplicate agentId {agentId} on {name}, replacing {existing.name}.");

        Registry[agentId] = this;
    }

    private void OnDisable()
    {
        if (Registry.TryGetValue(agentId, out var existing) && existing == this)
            Registry.Remove(agentId);
    }
}
