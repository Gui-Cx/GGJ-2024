using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pie : MonoBehaviour
{

    Rigidbody2D rd;

    private void Start()
    {
        rd = GetComponent<Rigidbody2D>();
    }
    private void sendMessageToNPCHit(Collision2D npcHit)
    {
        NPCItemHandler npcItemHandler;
        if (npcHit.gameObject.TryGetComponent<NPCItemHandler>(out npcItemHandler))
        {
            npcItemHandler.OnItemTriggered(ITEM_TYPE.Pie);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        sendMessageToNPCHit(collision);
    }
}
