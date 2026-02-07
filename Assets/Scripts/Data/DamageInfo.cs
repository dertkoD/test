using System;
using UnityEngine;

[Serializable]
public struct DamageInfo
{
    public int attackerAgentId;
    public int targetAgentId;
    public int damage;

    public int hpBefore;
    public int hpAfter;

    public Vector3 hitPoint;
}
