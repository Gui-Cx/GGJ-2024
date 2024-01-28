using UnityEngine;
using UnityEngine.Tilemaps;

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

    private Grid _grid;
    private Tilemap _backgroundTilemap;
    private Tilemap _grayscaleTilemap;
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

    public void SetGridInfo(Grid grid, Tilemap backgroundTilemap, Tilemap grayscaleTilemap)
    {
        _grid = grid;
        _backgroundTilemap = backgroundTilemap;
        _grayscaleTilemap = grayscaleTilemap;
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
        GrayscaleEffect();
    }

    private void GrayscaleEffect()
    {
        _grayscaleTilemap.ClearAllTiles();

        float sadnessRadius = _currentSadnessRate == 0 ? 0 : _minSadnessRadius + _maxSadnessRadius * _currentSadnessRate;

        Vector3Int cellPosition = _grid.WorldToCell(transform.position);
        TileBase tile;

        int xMin = cellPosition.x - Mathf.RoundToInt(sadnessRadius);
        int xMax = cellPosition.x + Mathf.RoundToInt(sadnessRadius);
        int yMin = cellPosition.y - Mathf.RoundToInt(sadnessRadius);
        int yMax = cellPosition.y + Mathf.RoundToInt(sadnessRadius);

        for(int tileX = xMin; tileX < xMax; tileX++)
        {
            for (int tileY = yMin; tileY < yMax; tileY++)
            {
                Vector3Int tilePosition = new Vector3Int(tileX, tileY);

                tile = _backgroundTilemap.GetTile(tilePosition);
                _grayscaleTilemap.SetTile(tilePosition, tile);
            }
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
