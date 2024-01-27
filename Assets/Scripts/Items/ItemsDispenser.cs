using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsDispenser : MonoBehaviour,IInteractable
{
    [SerializeField] ITEM_TYPE typeItem;
    public bool Interact(Interactor interactor)
    {
        interactor.gameObject.GetComponent<Player>().SetCurrentItem(typeItem);
        return true;
    }
}
