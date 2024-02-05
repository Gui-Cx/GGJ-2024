using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using static NPCEvents;

/// <summary>
/// Script that will handle the spawn of the NPCs
/// It will work like this :
/// - every x seconds, IF the number of NPCs is below max capacity, it will (random-ass random) spawn a new npc (or not lol)
/// - IF a NPC dies, then the next time a npc is supposed to spawn (random) then IT WILL SPAWN
/// ALSO, at the game's init, it will handle the initial NPC Spawn
/// </summary>
public class NPCSpawnManager : MonoBehaviour
{
    #region VARIABLES
    [Header("Spawn variables")]
    [SerializeField] private float _spawnTimer=10f;
    [SerializeField, Range(0, 100)] private int _spawnChance;
    [SerializeField, Range(0, 100)] private int _movementChance;

    [Header("Spawn points")]
    [SerializeField] private Transform[] _spawnPoints;
    private Dictionary<Transform, bool> _spawnPointsOccupationDict; //will keep an eye on which spawnpoint is empty or not and available to have a new NPC (true if occupied, false otherwise)

    [Header("NPC Elements")]
    public int MaxHospitalCapacity=20;
    public int CurNPCNumber;
    [SerializeField] private int _initialSpawnNPCNumber;
    [SerializeField] private GameObject[] _availableNPC;
    #endregion

    private void OnValidate()
    {
        Assert.IsNotNull(_availableNPC);
        Assert.IsNotNull(_availableNPC);
        if (_initialSpawnNPCNumber > MaxHospitalCapacity)
        {
            Debug.LogError("Spawn Manager : inputted more initial npcs than max hospital capacity");
        }
    }

    public void Start()
    {
        _spawnPointsOccupationDict = new Dictionary<Transform, bool>();
        foreach(Transform spawnpoint in _spawnPoints)
        {
            _spawnPointsOccupationDict.Add(spawnpoint, false);
        }

        CurNPCNumber = 0;
        InitialNPCSpawn();
        NPCEvents.Instance.Event.AddListener(OnEventReceived);
    }

    private void OnDestroy()
    {
        if(NPCEvents.Instance != null)
        {
            NPCEvents.Instance.Event.RemoveListener(OnEventReceived);
        }
    }

    /// <summary>
    /// This function will handle the initial spawn of the NPCs
    /// </summary>
    private void InitialNPCSpawn()
    {
        for(int i = 0;i<_initialSpawnNPCNumber;++i)
        {
            SpawnNewNPC();
        }
        //At the end of the function :
        StartCoroutine(SpawnTimer());
    }


    /// <summary>
    /// Main coroutine that will handle the spawn of an npc
    /// </summary>
    private IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(_spawnTimer);
        if(CurNPCNumber == MaxHospitalCapacity)
        {
            //Debug.Log("Spawn Manager : Stopping NPC spawn coroutine due to max capacity reached");
            yield return null;
        }
        else
        {
            //Debug.Log("Spawn Manager : Attempting to spawn a new npc");
            if (AttemptNPCSpawn())
            {
                SpawnNewNPC();
            }
            StartCoroutine(SpawnTimer());
        }
    }

    /// <summary>
    /// Function that will return wether or not rng-based a npc can spawn (regardless of limit)
    /// </summary>
    private bool AttemptNPCSpawn()
    {
        int rng = UnityEngine.Random.Range(0, 100);
        if (rng < _spawnChance)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Function that will effectively spawn a NPC
    /// </summary>
    private void SpawnNewNPC()
    {
        Debug.Log("Spawn Manager : Spawning new NPC");
        //selecting a valid spawn point :
        List<Transform> availableSpawnPoints = new List<Transform>();
        foreach(var spawnpoint in _spawnPointsOccupationDict)
        {
            if (!spawnpoint.Value)
            {
                availableSpawnPoints.Add(spawnpoint.Key);
            }
        }
        if(availableSpawnPoints.Count == 0)
        {
            return;
        }
        Transform selectedSpawnPoint = SelectSpawnPoint(availableSpawnPoints);
        //selecting npc to spawn :
        GameObject newNpc = Instantiate(SelectNPC());
        newNpc.transform.position = selectedSpawnPoint.transform.position;
        newNpc.GetComponent<NPCBehaviourController>().SpawnPoint = selectedSpawnPoint;
        if (UnityEngine.Random.Range(0, 100) <= _movementChance)
        {
            newNpc.GetComponent<NPCBehaviourController>().MakeMovingNPC();
        }
        //updating dict values :
        CurNPCNumber++;
        _spawnPointsOccupationDict[selectedSpawnPoint] = true;
    }

    private Transform SelectSpawnPoint(List<Transform> availableSpawnPoint)
    {
        return availableSpawnPoint[UnityEngine.Random.Range(0, availableSpawnPoint.Count)];
    }

    private GameObject SelectNPC()
    {
        return _availableNPC[UnityEngine.Random.Range(0, _availableNPC.Length)];
    }

    #region DEATH OF NPC FUNCTIONS
    private void OnEventReceived(NPCGameEventArg arg)
    {
        switch (arg.Type)
        {
            case NPCGameEventType.Death:
                OnNPCDeath(arg);
                break;
        }
    }

    private void OnNPCDeath(NPCGameEventArg arg)
    {
        //Debug.Log("Spawn Manager : NPC is dead, updating curNPCNumber values");
        CurNPCNumber--;
        _spawnPointsOccupationDict[arg.Npc.GetComponent<NPCBehaviourController>().SpawnPoint] = false;
        StopAllCoroutines();
        StartCoroutine(SpawnTimer());
    }
    #endregion
}
