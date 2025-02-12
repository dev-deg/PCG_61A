
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
        }

        private int[] GenerateHeights()
        {
            int width = settings.Width;
            int[] heights = new int[settings.Width];
            System.Random rng = new System.Random(settings.Seed);
            
            int minSurface = settings.MinSurfaceHeight;
            int maxSurface = settings.MaxSurfaceHeight;
            int maxHeightVariations = settings.MaxHeightVariation;
            int minSectionWidth = settings.MinSectionWidth;

            int currentHeight = rng.Next(minSurface, maxSurface + 1);

            int i = 0;
            while (i < width)
            {
                //TODO: Generate the heights
            }
            return heights;
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

