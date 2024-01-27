using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class SadnessParticles : MonoBehaviour
{ 
    [SerializeField] private float _minEmissionRate;
    [SerializeField] private float _maxEmissionRate;

    private ParticleSystem _particleSystem;

    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Sets the emission rate for sadness particles. Depends on the current happyness value and the threshold value
    /// at which the NPC starts being sad.
    /// </summary>
    /// <param name="happyness">Current NPC happyness value</param>
    /// <param name="thresholdValue">Value under which the NPC is considered sad</param>
    public void SetEmission(int happyness, int thresholdValue)
    {
        float emissionRate = _minEmissionRate + _maxEmissionRate * (thresholdValue - happyness)/ thresholdValue;
        
        ParticleSystem.EmissionModule emissionModule = _particleSystem.emission;
        emissionModule.rateOverTime = emissionRate;
    }
}
