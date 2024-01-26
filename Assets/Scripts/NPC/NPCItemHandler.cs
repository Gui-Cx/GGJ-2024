using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that will handle all the reactions from the various items the player activates/throws at the NPC.
/// This script should only act as a receiver of calls, and should NOT handle the detections of said calls
/// </summary>
[RequireComponent(typeof(NPCBehaviourController))]
public class NPCItemHandler : MonoBehaviour
{
    private NPCBehaviourController _npcBehaviourController;

    private void Awake()
    {
        _npcBehaviourController = GetComponent<NPCBehaviourController>();
    }

    public void OnItemTriggered(ITEM_TYPE itemType) => _npcBehaviourController.OnItemTriggered(itemType);

}
