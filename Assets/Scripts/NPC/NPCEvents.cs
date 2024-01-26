using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum NPCGameEventType
{
    Death
}

/// <summary>
/// Class that will handle the events of the NPC
/// </summary>
public class NPCEvents : MonoBehaviour
{
    /// <summary>
    /// Basis for the game events
    /// </summary>
    public class NPCGameEvent : UnityEvent<NPCGameEventType>
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
        Event = new NPCGameEvent();
    }
    #endregion

    public NPCGameEvent Event;
}
