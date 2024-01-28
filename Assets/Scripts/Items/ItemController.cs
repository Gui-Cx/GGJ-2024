using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemController : MonoBehaviour
{

    [SerializeField] LayerMask _npcMask;

    private ContactFilter2D contactFilter2D;

    private ItemDataElement currentItem;

    [SerializeField] GameObject pie;
    private Player player;
    
    int reloadPieTime=1;
    bool canThrowPie = true;
    void Start()
    {
        player = GetComponent<Player>();
        contactFilter2D.SetLayerMask(_npcMask);
    }
    public void OnItemUsed(ITEM_TYPE type,float time=0f){
        currentItem = GameManager.Instance.itemsData.ItemDataElements.First(item => item.Type == type);
        Vector2 positionVec2 = new Vector2(transform.position.x, transform.position.y);
        Vector2 offset;
        switch (currentItem.UseType)
        {
            case USE_TYPE.Circle:  
                if (time == 0f)
                {
                    Debug.LogFormat("Item type {0} triggered Circle", currentItem.Type);
                    offset = positionVec2 + (player.isFacingRight ? 1 : -1) * currentItem.UseOffset;
                    castCircle(offset, currentItem.UseRange);
                }
            break;
            case USE_TYPE.Ray:
                if (time == 0f)
                {
                    offset = positionVec2 + (player.isFacingRight ? 1 : -1) * currentItem.UseOffset;
                    castRay(offset, (player.isFacingRight ? Vector2.right : Vector2.left), currentItem.UseRange);
                    Debug.LogFormat("Item type {0} triggered Ray", currentItem.Type);
                }

            break;
            case USE_TYPE.The_Pie:
                Debug.LogFormat("Item type {0} triggered the Piiiie", currentItem.Type);
                //wait for release
                if(canThrowPie)ThrowPie(time);
            break;
        }
    }

    private void ThrowPie(float time)
    {
        if (time < 1f) { return; }
        float speedThrow = Mathf.Min(Mathf.Max(time * 3f, 2f),10f);
        if (GetComponent<Player>().isFacingRight)
        {
            Pie currentPie = Instantiate(pie, transform.position+new Vector3(1,0,0), Quaternion.identity).GetComponent<Pie>();
            currentPie.speed = speedThrow;
            currentPie.goRight = true;
        }
        else
        {
            Pie currentPie = Instantiate(pie, transform.position + new Vector3(-1, 0, 0), Quaternion.identity).GetComponent<Pie>();
            currentPie.speed = speedThrow;
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
            return;
        }
        //if we arrive here, then it means that the cast has failed : no one was overlapped
        if(currentItem.Type == ITEM_TYPE.Ballon_Dog)
        {
            GetComponent<Player>().SetCurrentItem(ITEM_TYPE.Ballon_Heart);
            currentItem = GameManager.Instance.itemsData.ItemDataElements.First(item => item.Type == ITEM_TYPE.Ballon_Heart);
        }
        else if(currentItem.Type == ITEM_TYPE.Ballon_Heart)
        {
            GetComponent<Player>().SetCurrentItem(ITEM_TYPE.Ballon_Cringe);
            currentItem = GameManager.Instance.itemsData.ItemDataElements.First(item => item.Type == ITEM_TYPE.Ballon_Cringe);
        } 
        else if(currentItem.Type == ITEM_TYPE.Ballon_Cringe)
        {
            GetComponent<Player>().SetCurrentItem(ITEM_TYPE.Ballon_Dog);
            currentItem = GameManager.Instance.itemsData.ItemDataElements.First(item => item.Type == ITEM_TYPE.Ballon_Dog);
        }
    }

    private void castRay(Vector2 start, Vector2 direction, float range)
    {
        List<RaycastHit2D> npcHit = new List<RaycastHit2D>();
        if (Physics2D.Raycast(start, direction, contactFilter2D, npcHit, range) > 0)
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
