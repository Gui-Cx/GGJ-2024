using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum NPCGameEventType
{
    Death
}

public struct NPCGameEventArg
{
    public GameObject Npc;
    public NPCGameEventType Type;
}

/// <summary>
/// Class that will handle the events of the NPC
/// </summary>
public class NPCEvents : MonoBehaviour
{
    /// <summary>
    /// Basis for the game events
    /// </summary>
    public class NpcGameEvent : UnityEvent<NPCGameEventArg>
    {
    }

    #region SINGLETON DESIGN PATTERN
    private static NPCEvents _instance;
    public static NPCEvents Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new NPCEvents();
            }
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
        Event = new NpcGameEvent();
    }
    #endregion

    public NpcGameEvent Event;
}
