using UnityEngine;
using System.Collections.Generic;
using ProceduralGeneration.Utilities;

namespace TerrainBasedGeneration.Utilities
{
    /// <summary>
    /// Provides utility methods specifically for terrain-based generation using Unity's Terrain component.
    /// </summary>
    public static class TerrainUtility
    {
        /// <summary>
        /// Generates a heightmap using fractal noise with the specified parameters.
        /// </summary>
        /// <param name="resolution">Size of the heightmap (must be 2^n+1)</param>
        /// <param name="scale">Scale of the noise (lower values = larger features)</param>
        /// <param name="octaves">Number of octaves (detail layers)</param>
        /// <param name="persistence">Controls how much influence higher octaves have</param>
        /// <param name="lacunarity">Controls frequency increase per octave</param>
        /// <param name="seed">Random seed for reproducible results</param>
        /// <returns>A heightmap as a 2D array of normalized height values [0-1]</returns>
        public static float[,] GenerateHeightmap(int resolution, float scale, int octaves, 
                                                float persistence, float lacunarity, int seed)
        {
            // Use the seed to create a reproducible random offset
            System.Random prng = new System.Random(seed);
            float offsetX = RandomUtility.Range(prng, -10000f, 10000f);
            float offsetY = RandomUtility.Range(prng, -10000f, 10000f);
            
            // Generate fractal noise using the NoiseUtility from ProceduralGeneration
            return NoiseUtility.GenerateFractalNoise2D(
                resolution, resolution, 
                scale, octaves, 
                persistence, lacunarity, 
                offsetX, offsetY);
        }
        
        /// <summary>
        /// Applies a heightmap to a Unity Terrain component.
        /// </summary>
        /// <param name="terrain">The terrain component to modify</param>
        /// <param name="heightmap">The heightmap data [0-1]</param>
        /// <param name="heightCurve">Optional curve to modify the height distribution</param>
        public static void ApplyHeightmapToTerrain(Terrain terrain, float[,] heightmap, AnimationCurve heightCurve = null)
        {
            if (terrain == null || heightmap == null)
                return;
                
            // Get terrain data reference
            TerrainData terrainData = terrain.terrainData;
            
            // Apply heightmap resolution
            int resolution = heightmap.GetLength(0);
            terrainData.heightmapResolution = resolution;
            
            // Debug info to verify heightmap has proper variation
            float minHeight = float.MaxValue;
            float maxHeight = float.MinValue;
            
            // Create a copy of the heightmap to modify if we have a curve
            float[,] processedHeightmap = new float[resolution, resolution];
            
            // Apply the height curve if provided
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float height = heightmap[x, y];
                    
                    if (heightCurve != null)
                    {
                        // Apply the height curve to modify the distribution
                        height = heightCurve.Evaluate(height);
                    }
                    
                    // Apply additional contrast to ensure visible terrain features
                    // This helps prevent flat terrain by enhancing height differences
                    height = Mathf.Pow(height, 0.9f);
                    
                    processedHeightmap[x, y] = height;
                    
                    // Track min/max values for debugging
                    minHeight = Mathf.Min(minHeight, height);
                    maxHeight = Mathf.Max(maxHeight, height);
                }
            }
            
            // Debug info
            Debug.Log($"Heightmap range: min={minHeight}, max={maxHeight}, delta={maxHeight-minHeight}");
            
            // If the heightmap is still too flat, add more variation
            if (maxHeight - minHeight < 0.05f)
            {
                Debug.LogWarning("Final heightmap is too flat. Adding forced height variation.");
                for (int y = 0; y < resolution; y++)
                {
                    for (int x = 0; x < resolution; x++)
                    {
                        // Apply a sine wave pattern to create some hills
                        float xFactor = (float)x / resolution * 10f;
                        float yFactor = (float)y / resolution * 10f;
                        float variation = (Mathf.Sin(xFactor) * Mathf.Cos(yFactor) + 1f) * 0.4f;
                        
                        // Blend with original height
                        processedHeightmap[x, y] = processedHeightmap[x, y] * 0.2f + variation * 0.8f;
                    }
                }
            }
            
