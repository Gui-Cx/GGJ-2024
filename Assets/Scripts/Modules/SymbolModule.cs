using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Script used by the symbol elements over the NPC heads/over other elements
/// </summary>
public class SymbolModule : MonoBehaviour
{
    [SerializeField] private ItemsData _itemData;
    [SerializeField] private Image _symbolImage;

    private Dictionary<ITEM_TYPE, Sprite> _symbolSprites;

    private void OnValidate()
    {
        Assert.IsNotNull(_symbolImage);    
        Assert.IsNotNull(_itemData);
    }

    private void Awake()
    {
        _symbolSprites = new Dictionary<ITEM_TYPE, Sprite>();
        foreach(var item in _itemData.ItemDataElements)
        {
            _symbolSprites.Add(item.Type, item.Symbol);
        }
    }

    public void DisplaySymbol(ITEM_TYPE type)
    {
        _symbolImage.sprite = _symbolSprites[type];
    }
}
