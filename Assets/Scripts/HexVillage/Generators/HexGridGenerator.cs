using System;
using System.Collections.Generic;
using UnityEngine;


namespace HexVillage.Generators
{
    [ExecuteAlways]
    public class HexGridGenerator : MonoBehaviour
    {
        private HexVillageSettings _settings;
        public Dictionary<Vector2Int, GameObject> HexTiles = new Dictionary<Vector2Int, GameObject>();
        private System.Random _rng;
        public void Initialise(HexVillageSettings settings)
        {
            _settings = settings;
            _rng = new System.Random(settings.RandomSeed);
        }

        public void GenerateGrid()
        {
            Initialise(_settings);
            //---- Island Generation Parametes ----
            float noiseScale = 0.3f;
            float noiseWeight = 0.2f;
            
            float waterThreshold = 0.7f;
            float sandThreshold = 0.6f;
            float dirtThreshold = 0.5f;
            //Rest is grass
            
            // Determine the grid's center and max distance 
            Vector2 center = new Vector2(_settings.GridWidth /2f, _settings.GridHeight /2f);
            float maxDist = center.magnitude;
            
            for (int x = 0; x < _settings.GridWidth; x++)
            {
                for (int y = 0; y < _settings.GridHeight; y++)
                {
                    Vector3 worldPos = CalculateHexPosition(x, y, _settings.TileSize);
                    
                    //Compute normalised distance from the grid's centre
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    float normalisedDistance = dist / maxDist;
                    
                    // Add Perlin noise for irregularity
                    float perlinNoise = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);
                    float value = normalisedDistance + perlinNoise * noiseWeight;
                    
                    // Determine the tile type
                    string tileType = "";
                    if (value > waterThreshold)
                    {
                        tileType = "water";
                    }else if (value > sandThreshold)
                    {
                        tileType = "sand";
                    }else if (value > dirtThreshold)
                    {
                        tileType = "dirt";
                    }else
                    {
                        tileType = "grass";
                    }
                    
                    //Get the model
                    GameObject tile = GetTerrainTile(tileType);
                    GameObject spawnedTile = Instantiate(tile, worldPos, Quaternion.identity, transform);
                    spawnedTile.name = $"HexTile_{x}_{y}_{tileType}";
                    HexTiles[new Vector2Int(x, y)] = spawnedTile;
                }
            }
        }

        private GameObject GetTerrainTile(string type = "grass")
        {
            type = type.ToLower();
            List<Tuple<GameObject, float>> candidates = new List<Tuple<GameObject, float>>();
            float totalWeight = 0;
    
            foreach (GameObject tile in _settings.Terrain)
            {
                if (tile.name.Contains(type))
                {
                    //10% chance of spawning a different tile
                    float weight = 1f;
                    if (tile.name == type) weight = 10f;
                    candidates.Add(Tuple.Create(tile, weight));
                    totalWeight += weight;
                }
            }

            if (candidates.Count > 0)
            {
                float random = (float)(_rng.NextDouble() * totalWeight);
                float cumulative = 0;
        
                foreach (var candidate in candidates)
                {
                    cumulative += candidate.Item2;
                    if (random <= cumulative)
                    {
                        return candidate.Item1;
                    }
                }
                return candidates[^1].Item1; // Fallback to last item
            }
            return null;
        }

        public Vector3 CalculateHexPosition(int x, int y, float size)
        {
            float posX = size * Mathf.Sqrt(3f) * (x + 0.5f * (y % 2));
            float posZ = size * 1.5f * y;
            return new Vector3(posX, 0, posZ);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            for (int x = 0; x < _settings.GridWidth; x++)
            {
                for (int y = 0; y < _settings.GridHeight; y++)
                {
                    Vector3 pos = CalculateHexPosition(x, y, _settings.TileSize);
                    Gizmos.DrawWireSphere(pos, 0.1f);
                }
            }
        }
    }
}