using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Pie : MonoBehaviour
{
    Rigidbody2D rd;
    public float speed;
    public bool goRight;

    private void Start()
    {
        rd=GetComponent<Rigidbody2D>();
        rd.velocity= speed *((goRight?1:-1) * new Vector2(1f,0) + new Vector2(0, 1f));
        Destroy(this.gameObject, 5);
    }
    private void sendMessageToNPCHit(Collision2D npcHit)
    {
        NPCItemHandler npcItemHandler;
        if (npcHit.gameObject.TryGetComponent<NPCItemHandler>(out npcItemHandler))
        {
            npcItemHandler.OnItemTriggered(ITEM_TYPE.Pie);
            Destroy(this.gameObject);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        sendMessageToNPCHit(collision); 
    }

    private void Update()
    {
        
        if (rd.velocity.x != 0 && rd.velocity.magnitude>2)
        {
            float angle = Mathf.Rad2Deg * Mathf.Atan(rd.velocity.y / rd.velocity.x)+ (goRight ? 1 : -1) * -90;
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
          
    }
}
