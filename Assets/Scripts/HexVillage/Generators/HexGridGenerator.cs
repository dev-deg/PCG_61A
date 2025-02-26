using System;
using System.Collections.Generic;
using UnityEngine;


namespace HexVillage.Generators
{
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
            for (int x = 0; x < _settings.GridWidth; x++)
            {
                for (int y = 0; y < _settings.GridHeight; y++)
                {

                }
            }
        }

        private GameObject GetTerrainTile(string type = "grass")
        {
            type = type.ToLower();
            List<GameObject> candidates = new List<GameObject>();
            foreach (GameObject tile in _settings.Terrain)
            {
                if (tile.name.ToLower() == type)
                {
                    return tile;
                }
                if (tile.name.Contains(type))
                {
                    candidates.Add(tile);
                }
            }
            if (candidates.Count > 0)
            {
                return candidates[_rng.Next(0, candidates.Count)];
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