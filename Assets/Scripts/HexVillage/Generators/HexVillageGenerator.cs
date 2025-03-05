using Sirenix.OdinInspector;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

namespace HexVillage.Generators
{
    [RequireComponent(typeof(HexGridGenerator))]
    [ExecuteAlways]
    public class HexVillageGenerator : MonoBehaviour
    {
        [SerializeField] private HexVillageSettings settings;
        public HexGridGenerator GridGenerator { get; private set; }

        private Transform _villageParent;

        private void OnValidate()
        {
            EnsureDependencies();
        }

        private void Awake()
        {
            EnsureDependencies();
        }

        private void EnsureDependencies()
        {
            _villageParent = transform;
            GridGenerator = GetComponent<HexGridGenerator>();
            GridGenerator.Initialise(settings);
        }

        [Button("Generate Village")]
        public void GenerateVillage()
        {
            ClearVillage();
            //1.Generate the Grid
            GridGenerator.GenerateGrid();
            
            //2. Spawn the buildings
            SpawnBuildings();
        }

        private void SpawnBuildings()
        {
            System.Random rng = new System.Random(settings.RandomSeed + 1283);
            for (int x = 0; x < settings.GridWidth; x++)
            {
                for (int y = 0; y < settings.GridHeight; y++)
                {
                    if (rng.NextDouble() < settings.BuildingChance)
                    {
                        Vector2Int gridPosition = new Vector2Int(x, y);
                        
                        //We need to check the tile type at this current position
                        string tileTypeAtPos = GetTileTypeAtPos(gridPosition);
                        if (tileTypeAtPos == "grass" || tileTypeAtPos == "dirt")
                        {
                            //Get a random building
                            GameObject building = settings.Buildings[rng.Next(0, settings.Buildings.Count)];
                            Vector3 worldPos = GridGenerator.CalculateHexPosition(x, y, settings.TileSize);
                            GameObject buildingInstance = Instantiate(building, worldPos, Quaternion.identity, _villageParent);
                            buildingInstance.name = $"Building_{x}_{y}";
                        }
                    }
                }
            }
        }
        
        private string GetTileTypeAtPos(Vector2Int tilePosition)
        {
            if (!GridGenerator.HexTiles.ContainsKey(tilePosition)) return "";
            
            string tileName = GridGenerator.HexTiles[tilePosition].name;
            var match = Regex.Match(tileName, @"HexTile_\d+_\d+_(\w+)");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
        
        [Button("Clear Village")]
        public void ClearVillage()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
    }
}