using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pie : MonoBehaviour
{
    public float radius=50f;
    LayerMask _npcMask;
    private ContactFilter2D contactFilter2D;

    void Start()
    {
        contactFilter2D.SetLayerMask(_npcMask);
    }
    private void Update()
    {
        List<Collider2D> npcColliders = new List<Collider2D>();
        if (Physics2D.OverlapCircle(transform.position, 500f, contactFilter2D, npcColliders) > 0)
        {
            foreach (Collider2D npcColIterator in npcColliders)
            {
                sendMessageToNPCHit(npcColIterator);
            }
        }
    }

    private void sendMessageToNPCHit(Collider2D npcHit)
    {
        NPCItemHandler npcItemHandler;
        if (npcHit.TryGetComponent<NPCItemHandler>(out npcItemHandler))
        {
            npcItemHandler.OnItemTriggered(ITEM_TYPE.Pie);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        sendMessageToNPCHit(collision);
    }
}
