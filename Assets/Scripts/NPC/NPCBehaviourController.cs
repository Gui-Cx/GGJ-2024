using System;
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

public enum NPC_STATE
{
    Idle,
    Satisfied,
    NotSatisfied,
    Dead,
    Sad
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
    #region DATA VARIABLES
    [SerializeField] private Animator _anim;
    [SerializeField] private NPCMovementData _movementData;
    [Space(10)]
    [SerializeField] private NPCTypeData[] _availableNPCData;
    #endregion

    #region VARIABLES
    private Dictionary<ITEM_TYPE, NPC_STATE> _itemInteractionDict;
    private NPCHappinessBarController _happinessBarController;
    private NPCSymbolController _symbolController;
    private NPCMovementController _movementController;

    private NPC_STATE _state;
    private ITEM_TYPE _wantedItem;
    private NPCTypeData _curNpcData;

    [HideInInspector] public Transform SpawnPoint;
    #endregion

    private void OnValidate()
    {
        Assert.IsNotNull(_movementData);
    }

    private void Awake()
    {
        _happinessBarController = GetComponent<NPCHappinessBarController>();
        _symbolController = GetComponent<NPCSymbolController>();
    }

    private void Start()
    {
        SwitchState(NPC_STATE.Idle);
        SwitchNPCData();

        UpdateItemInteractionTable();
        GameManager.Instance.UpdateNumberTotalOfClients();
    }

    /// <summary>
    /// Updates the dict of values that link the item types to the outcome (satisfied/not/dead)
    /// </summary>
    private void UpdateItemInteractionTable()
    {
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

        Destroy(gameObject); //TODO : PROBABLY CHANGE THAT
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
        if (_state == NPC_STATE.Sad && state != NPC_STATE.Sad)
        {
            GameManager.Instance.UpdateSadNumber(false);
        }
        if (_state != NPC_STATE.Sad && state == NPC_STATE.Sad)
        {
            GameManager.Instance.UpdateSadNumber(true);
        }

        _state = state;
        _symbolController.UpdateSymbolItem(_wantedItem);

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
        _symbolController.HideSymbol();
        GameManager.Instance.UpdateNumberOfSatisfiedClients();
        SwitchNPCData();
    }

    /// <summary>
    /// Function triggered (internally) when the incorrect item has been applied on the NPC
    /// </summary>
    public void IncorrectItemApplied()
    {
        GameManager.Instance.UpdateNumberOfNotAmusedClients();
        SwitchState(NPC_STATE.NotSatisfied);
        StartCoroutine(NotSatisfiedTimer());
    }

    /// <summary>
    /// Function that will handle the item that has been triggered on the 
    /// </summary>
    public void OnItemTriggered(ITEM_TYPE type)
    {
        if (_itemInteractionDict.TryGetValue(type, out var npcState))
        {
            if (!_happinessBarController.HappinessIsActive)
            {
                if (npcState == NPC_STATE.Satisfied)
                {
                    CorrectItemApplied();
                }
                else if (npcState == NPC_STATE.NotSatisfied)
                {
                    IncorrectItemApplied();
                }
            }
            else if (npcState == NPC_STATE.Dead)
            {
                Dies();
            }
        }
    }
    #endregion

    #region MOVEMENT RELATED FUNCTIONS

    /// <summary>
    /// Function that will render that NPC a moving one. To be certain, it should be called immediately after spawn.
    /// Should only be called by the spawner
    /// </summary>
    public void MakeMovingNPC()
    {
        _movementController = gameObject.AddComponent<NPCMovementController>();
        _movementController.InitializeData(_movementData, _anim);
    }
    #endregion
}
