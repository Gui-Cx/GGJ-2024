using System.Collections.Generic;
using Unity.VisualScripting;
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
    float lastRadius = 0f;
    List<Vector3Int> greyTiles;



    private Grid _grid;
    private Tilemap _backgroundTilemap;
    private Tilemap _grayscaleTilemap;
    private ParticleSystem _particleSystem;

    //Value between 0 and 1, corresponds to the percentage of sadness
    private float _currentSadnessRate;
    //Position of the cell on the gridmap at which the pnc is located
    private Vector3Int _cellPosition;

    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        greyTiles = new List<Vector3Int>();
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
        //GrayscaleEffect();
    }

    void ClearTiles(List<Vector3Int> lastGreyTiles)
    {
        
            foreach (Vector3Int tilepos in lastGreyTiles)
            {
                _grayscaleTilemap.SetTile(tilepos, null);
            };
    }
    private void GrayscaleEffect()
    {
        float sadnessRadius = 2;
        //if (_currentSadnessRate != 0) sadnessRadius = Mathf.Lerp(_minSadnessRadius, _maxSadnessRadius, _currentSadnessRate);
        //print(sadnessRadius);
        Vector3Int newCellPosition = _grid.WorldToCell(transform.position);

        List<Vector3Int> lastGreyTiles = new List<Vector3Int>();
        foreach (var tile in greyTiles)
        {
            lastGreyTiles.Add(tile);
        }
        greyTiles = new List<Vector3Int>();

        if (newCellPosition != _cellPosition ||lastRadius!=sadnessRadius)
        {

            _cellPosition = newCellPosition;

            int xMin = _cellPosition.x - Mathf.RoundToInt(sadnessRadius);
            int xMax = _cellPosition.x + Mathf.RoundToInt(sadnessRadius);
            int yMin = _cellPosition.y - Mathf.RoundToInt(sadnessRadius);
            int yMax = _cellPosition.y + Mathf.RoundToInt(sadnessRadius);

            TileBase tile;
            for (int tileX = xMin; tileX < xMax; tileX++)
            {
                for (int tileY = yMin; tileY < yMax; tileY++)
                {
                    Vector3Int tilePosition = new Vector3Int(tileX, tileY);
                    lastGreyTiles.Remove(tilePosition);
                    tile = _backgroundTilemap.GetTile(tilePosition);
                    _grayscaleTilemap.SetTile(tilePosition, tile);
                    greyTiles.Add(tilePosition);   
                }
            }
            lastRadius = sadnessRadius;
            ClearTiles(lastGreyTiles);
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