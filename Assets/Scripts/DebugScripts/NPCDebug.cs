using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDebug : MonoBehaviour
{
    public ITEM_TYPE Type;
    public bool ActivateItem;
    public bool SwitchData;

    private void Update()
    {
        if (ActivateItem)
        {
            ActivateItem = false;
            GetComponent<NPCItemHandler>().OnItemTriggered(Type);
        }
        if(SwitchData)
        {
            SwitchData = false;
            GetComponent<NPCBehaviourController>().SwitchNPCData();
        }
    }
}
