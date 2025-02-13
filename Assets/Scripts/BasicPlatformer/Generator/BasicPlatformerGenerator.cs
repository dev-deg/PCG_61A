
using System;
using UnityEngine;
using BasicPlatformer.Settings;
using Sirenix.OdinInspector;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace BasicPlatformer.Generators
{

    public enum TopTileType
    {
        NormalGrass,
        LeftCornerGrass,
        RightCornerGrass,
        Water
    }

    [ExecuteAlways]
    public class BasicPlatformerGenerator : MonoBehaviour
    {
        [Title("Tilemap Generator Settings")] 
        [SerializeField]
        private BasicPlatformerSettings settings;

        [SerializeField] [Required] private Tilemap groundTilemap;

        // Auto-assign a tilemap if not set
        private void OnValidate()
        {
            if (groundTilemap == null)
                groundTilemap = GetComponent<Tilemap>();
        }
        
        [Button("Generate Terrain")]
        public void GenerateTerrain()
        {
            ResetTerrain();
            
            //PCG Algorithm
            
            // Step 1. Randomise Terrain
            int[] heights = GenerateHeights();
            // Step 2. Determine Corner Placement

            // Step 3. Render the terrain
            RenderTiles(heights, null);
        }

        private int[] GenerateHeights()
        {
            int width = settings.Width;
            int[] heights = new int[settings.Width];
            System.Random rng = new System.Random(settings.Seed);
            
            int minSurface = settings.MinSurfaceHeight;
            int maxSurface = settings.MaxSurfaceHeight;
            int maxHeightVariation = settings.MaxHeightVariation;
            int minSectionWidth = settings.MinSectionWidth;

            int currentHeight = rng.Next(minSurface, maxSurface + 1);

            int i = 0;
            while (i < width)
            {
                //TODO: Generate the heights
                int remaining = width - i;
                
                //Generate a random Width
                int sectionWidth = (remaining < minSectionWidth)
                    ? remaining
                    : rng.Next(minSectionWidth, Math.Min(minSectionWidth + 3, remaining) + 1);
                //Generate a random Height
                if (i > 0)
                {
                    int delta = rng.Next(-maxHeightVariation, maxHeightVariation + 1);
                    currentHeight = Mathf.Clamp(heights[i - 1] + delta, minSurface, maxSurface);
                }

                for (int j = 0; j < sectionWidth && i < width; j++)
                {
                    heights[i] = currentHeight;
                    i++;
                }
            }
            return heights;
        }

        private void RenderTiles(int[] heights, TopTileType[] topTileTypes)
        {
            int width = settings.Width;
            int dirtDepth = settings.DirtDepth;

            for (int x = 0; x < width; x++)
            {
                int H = heights[x];
                TileBase topTile = settings.GrassTile;
                
                //Place the top grass tile
                Vector3Int pos = new Vector3Int(x, H, 0);
                groundTilemap.SetTile(pos, topTile);
                
                //Place dirt tiles beneath it
                for (int y = H - 1; y >= H - dirtDepth; y--)
                {
                    groundTilemap.SetTile(new Vector3Int(x,y,0),settings.DirtTile);
                }
            }
        }
        
        [Button("Reset Terrain")]
        public void ResetTerrain()
        {
            if (groundTilemap == null)
            {
                Debug.LogError("Ground Tilemap reference missing..");
                return;
            }
            groundTilemap.ClearAllTiles();
        }
    }
}