            // Apply the heightmap to the terrain
            terrainData.SetHeights(0, 0, processedHeightmap);
        }
        
        /// <summary>
        /// Creates or updates a terrain component for use with the generator.
        /// </summary>
        /// <param name="terrainGameObject">GameObject to add/get the terrain component from</param>
        /// <param name="width">Width of the terrain in world units</param>
        /// <param name="length">Length of the terrain in world units</param>
        /// <param name="height">Maximum height of the terrain</param>
        /// <returns>A reference to the terrain component</returns>
        public static Terrain SetupTerrain(GameObject terrainGameObject, int width, int length, int height)
        {
            // Get or create a terrain component
            Terrain terrain = terrainGameObject.GetComponent<Terrain>();
            if (terrain == null)
            {
                terrain = terrainGameObject.AddComponent<Terrain>();
                terrainGameObject.AddComponent<TerrainCollider>();
            }
            
            // Get or create terrain data
            TerrainData terrainData = terrain.terrainData;
            if (terrainData == null)
            {
                terrainData = new TerrainData();
                terrain.terrainData = terrainData;
            }
            
            // Set the terrain size
            terrainData.size = new Vector3(width, height, length);
            
            // Make sure the terrain collider uses the same terrain data
            TerrainCollider terrainCollider = terrainGameObject.GetComponent<TerrainCollider>();
            if (terrainCollider != null)
            {
                terrainCollider.terrainData = terrainData;
            }
            
            return terrain;
        }

        /// <summary>
        /// Applies Gaussian smoothing to a heightmap to reduce noise and spikiness
        /// </summary>
        /// <param name="heightmap">The original heightmap to smooth</param>
        /// <param name="blurRadius">Radius of the blur kernel (larger = smoother terrain)</param>
        /// <param name="iterations">Number of smoothing passes to perform</param>
        /// <returns>A smoothed heightmap</returns>
        public static float[,] SmoothHeightmap(float[,] heightmap, int blurRadius = 2, int iterations = 2)
        {
            int width = heightmap.GetLength(0);
            int height = heightmap.GetLength(1);
            
            float[,] result = new float[width, height];
            System.Array.Copy(heightmap, result, heightmap.Length);
            
            // Create a temporary buffer to store intermediate results
            float[,] tempHeightmap = new float[width, height];
            
            // Perform multiple iterations of smoothing
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                // Copy the current result to the temporary buffer
                System.Array.Copy(result, tempHeightmap, result.Length);
                
                // Apply Gaussian-like blur
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        float sum = 0;
                        int count = 0;
                        
                        // Sample neighboring pixels
                        for (int ky = -blurRadius; ky <= blurRadius; ky++)
                        {
                            for (int kx = -blurRadius; kx <= blurRadius; kx++)
                            {
                                int sampleX = Mathf.Clamp(x + kx, 0, width - 1);
                                int sampleY = Mathf.Clamp(y + ky, 0, height - 1);
                                
                                // Calculate weight based on distance (approximating Gaussian)
                                float distance = Mathf.Sqrt(kx * kx + ky * ky);
                                float weight = Mathf.Max(0, blurRadius - distance) / blurRadius;
                                
                                sum += tempHeightmap[sampleX, sampleY] * weight;
                                count += Mathf.RoundToInt(weight);
                            }
                        }
                        
                        // Apply weighted average
                        result[x, y] = (count > 0) ? sum / count : tempHeightmap[x, y];
                    }
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Normalizes a heightmap to ensure its values are within the [0-1] range
        /// </summary>
        /// <param name="heightmap">The heightmap to normalize</param>
        /// <returns>A normalized heightmap with values in [0-1]</returns>
        public static float[,] NormalizeHeightmap(float[,] heightmap)
        {
            int width = heightmap.GetLength(0);
            int height = heightmap.GetLength(1);
            
            // Find min and max values
            float minHeight = float.MaxValue;
            float maxHeight = float.MinValue;
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float value = heightmap[x, y];
                    minHeight = Mathf.Min(minHeight, value);
                    maxHeight = Mathf.Max(maxHeight, value);
                }
            }
            
            // If the heightmap is flat, add some variation
            if (Mathf.Approximately(minHeight, maxHeight))
            {
                Debug.LogWarning("Heightmap is completely flat! Adding random variation.");
                return GenerateHeightmap(width, 0.03f, 3, 0.4f, 2.5f, UnityEngine.Random.Range(0, 100000));
            }
            
            // Ensure we have an adequate range for terrain features
            if (maxHeight - minHeight < 0.1f)
            {
                Debug.LogWarning($"Heightmap has very little variation (range: {maxHeight - minHeight}). Enhancing contrast.");
                // Create a new heightmap with enhanced contrast
                float[,] enhancedHeightmap = new float[width, height];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Enhance contrast by expanding the range
                        float normalizedValue = (heightmap[x, y] - minHeight) / (maxHeight - minHeight);
                        // Apply a power function to increase contrast (values below 0.5 decrease, above 0.5 increase)
                        enhancedHeightmap[x, y] = Mathf.Pow(normalizedValue, 0.7f);
                    }
                }
                return enhancedHeightmap;
            }
            
            // Normal case - heightmap has good variation, just normalize to [0-1]
            float[,] normalizedHeightmap = new float[width, height];
            float range = maxHeight - minHeight;
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    normalizedHeightmap[x, y] = (heightmap[x, y] - minHeight) / range;
                }
            }
            
            return normalizedHeightmap;
        }
    }
}