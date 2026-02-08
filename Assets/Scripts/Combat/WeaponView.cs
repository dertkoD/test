using UnityEngine;

public class WeaponView : MonoBehaviour
{
    [SerializeField] private Transform muzzle;
    public Transform Muzzle => muzzle;
}
