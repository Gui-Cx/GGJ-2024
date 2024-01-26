using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public enum NPC_TYPES
{
    Old,
    Baby,
    Depressed,
    Death,
    Wheelchair,
    SadClown
}

public enum ITEM_TYPE //TODO : MOVE OUT OF HERE
{
    RedNose,
    Flower,
    Ballon,
    Pie,
    Hug
}

public enum NPC_STATE
{
    Idle,
    Satisfied,
    NotSatisfied,
    Dead
}

/// <summary>
/// This script will handle the basic behaviours of the NPCs 
/// Said behaviour :
/// - Static NPC
/// - Moving NPCs
/// 
/// This script will also handle the "type" of NPC and what it requests :
/// Types :
/// - Old
/// - Young (Babies)
/// - Depressed (Adult random)
/// - Death
/// - Wheelchair
/// - Sad clowns
/// </summary>
[RequireComponent(typeof(NPCItemHandler))]
public class NPCBehaviourController : MonoBehaviour
{
    #region SERIALIZED VARIABLES
    [field:SerializeField]public NPC_TYPES NpcType { get; private set; }
    [SerializeField, InspectorName("Item Type")] private ITEM_TYPE _itemType;
    #endregion

    #region VARIABLES
    private NPC_STATE _state;
    #endregion

    private void Awake()
    {
        _state = NPC_STATE.Idle;
    }

    #region ITEM RELATED FUNCTIONS
    /// <summary>
    /// Function triggered (internally) when the correct item has been applied on the NPC
    /// </summary>
    public void CorrectItemApplied()
    {
        Debug.Log("NPC " + this.gameObject.name + " : CORRECT ITEM APPLIED");
        _state = NPC_STATE.Satisfied;
    }

    /// <summary>
    /// Function triggered (internally) when the incorrect item has been applied on the NPC
    /// It will trigger either DEATH (for example rednose to the old) OR simply nothing
    /// </summary>
    public void IncorrectItemApplied()
    {
        Debug.Log("NPC " + this.gameObject.name + " : INCORRECT ITEM APPLIED");
        _state = NPC_STATE.NotSatisfied;
    }

    /// <summary>
    /// Function that will handle the item that has been triggered on the 
    /// </summary>
    public void OnItemTriggered(ITEM_TYPE type)
    {
        Debug.Log("NPC " + this.gameObject.name + " : Item " + type + " APPLIED");
        if (type == _itemType)
        {
            CorrectItemApplied();
        }
        else
        {
            IncorrectItemApplied();
        }
    }
    #endregion
}
