using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Script that will handle the symbol that is to be displayed over the NPC
/// </summary>
public class NPCSymbolController : MonoBehaviour
{ 
    [SerializeField] private GameObject _symbol;

    private SymbolModule _symbolModule;

    private void OnValidate()
    {
        Assert.IsNotNull(_symbol);
    }

    private void Awake()
    {
        _symbolModule = _symbol.GetComponent<SymbolModule>();
    }

    /// <summary>
    /// Function called by NPCBehaviourController that will switch the symbol over the NPC head
    /// </summary>
    public void UpdateSymbolItem(ITEM_TYPE type)
    {
        _symbolModule.DisplaySymbol(type);
    }

    /// <summary>
    /// Hide the symbol during the happiness timer, called by NPCBehaviourController
    /// </summary>
    public void HideSymbol()
    {
        _symbol.SetActive(false);
    }

    public void DisplaySymbol()
    {
        _symbol.SetActive(true);
    }
}
