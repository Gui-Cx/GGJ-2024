using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object that will contain all the relevant data for the NPC's movement behaviour
/// </summary>
[CreateAssetMenu(fileName ="NPCMovementData",menuName ="Scriptable Object/NPC Movement Data")]
public class NPCMovementData : ScriptableObject
{
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float PauseTimer { get; private set; }
    [field:SerializeField] public float MovementCooldown { get; private set; }
}
