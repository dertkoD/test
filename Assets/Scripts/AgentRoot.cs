using System.Collections.Generic;
using UnityEngine;

public class AgentRoot : MonoBehaviour
{
    private static readonly Dictionary<int, AgentRoot> Registry = new();
    private static readonly Dictionary<Collider, AgentRoot> ColliderRegistry = new();

    [SerializeField] private int agentId;
    [SerializeField] private Transform handSocket;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider pickupBodyCollider;
    [SerializeField] private Transform aimTarget;
    [SerializeField] private Health health;
    public int AgentId => agentId; 
    public Transform HandSocket => handSocket; 
    public Animator Animator => animator;
    public Collider PickupBodyCollider => pickupBodyCollider;
    public Transform AimTarget => aimTarget;
    public Health Health => health;

    public static bool TryGetById(int id, out AgentRoot root) => Registry.TryGetValue(id, out root);
    public static bool TryGetByCollider(Collider collider, out AgentRoot root) => ColliderRegistry.TryGetValue(collider, out root);

    private void OnEnable()
    {
        if (pickupBodyCollider)
        {
            ColliderRegistry[pickupBodyCollider] = this;
        }

        if (agentId == 0) return;

        Registry[agentId] = this;
    }

    private void OnDisable()
    {
        if (Registry.TryGetValue(agentId, out var existing) && existing == this)
            Registry.Remove(agentId);

        if (pickupBodyCollider && ColliderRegistry.TryGetValue(pickupBodyCollider, out var existingCollider) && existingCollider == this)
            ColliderRegistry.Remove(pickupBodyCollider);
    }
}
