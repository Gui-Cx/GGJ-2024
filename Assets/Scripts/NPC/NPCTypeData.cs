using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//basically, each item is assigned an "outcome" in terms of NPC state. For example, an old guy will respond "satisfied" if handled with hug, disatisfied otherwise, and DEAD with red nose
//So we create a table here that associates each item to an "outcome state"
[Serializable]
public struct ItemInteractionTable
{
    public ITEM_TYPE Type;
    public NPC_STATE OutcomeState;
}

/// <summary>
/// This script contains the data that each archetype of NPCs will have
/// </summary>
[CreateAssetMenu(menuName ="Scriptable Object/NPC Type",fileName ="NPCTypeData")]
public class NPCTypeData : ScriptableObject
{
    [field:SerializeField] public NPC_TYPES Type {  get; private set; }
    [field:SerializeField] public ItemInteractionTable[] ItemInteractionTable { get; private set; }

}
