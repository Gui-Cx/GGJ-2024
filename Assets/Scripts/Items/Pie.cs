using System.Collections;
using System.Collections.Generic;
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
        rd.velocity= speed *((goRight?1:-1) * new Vector3(1f,0, 0) + new Vector3(0, 1f, 0));
        print("Pie velocity" + rd.velocity);
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

}
