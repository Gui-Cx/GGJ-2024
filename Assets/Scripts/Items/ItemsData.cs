using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public enum ITEM_TYPE
{
    RedNose,
    Flower,
    Ballon,
    Pie,
    Hug,
    Trumpet,
    Gun
}

public enum USE_TYPE
{
    Circle,
    Ray,
    The_Pie
}

[Serializable]
public struct ItemDataElement
{
    public string Name;
    public Sprite Symbol;
    public ITEM_TYPE Type;
    public USE_TYPE UseType; 
    public float UseRange;
    public Vector2 UseOffset;
}

/// <summary>
/// Main scriptable object that will contain all the necessary data of the items as to avoid data dispersion
/// </summary>
[CreateAssetMenu(fileName ="ItemsData",menuName ="Scriptable Object/Items Data")]
public class ItemsData : ScriptableObject
{
    [field: SerializeField] public ItemDataElement[] ItemDataElements { get; private set; }
}
