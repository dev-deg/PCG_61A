
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
        [SerializeField] [Required] private Tilemap backgroundTilemap;
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
            TopTileType[] topTileTypes = DetermineCornerTiles(heights);
            // Step 3. Render the terrain
            RenderTiles(heights, topTileTypes);
            // Step 4. Render the background
            RenderSkyBackground(heights);
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

        private TopTileType[] DetermineCornerTiles(int[] heights)
        {
            int width = heights.Length;
            int dirtDepth = settings.DirtDepth;
            TopTileType[] topTileTypes = new TopTileType[width];

            for (int x = 0; x < width; x++)
            {
                int H = heights[x];
                bool leftOccupied = false;
                bool rightOccupied = false;
                
                //Checking the left neighbour
                if (x > 0)
                {
                    int leftTop = heights[x - 1];
                    leftOccupied = (H >= leftTop - dirtDepth) && (H <= leftTop);
                }
                
                //Checking the right neighbour
                if (x < width-1)
                {
                    int rightTop = heights[x + 1];
                    rightOccupied = (H >= rightTop - dirtDepth) && (H <= rightTop);
                }
                
                //if this is the first tile we set to left grass corner
                if (x == 0)
                {
                    topTileTypes[x] = TopTileType.LeftCornerGrass;
                    //if this is the last tile we set to right grass corner
                }else if (x == width - 1)
                {
                    topTileTypes[x] = TopTileType.RightCornerGrass; 
                }
                else
                {
                    if (leftOccupied && rightOccupied)
                    {
                        topTileTypes[x] = TopTileType.NormalGrass;
                    }else if (!leftOccupied && rightOccupied)
                    {
                        topTileTypes[x] = TopTileType.LeftCornerGrass;
                    }else if (leftOccupied && !rightOccupied)
                    {
                        topTileTypes[x] = TopTileType.RightCornerGrass;
                    }
                    else
                    {
                        //special tile
                        topTileTypes[x] = TopTileType.NormalGrass;
                    }
                }
            }
            return topTileTypes;
        }

        private void RenderTiles(int[] heights, TopTileType[] topTileTypes)
        {
            int width = settings.Width;
            int dirtDepth = settings.DirtDepth;

            for (int x = 0; x < width; x++)
            {
                int H = heights[x];
                TileBase topTile = settings.GrassTile;
                
                //Select top tile variant based on the corner assignment
                switch (topTileTypes[x])
                {
                    case TopTileType.NormalGrass:
                        topTile = settings.GrassTile;
                        break;
                    case TopTileType.LeftCornerGrass:
                        topTile = settings.GrassLeftTile;
                        break;
                    case TopTileType.RightCornerGrass:
                        topTile = settings.GrassRightTile;
                        break;
                }
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

        private void RenderSkyBackground(int[] heights)
        {
            int width = heights.Length;
            
            // Finding the highest tile from the array
            int highestTile = int.MinValue;
            for (int i = 0; i < heights.Length; i++)
            {
                if (heights[i] > highestTile)
                    highestTile = heights[i];
            }
            //The maximum height of the sky (highest tile in the array + the fill extra
            int maxSkyHeight = highestTile + settings.SkyBackgroundFillExtra;
            
            //Loop through every x value and its corresping area of the sky to draw the sky tile
            for (int x = 0; x < width; x++)
            {
                int colTop = heights[x];
                for (int y = colTop + 1; y <= maxSkyHeight; y++)
                {
                    backgroundTilemap.SetTile(new Vector3Int(x,y,-1),settings.SkyTile);
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
            backgroundTilemap.ClearAllTiles();
        }
    }
}

