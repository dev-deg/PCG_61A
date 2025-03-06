using Sirenix.OdinInspector;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting.Dependencies.NCalc;

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
            
            //2. Fill the water holes
            FillWaterHoles();
            
            //3. Spawn the buildings
            SpawnBuildings();
        }

        private void FillWaterHoles()
        {
            System.Random rng = new System.Random(settings.RandomSeed + 1283);
            for (int x = 0; x < settings.GridWidth; x++)
            {
                for (int y = 0; y < settings.GridHeight; y++)
                {
                    Vector2Int gridPosition = new Vector2Int(x, y);
                    string tileTypeAtPos = GetTileTypeAtPos(gridPosition);
                    //if the tile is a water and all adjacent tiles are land tiles
                    if (tileTypeAtPos.Contains("water") && AreAllAdjacentTilesLand(gridPosition))
                    {
                        //fill the hole by spawning a building
                        SpawnBuildingAtPosition(gridPosition, x, y, rng);   
                    }
                }
            }
        }
        
        private void SpawnBuildingAtPosition(Vector2Int gridPosition, int x, int y, System.Random rng)
        {
            GameObject building = settings.Buildings[rng.Next(0, settings.Buildings.Count)];
            Vector3 worldPos = GridGenerator.CalculateHexPosition(x, y, settings.TileSize);

            float rotAngle = (((x * 7 + y * 11) % 6) * 60f);
            Quaternion rot = Quaternion.Euler(0, rotAngle, 0);

            // Spawning the building
            GameObject buildingInstance = Instantiate(building, worldPos, rot, _villageParent);
            buildingInstance.name = $"Building_{x}_{y}";

            // Updating the dictionary
            GridGenerator.HexTiles[gridPosition] = buildingInstance;
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
                            //Delete previous building
                            DestroyImmediate(GridGenerator.HexTiles[gridPosition]);
                            SpawnBuildingAtPosition(gridPosition, x, y, rng); 

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
        
        private List<Vector2Int> GetHexNeighbours(Vector2Int coords)
        {
            //The selected code is part of a method that calculates the neighboring
            //hex tiles for a given hex tile in a hexagonal grid.
            //The method takes the coordinates of the current hex tile
            //as input and returns a list of coordinates representing its neighbors.
            //The hexagonal grid is represented using an offset coordinate system,
            //where the rows are staggered. This means that the neighbors of a hex tile
            //depend on whether the row index (y) is even or odd.
            
            List<Vector2Int> neighbours = new List<Vector2Int>();
            int x = coords.x, y = coords.y;
            //Offset is going to differ for odd and even rows
            //For even rows (y % 2 == 0), the neighbors are calculated as follows:
            if (y % 2 == 0)
            {
                neighbours.Add(new Vector2Int(x, y - 1));
                neighbours.Add(new Vector2Int(x + 1, y - 1));
                neighbours.Add(new Vector2Int(x - 1, y));
                neighbours.Add(new Vector2Int(x + 1, y));
                neighbours.Add(new Vector2Int(x, y + 1));
                neighbours.Add(new Vector2Int(x + 1, y + 1 ));
            }
            else
            //For odd rows, the neighbors are calculated differently:
            {
                neighbours.Add(new Vector2Int(x - 1, y - 1));
                neighbours.Add(new Vector2Int(x, y - 1));
                neighbours.Add(new Vector2Int(x - 1, y));
                neighbours.Add(new Vector2Int(x + 1, y));
                neighbours.Add(new Vector2Int(x - 1, y + 1));
                neighbours.Add(new Vector2Int(x, y + 1));
            }
            return neighbours;        }

        //Solve the holes problem
        private bool AreAllAdjacentTilesLand(Vector2Int coords)
        {
            List<Vector2Int> neighbours = GetHexNeighbours(coords);

            foreach (Vector2Int n in neighbours)
            {
                string tileType = GetTileTypeAtPos(n);
                if(tileType == "water")
                {
                    return false;
                }
            }
            return true;
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