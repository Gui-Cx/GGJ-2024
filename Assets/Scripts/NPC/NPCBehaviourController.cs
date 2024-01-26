using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
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
    Hug,
    Trumpet
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
    [SerializeField, InspectorName("Current NPC Data")] private NPCTypeData _curNpcData;
    [SerializeField, InspectorName("Available NPC Data")] private NPCTypeData[] _availableNPCData;
    #endregion

    #region VARIABLES
    private NPC_TYPES _type;
    private NPC_STATE _state;
    private Dictionary<ITEM_TYPE, NPC_STATE> _itemInteractionDict;
    #endregion

    private void OnValidate()
    {
        Assert.IsNotNull(_curNpcData);
    }

    private void Awake()
    {
        _state = NPC_STATE.Idle;
        UpdateItemInteractionTable();
    }

    private void UpdateItemInteractionTable()
    {
        _type = _curNpcData.Type;
        _itemInteractionDict = new Dictionary<ITEM_TYPE, NPC_STATE>();
        foreach (var item in _curNpcData.ItemInteractionTable)
        {
            _itemInteractionDict[item.Type] = item.OutcomeState;
        }
    }

    public void Dies()
    {
        Debug.Log("HE'S DEAD JOHN");
        gameObject.SetActive(false);
        NPCEvents.Instance.Event.Invoke(NPCGameEventType.Death);
    }

    /// <summary>
    /// Function that will handle the switch of interaction table
    /// </summary>
    public void SwitchNPCData()
    {
        int rng = UnityEngine.Random.Range(0, _availableNPCData.Length);
        _curNpcData = _availableNPCData[rng];
        UpdateItemInteractionTable();
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
        if(_itemInteractionDict.ContainsKey(type) && _itemInteractionDict[type] == NPC_STATE.Satisfied)
        {
            CorrectItemApplied();
        }
        else if(_itemInteractionDict.ContainsKey(type) && _itemInteractionDict[type] == NPC_STATE.Dead)
        {
            Dies();
        }
        else if(_itemInteractionDict.ContainsKey(type) && _itemInteractionDict[type] == NPC_STATE.NotSatisfied)
        {
            IncorrectItemApplied();
        }
        else
        {
            Debug.Log("No state change : NPC remains idle");
        }
    }
    #endregion
}
