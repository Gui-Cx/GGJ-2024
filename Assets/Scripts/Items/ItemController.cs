using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemController : MonoBehaviour
{

    [SerializeField] LayerMask _npcMask;

    private ContactFilter2D contactFilter2D;

    private ItemDataElement currentItem;

    [SerializeField] GameObject pie;
    int reloadPieTime=1;
    bool canThrowPie = true;
    void Start()
    {
        contactFilter2D.SetLayerMask(_npcMask);
    }
    public void OnItemUsed(ITEM_TYPE type){
        currentItem = GameManager.Instance.itemsData.ItemDataElements.First(item => item.Type == type);
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
                if(canThrowPie)ThrowPie();
            break;
        }
    }

    private void ThrowPie()
    {

        if (GetComponent<Player>().isFacingRight)
        {
            Pie currentPie = Instantiate(pie, transform.position+new Vector3(1,0,0), Quaternion.identity).GetComponent<Pie>();
            currentPie.speed = 5f;
            currentPie.goRight = true;
        }
        else
        {
            Pie currentPie = Instantiate(pie, transform.position + new Vector3(-1, 0, 0), Quaternion.identity).GetComponent<Pie>();
            currentPie.speed = 5f;
            currentPie.goRight = false;
        }
        canThrowPie = false;
        StartCoroutine(CountdownPie(reloadPieTime));

    }


    IEnumerator CountdownPie(int microseconds)
    {
        int counter = microseconds;
        while (counter > 0)
        {
            yield return new WaitForSeconds(0.1f);
            counter--;
        }
        canThrowPie = true;
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
        NPCItemHandler npcItemHandler;
        if (npcHit.TryGetComponent<NPCItemHandler>(out npcItemHandler))
        {
            npcItemHandler.OnItemTriggered(currentItem.Type);
        }
    }
}
