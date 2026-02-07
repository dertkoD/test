using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
    [SerializeField] private AgentRoot agentRoot;
    [SerializeField] private Animator animator;
    [SerializeField] private WeaponEquippedActionChannelSO weaponEquipped;

    [Header("Animator params")]
    [SerializeField] private string hasWeaponParam = "HasWeapon";

    private void OnEnable()
    {
        if (weaponEquipped) weaponEquipped.OnEvent += OnWeaponEquipped;
    }

    private void OnDisable()
    {
        if (weaponEquipped) weaponEquipped.OnEvent -= OnWeaponEquipped;
    }

    private void OnWeaponEquipped(int agentId, int weaponId)
    {
        if (!agentRoot || !animator) return;
        if (agentId != agentRoot.AgentId) return;

        animator.SetBool(hasWeaponParam, true);
    }
}
