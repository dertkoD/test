using System;
using UnityEngine;

[Serializable] 
public struct WeaponPickupData
{
    public WeaponPickupEventType eventType;

    public int pickerAgentId;

    public Collider pickerCollider;
    public WeaponPickupTrigger pickupTrigger;

    public int weaponId;
    public WeaponView weaponPrefab;
}
