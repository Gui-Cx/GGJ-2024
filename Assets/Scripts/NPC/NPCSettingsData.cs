using UnityEngine;

/// <summary>
/// This script contains the data that each archetype of NPCs will have
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Object/NPC Settings", fileName = "NPCSettingsData")]
public class NPCSettingsData : ScriptableObject
{
    [Header("Happiness Parameters")]
    public int MaxHappiness = 20;
    [Tooltip("Happiness level under which the NPC will start to emit sadness")]
    public int HappinessThreshold = 10;

    [Header("Time Parameters")]
    [Tooltip("Time between NPC happiness update")]
    public float HappinessUpdateDelay = 1f;
    [Tooltip("Time during which the NPC's happiness bar will not go down")]
    public float HappyTime = 5f;

    [Header("Sadness Radius")]
    public float MinSadnessRadius;
    public float MaxSadnessRadius;

    [Header("Particle emission")]
    public float MinEmissionRate;
    public float MaxEmissionRate;

    [Header("Score Parameters")]
    public int ScoreIncrease = 20;
    public int ScoreDecrease = 1;
}
