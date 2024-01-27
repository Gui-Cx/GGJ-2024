using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class AOESadness : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _sadnessMaterial;

    [Header("Sadness Radius")]
    [SerializeField] private float _minSadnessRadius;
    [SerializeField] private float _maxSadnessRadius;

    [Header("Particle emission")]
    [SerializeField] private float _minEmissionRate;
    [SerializeField] private float _maxEmissionRate;

    private ParticleSystem _particleSystem;

    //Value between 0 and 1, corresponds to the percentage of sadness
    private float _currentSadnessRate;

    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        SetSadness(5, 10);
    }

    /// <summary>
    /// Sets the sadness rate. Depends on the current happyness value and the threshold value
    /// at which the NPC starts being sad.
    /// </summary>
    /// <param name="happyness">Current NPC happyness value</param>
    /// <param name="thresholdValue">Value under which the NPC is considered sad</param>
    public void SetSadness(int happyness, int thresholdValue)
    {
        _currentSadnessRate = Mathf.Clamp01((float)(thresholdValue - happyness) / thresholdValue);

        SetEmission();
        SetSadnessRadius();
    }

    private void SetSadnessRadius()
    {
        if (_currentSadnessRate == 0) return;

        float sadnessRadius = _minSadnessRadius + _maxSadnessRadius * _currentSadnessRate;

        Collider[] colliders = Physics.OverlapSphere(transform.position, sadnessRadius);

        foreach(Collider collider in colliders)
        {
            Renderer renderer = collider.gameObject.GetComponentInChildren<Renderer>();
            if (renderer != null) renderer.material = _sadnessMaterial;
        }
    }

    private void SetEmission()
    {
        if (_currentSadnessRate == 0)
        {
            _particleSystem.Stop();
            return;
        }
        else _particleSystem.Play();

        float emissionRate = _minEmissionRate + _maxEmissionRate * _currentSadnessRate;
        
        ParticleSystem.EmissionModule emissionModule = _particleSystem.emission;
        emissionModule.rateOverTime = emissionRate;
    }
}
