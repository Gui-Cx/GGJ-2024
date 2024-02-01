using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
/// </summary>
[RequireComponent(typeof(NPCItemHandler))]
[RequireComponent(typeof(NPCHappinessBarController))]
public class NPCBehaviourController : MonoBehaviour
{
    #region DATA VARIABLES
    [Header("Data")]
    [SerializeField] private NPCSettingsData _settingsData;
    [SerializeField] private NPCMovementData _movementData;
    [SerializeField] private NPCTypeData _behaviourData;
    [Header("Elements")]
    [SerializeField] private Animator _animator;
    #endregion

    #region VARIABLES
    private Dictionary<ITEM_TYPE, NPC_STATE> _interactionDict;

    private NPCHappinessBarController _happinessBarController;
    private NPCMovementController _movementController;
    private NPCSymbolController _symbolController;

    private NPC_STATE _state;
    private ITEM_TYPE _wantedItem;
    private NPCBehaviourData _curNpcData;

    [HideInInspector] public Transform SpawnPoint;
    #endregion

    private void Awake()
    {
        _happinessBarController = GetComponent<NPCHappinessBarController>();
        _symbolController = GetComponent<NPCSymbolController>();
    }

    private void Start()
    {
        _happinessBarController.SetValues(_settingsData, _animator);
        
        SwitchState(NPC_STATE.Idle);
        SwitchNPCData();

        GameManager.Instance.UpdateTotalClientsCount();
    }

    /// <summary>
    /// Function that will handle the switch of interaction table
    /// </summary>
    public void SwitchNPCData()
    {
        int rng = UnityEngine.Random.Range(0, _behaviourData.BehaviourDatas.Length);
        _curNpcData = _behaviourData.BehaviourDatas[rng];

        UpdateInteractionTable();
    }

    /// <summary>
    /// Updates the dict of values that link the item types to the outcome (satisfied/not/dead)
    /// </summary>
    private void UpdateInteractionTable()
    {
        _interactionDict = new Dictionary<ITEM_TYPE, NPC_STATE>();

        foreach(var item in _curNpcData.InteractionTable)
        {
            if (item.Satisfies)
            {
                _interactionDict[item.Type] = NPC_STATE.Satisfied;
                _wantedItem = item.Type;
            }
            else if (item.Kills)
            {
                _interactionDict[item.Type] = NPC_STATE.Dead;
            }
        }

        foreach (ITEM_TYPE item in Enum.GetValues(typeof(ITEM_TYPE)))
        {
            if (!_interactionDict.ContainsKey(item))
            {
                _interactionDict[item] = NPC_STATE.NotSatisfied;
            }
        }

        _symbolController.UpdateSymbolItem(_wantedItem);
    }

    public void Dies()
    {
        Debug.Log("HE'S DEAD JOHN");
        _state = NPC_STATE.Dead;

        Destroy(gameObject); //TODO : PROBABLY CHANGE THAT
        GameManager.Instance.UpdateDeadClientsCount();
        NPCEvents.Instance.Event.Invoke(new NPCGameEventArg() { Npc=gameObject, Type=NPCGameEventType.Death});
    }

    #region STATE RELATED FUNCTIONS
    public void SwitchState(NPC_STATE state)
    {
        if (_state == NPC_STATE.Sad && state != NPC_STATE.Sad)
        {
            GameManager.Instance.UpdateSadCount(false);
        }
        if (_state != NPC_STATE.Sad && state == NPC_STATE.Sad)
        {
            GameManager.Instance.UpdateSadCount(true);
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
        SwitchNPCData();

        _happinessBarController.ActivateHappinessTime();
        _symbolController.HideSymbol();

        GameManager.Instance.UpdateSatisfiedClientsCount();
    }

    /// <summary>
    /// Function triggered (internally) when the incorrect item has been applied on the NPC
    /// </summary>
    public void IncorrectItemApplied()
    {
        SwitchState(NPC_STATE.NotSatisfied);
        StartCoroutine(NotSatisfiedTimer());

        GameManager.Instance.UpdateNotAmusedClientsCount();
    }

    /// <summary>
    /// Function that will handle the item that has been triggered on the 
    /// </summary>
    public void OnItemTriggered(ITEM_TYPE type)
    {
        if (_interactionDict.TryGetValue(type, out var npcState))
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
        _movementController.InitializeData(_movementData, _animator);
    }
    #endregion
}
