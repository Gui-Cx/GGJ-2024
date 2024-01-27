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
[RequireComponent(typeof(NPCHappinessBarController))]
public class NPCBehaviourController : MonoBehaviour
{
    #region SERIALIZED VARIABLES
    [SerializeField, InspectorName("Current NPC Data")] private NPCTypeData _curNpcData;
    [SerializeField, InspectorName("Available NPC Data")] private NPCTypeData[] _availableNPCData;
    #endregion

    #region VARIABLES
    private NPC_TYPES _type;
    [SerializeField] private NPC_STATE _state;
    private Dictionary<ITEM_TYPE, NPC_STATE> _itemInteractionDict;
    private NPCHappinessBarController _happinessBarController;
    private NPCSymbolController _symbolController;

    private ITEM_TYPE _wantedItem;

    [HideInInspector] public Transform SpawnPoint;
    #endregion

    private void OnValidate()
    {
        Assert.IsNotNull(_curNpcData);
    }

    private void Awake()
    {
        _happinessBarController = GetComponent<NPCHappinessBarController>();
        _symbolController = GetComponent<NPCSymbolController>();
    }

    private void Start()
    {
        SwitchState(NPC_STATE.Idle);
        UpdateItemInteractionTable();
    }

    /// <summary>
    /// Updates the dict of values that link the item types to the outcome (satisfied/not/dead)
    /// </summary>
    private void UpdateItemInteractionTable()
    {
        _type = _curNpcData.Type;
        _itemInteractionDict = new Dictionary<ITEM_TYPE, NPC_STATE>();
        foreach (var item in _curNpcData.ItemInteractionTable)
        {
            _itemInteractionDict[item.Type] = item.OutcomeState;
            if(item.OutcomeState == NPC_STATE.Satisfied)
            {
                _wantedItem = item.Type;
            }
        }
        _symbolController.UpdateSymbolItem(_wantedItem);
    }

    public void Dies()
    {
        Debug.Log("HE'S DEAD JOHN");
        _state = NPC_STATE.Dead;
        //gameObject.SetActive(false); //TODO : PROBABLY CHANGE THAT
        Destroy(gameObject);
        GameManager.Instance.UpdateNumberOfDeadClients();
        NPCEvents.Instance.Event.Invoke(new NPCGameEventArg() { Npc=gameObject, Type=NPCGameEventType.Death});
    }

    /// <summary>
    /// Function that will handle the switch of interaction table
    /// </summary>
    public void SwitchNPCData()
    {
        int rng = UnityEngine.Random.Range(0, _availableNPCData.Length);
        _curNpcData = _availableNPCData[rng];
        UpdateItemInteractionTable();
    }

    #region STATE RELATED FUNCTIONS
    public void SwitchState(NPC_STATE state)
    {
        _state = state;
        _symbolController.UpdateSymbolItem(_wantedItem);
        //Debug.Log("NPC " + this.gameObject.name + " : Switching back to state : " + _state);
        if(_state == NPC_STATE.Idle) 
        {
            _symbolController.DisplaySymbol();
        }
    }

    private IEnumerator NotSatisfiedTimer()
    {
        yield return new WaitForSeconds(1f);
        SwitchState(NPC_STATE.Idle);
    }
    #endregion

    #region ITEM RELATED FUNCTIONS
    /// <summary>
    /// Function triggered (internally) when the correct item has been applied on the NPC
    /// It will trigger its happiness time then go back to idle once that's done
    /// </summary>
    public void CorrectItemApplied()
    {
        SwitchState(NPC_STATE.Satisfied);
        _happinessBarController.ActivateHappinessTime();
        _symbolController.HideSymbolDuringHappinessTime();
        GameManager.Instance.UpdateNumberOfSatisfiedClients();
        SwitchNPCData();
        //Debug.Log("NPC " + this.gameObject.name + " : CORRECT ITEM APPLIED | State : "+_state+" and switching data");
    }

    /// <summary>
    /// Function triggered (internally) when the incorrect item has been applied on the NPC
    /// </summary>
    public void IncorrectItemApplied()
    {
        //Debug.Log("NPC " + this.gameObject.name + " : INCORRECT ITEM APPLIED");
        GameManager.Instance.UpdateNumberOfNotAmusedClients();
        SwitchState(NPC_STATE.NotSatisfied);
        StartCoroutine(NotSatisfiedTimer());
    }

    /// <summary>
    /// Function that will handle the item that has been triggered on the 
    /// </summary>
    public void OnItemTriggered(ITEM_TYPE type)
    {
        Debug.LogFormat("Cx : {0} received item {1}, expected {2}, result {3}", gameObject.name, type, _itemInteractionDict.Keys.ToString(), _itemInteractionDict[type]);
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
