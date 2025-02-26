using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using HexVillage.Utilities;

[Serializable]
public class HexVillageSettings
{
    [FoldoutGroup("Grid Settings")]
    [Tooltip("Number of tiles in the X-Direction")]
    [SerializeField]
    private int gridWidth = 20;
    
    [FoldoutGroup("Grid Settings")]
    [Tooltip("Number of tiles in the Y-Direction")]
    [SerializeField]
    private int gridHeight = 20;
    
    [FoldoutGroup("Grid Settings")]
    [Tooltip("The size of each hex tile")]
    [SerializeField]
    private float tileSize = 0.58f;

    [FoldoutGroup("Grid Settings")] 
    [Tooltip("Seed for the rng")]
    [SerializeField]
    private int randomSeed = 123;

    [FoldoutGroup("Tile Settings")] 
    [Tooltip("The tile database that contains all models")]
    [Required]
    [SerializeField]
    private TileDatabaseSO tileDatabase;
    
    [Tooltip("The probability of replacing a terrain tile with a building")]
    [Range(0f, 1f)]
    [SerializeField]
    private float buildingChance = 0.1f;

    public float TileSize => tileSize;

    public float BuildingChance => buildingChance;

    public int GridWidth => gridWidth;

    public int GridHeight => gridHeight;

    public int RandomSeed => randomSeed;

    public List<GameObject> Terrain
    {
        get => tileDatabase != null ? tileDatabase.TerrainTiles : new List<GameObject>();
    }
    public List<GameObject> Buildings
    {
        get => tileDatabase != null ? tileDatabase.BuildingTiles : new List<GameObject>();
    }
    public List<GameObject> Roads
    {
        get => tileDatabase != null ? tileDatabase.PathTiles : new List<GameObject>();
    }
    public List<GameObject> Rivers
    {
        get => tileDatabase != null ? tileDatabase.RiverTiles : new List<GameObject>();
    }
    public List<GameObject> Units
    {
        get => tileDatabase != null ? tileDatabase.UnitTiles : new List<GameObject>();
    }
}
