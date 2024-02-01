using System;
using UnityEngine;

//Association between an item used on an NPC and an outcome state
[Serializable]
public struct ItemInteraction
{
    public ITEM_TYPE Type;
    public bool Satisfies;
    public bool Kills;
}

/// <summary>
/// This script contains the data that each archetype of NPCs will have
/// </summary>
[CreateAssetMenu(menuName ="Scriptable Object/NPC Behaviour",fileName ="NPCBehaviourData")]
public class NPCBehaviourData : ScriptableObject
{
    public ItemInteraction[] InteractionTable;
}
