using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDebug : MonoBehaviour
{
    public ITEM_TYPE Type;
    public bool ActivateItem;

    private void Update()
    {
        if (ActivateItem)
        {
            ActivateItem = false;
            GetComponent<NPCItemHandler>().OnItemTriggered(Type);
        }
    }
}
