using UnityEngine;

public enum NPC_TYPES
{
    Old,
    Child,
    Depressed,
    Wheelchair,
    SadClown,
    Death
}

[CreateAssetMenu(menuName = "Scriptable Object/NPC Type", fileName = "NPCTypeData")]
public class NPCTypeData : ScriptableObject
{
    public NPC_TYPES Type;
    public NPCBehaviourData[] BehaviourDatas;
}
