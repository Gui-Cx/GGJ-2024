using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemController : MonoBehaviour
{

    ItemDataElement currentItem;

    [SerializeField] LayerMask _npcMask;

    private ContactFilter2D contactFilter2D;

    void Awake()
    {
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(_npcMask);
    }
    public void OnItemUsed(){
        Vector2 positionVec2 = new Vector2(transform.position.x, transform.position.y);
        switch (currentItem.UseType){
            case USE_TYPE.Circle:
                Debug.LogFormat("Item type {0} triggered Circle", currentItem.Type);
                castCircle(positionVec2+currentItem.UseOffset, currentItem.UseRange);
            break;
            case USE_TYPE.Ray:
                Debug.LogFormat("Item type {0} triggered Ray", currentItem.Type);
                castRay(currentItem.UseRange);
            break;
            case USE_TYPE.The_Pie:
                Debug.LogFormat("Item type {0} triggered the Piiiie", currentItem.Type);
            break;
        }
    }
    
    private void castCircle(Vector2 position, float radius)
    {
        List<Collider2D> npcColliders = new List<Collider2D>();
        if (Physics2D.OverlapCircle(position, radius, contactFilter2D, npcColliders) > 0)
        {
            foreach(Collider2D npcColIterator in npcColliders)
            {
                sendMessageToNPCHit(npcColIterator);
            }
        }
    }

    private void castRay(float range)
    {
        List<RaycastHit2D> npcHit = new List<RaycastHit2D>();
        if (Physics2D.Raycast(transform.position, transform.forward, contactFilter2D, npcHit, range) > 0)
        {
            foreach(RaycastHit2D npcHitIterator in npcHit)
            {
                sendMessageToNPCHit(npcHitIterator.collider);
            }
        }
    }

    private void sendMessageToNPCHit(Collider2D npcHit)
    {
        NPCBehaviourController npcBehaviourController;
        if (npcHit.TryGetComponent<NPCBehaviourController>(out npcBehaviourController))
        {
            npcBehaviourController.OnItemTriggered(currentItem.Type);
        }
    }
}
