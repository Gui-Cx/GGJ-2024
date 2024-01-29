using System;
using UnityEngine;

public enum ITEM_TYPE
{
    RedNose,
    Flower,
    Ballon_Dog,
    Ballon_Heart,
    Ballon_Cringe,
    Pie,
    Hug,
    Trumpet,
    Gun,
    None
}

public enum USE_TYPE
{
    Circle,
    Ray,
    The_Pie
}


[Serializable]
public struct ItemData
{
    public string Name;
    public Sprite Symbol;
    public ITEM_TYPE Type;
    public USE_TYPE UseType; 
    public float UseRange;
    public Vector2 UseOffset;
    public GameObject Particles;
}

/// <summary>
/// Main scriptable object that will contain all the necessary data of the items as to avoid data dispersion
/// </summary>
[CreateAssetMenu(fileName ="ItemsData",menuName ="Scriptable Object/Items Data")]
public class ItemsData : ScriptableObject
{
    [field: SerializeField] public ItemData[] Items { get; private set; }
}
