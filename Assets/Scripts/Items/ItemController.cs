using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class ItemController : MonoBehaviour
{

    [SerializeField] LayerMask _npcMask;

    private ContactFilter2D contactFilter2D;

    private ItemDataElement currentItem;

    [Header("Throw Pie")]
    [SerializeField] private Slider throwBar;
    [SerializeField] GameObject pie;
    [SerializeField] int reloadPieTime=1;

    [Header("Parameters")]
    [SerializeField] float throwStrength;
    [SerializeField] float maxPressTime;
    [SerializeField] float minPressTime;
    [SerializeField] Gradient sliderColor;

    [Header("gizmo")]
    Vector2 gizmosoffset;
    float gizmosradius;
    Vector2 gizmosfrom;
    Vector2 gizmosto;

    private Player player;
    bool displayTime;
    bool canThrowPie = true;

    private void Awake()
    {
        throwBar.value = 0;
    }
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
                    offset = positionVec2 + (player.IsFacingRight ? 1 : -1) * currentItem.UseOffset;
                    castCircle(offset, currentItem.UseRange);
                    gizmosoffset = offset;
                    gizmosradius = currentItem.UseRange;

                }
            break;
            case USE_TYPE.Ray:
                if (time == 0f)
                {
                    offset = positionVec2 + (player.IsFacingRight ? 1 : -1) * currentItem.UseOffset;
                    castRay(offset, (player.IsFacingRight ? Vector2.right : Vector2.left), currentItem.UseRange);
                    Debug.LogFormat("Item type {0} triggered Ray", currentItem.Type);
                    gizmosfrom = offset;
                    gizmosto = offset + (player.IsFacingRight ? Vector2.right : Vector2.left) * currentItem.UseRange;
                }

            break;
            case USE_TYPE.The_Pie:
                Debug.LogFormat("Item type {0} triggered the Piiiie", currentItem.Type);
                //wait for release
                if(canThrowPie)ThrowPie(time);
            break;
        }
    }

    public void GetTimeHold(float time)
    {
        if (currentItem.UseType == USE_TYPE.The_Pie)
        {
            throwBar.value = (time - minPressTime) / maxPressTime;
            throwBar.fillRect.GetComponent<UnityEngine.UI.Image>().color = sliderColor.Evaluate(throwBar.value);
        }
    }
    private void ThrowPie(float time)
    {
        if (time < minPressTime) { return; }
        float speedThrow = throwStrength * (Mathf.Clamp(time, 0, maxPressTime) - minPressTime) / (maxPressTime - minPressTime);

        if (GetComponent<Player>().IsFacingRight)
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
        GetTimeHold(0);

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


    private void OnDrawGizmos()
    {
        if(currentItem.UseType == USE_TYPE.Circle)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(gizmosoffset, gizmosradius);
        }
       
        if (currentItem.UseType == USE_TYPE.Ray)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(gizmosfrom,gizmosto);
        }
    }
}
