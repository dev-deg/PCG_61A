using System;
using UnityEngine;

namespace ProceduralGeneration.Utilities
{
    /// <summary>
    /// Provides methods for generating splat maps from heightmaps.
    /// A splatmap (or alpha map) is a 3D array that controls how terrain textures blend together.
    /// For each point on the terrain, it stores a weight for each texture layer.
    /// These weights determine how much of each texture is visible at that point.
    /// </summary>
    public static class SplatMapUtility
    {
        /// <summary>
        /// Generates a terrain texture blend map based on height and slope.
        /// This method demonstrates a fundamental procedural texturing technique based on terrain features:
        /// - Low, flat areas get grass textures
        /// - Steep slopes get rock textures regardless of height
        /// - High elevations get snow textures
        /// This produces natural-looking terrain where:
        /// - Valleys and plains are grassy
        /// - Cliffs and steep hillsides are rocky
        /// - Mountain peaks are snow-covered
        /// </summary>
        /// <param name="heightmap">A 2D array of normalized heights [0-1]</param>
        /// <returns>A 3D blend map with weights for each texture layer</returns>
        public static float[,,] GenerateSplatMap(float[,] heightmap)
        {
            // Validate input
            if (heightmap == null)
                throw new ArgumentNullException(nameof(heightmap));

            int width = heightmap.GetLength(0);
            int height = heightmap.GetLength(1);
            int layers = 3; // We use exactly 3 texture layers: grass, rock, snow
            
            // Create our splatmap - this is what we'll return
            float[,,] splatMap = new float[width, height, layers];

            // These thresholds control the height-based texture transitions
            // STUDENTS: Try adjusting these values to see how they affect texture distribution
            float grassThreshold = 0.4f;   // Below this height is primarily grass
            float snowThreshold = 0.7f;    // Above this height is primarily snow
            float slopeThreshold = 0.25f;  // Slopes steeper than this get rock texture

            // Process each point on the terrain
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float currentHeight = heightmap[x, y];

                    // STEP 1: Calculate the slope at this point
                    // Higher slope values mean steeper terrain
                    float slope = CalculateSlope(heightmap, x, y, width, height);
                    
                    // STEP 2: Calculate weights for each texture layer
                    // Start with weights of 0
                    float grassWeight = 0f;
                    float rockWeight = 0f;
                    float snowWeight = 0f;
                    
                    // GRASS WEIGHT: Higher at low elevations and flat areas
                    // More grass on flat terrain and areas below grassThreshold
                    grassWeight = (1f - slope) * (1f - currentHeight / grassThreshold);
                    grassWeight = Mathf.Clamp01(grassWeight);
                    
                    // ROCK WEIGHT: Higher on steep slopes
                    // Areas with high slope get more rock texture
                    rockWeight = slope / slopeThreshold;
                    rockWeight = Mathf.Clamp01(rockWeight);
                    
                    // SNOW WEIGHT: Higher at high elevations
                    // Areas above snowThreshold get more snow
                    if (currentHeight > snowThreshold)
                    {
                        // Linear ramp from snowThreshold (0) to 1.0 (1)
                        snowWeight = (currentHeight - snowThreshold) / (1f - snowThreshold);
                        snowWeight *= (1f - slope); // Less snow on steep slopes
                        snowWeight = Mathf.Clamp01(snowWeight);
                    }

                    // STEP 3: Normalize weights so they sum to 1
                    // This ensures textures always blend properly
                    float totalWeight = grassWeight + rockWeight + snowWeight;
                    
                    // Avoid division by zero (if all weights are 0)
                    if (totalWeight < 0.01f)
                    {
                        // Default to grass if no clear choice
                        splatMap[x, y, 0] = 1f; // Grass
                        splatMap[x, y, 1] = 0f; // Rock
                        splatMap[x, y, 2] = 0f; // Snow
                    }
                    else
                    {
                        // Normalize weights to sum to 1
                        splatMap[x, y, 0] = grassWeight / totalWeight; // Grass
                        splatMap[x, y, 1] = rockWeight / totalWeight;  // Rock
                        splatMap[x, y, 2] = snowWeight / totalWeight;  // Snow
                    }
                }
            }

            return splatMap;
        }
        
        /// <summary>
        /// Calculates the slope (steepness) at a point in the heightmap
        /// This is a simple method to calculate terrain steepness by comparing
        /// a point's height with its neighbors. The more height difference,
        /// the steeper the terrain at that point.
        /// </summary>
        private static float CalculateSlope(float[,] heightmap, int x, int y, int width, int height)
        {
            float centerHeight = heightmap[x, y];
            float totalDifference = 0f;
            int neighborCount = 0;
            
            // Check all 8 neighboring cells
            for (int offsetX = -1; offsetX <= 1; offsetX++)
            {
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    // Skip the center cell
                    if (offsetX == 0 && offsetY == 0) continue;
                    
                    // Make sure we don't go out of bounds
                    int neighborX = Mathf.Clamp(x + offsetX, 0, width - 1);
                    int neighborY = Mathf.Clamp(y + offsetY, 0, height - 1);
                    
                    // Add the absolute height difference
                    totalDifference += Mathf.Abs(centerHeight - heightmap[neighborX, neighborY]);
                    neighborCount++;
                }
            }
            
            // Return the average height difference (slope)
            return totalDifference / neighborCount;
        }
    }
}
