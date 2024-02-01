using System.Collections.Generic;
using UnityEngine;

public class AOESadness : MonoBehaviour
{
    private static readonly float RADIUS_TO_SCREEN = 0.05f;

    [SerializeField] private SpriteRenderer _circleSprite;

    private ParticleSystem _particleSystem;
    private Material _grayMaterial;

    private float _minSadnessRadius;
    private float _maxSadnessRadius;

    private float _minEmissionRate;
    private float _maxEmissionRate;

    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _grayMaterial = _circleSprite.material;
    }

    public void SetValues(float minRadius, float maxRadius, float minEmission, float maxEmission)
    {
        _minSadnessRadius = minRadius;
        _maxSadnessRadius = maxRadius;
        _minEmissionRate = minEmission;
        _maxEmissionRate = maxEmission;

        RemoveSadness();
    }

    public void RemoveSadness()
    {
        SetEmission(0);
        GrayscaleEffect(0);
    }

    /// <summary>
    /// Sets the sadness rate. Depends on the current happiness value and the threshold value
    /// at which the NPC starts being sad.
    /// </summary>
    /// <param name="happiness">Current NPC happiness value</param>
    /// <param name="thresholdValue">Value under which the NPC is considered sad</param>
    public void SetSadness(int happiness, int thresholdValue)
    {
        float sadnessRate = Mathf.Clamp01((float)(thresholdValue - happiness) / thresholdValue);

        SetEmission(sadnessRate);
        GrayscaleEffect(sadnessRate);
    }

    private void GrayscaleEffect(float sadnessRate)
    {
        float sadnessRadius = sadnessRate == 0 ? 0 : Mathf.Lerp(_minSadnessRadius, _maxSadnessRadius, sadnessRate);
        _grayMaterial.SetFloat("_Radius", sadnessRadius * RADIUS_TO_SCREEN);
    }

    private void SetEmission(float sadnessRate)
    {
        if (sadnessRate == 0)
        {
            _particleSystem.Stop();
            return;
        }
        else _particleSystem.Play();

        float emissionRate = _minEmissionRate + _maxEmissionRate * sadnessRate;

        ParticleSystem.EmissionModule emissionModule = _particleSystem.emission;
        emissionModule.rateOverTime = emissionRate;
    }
}