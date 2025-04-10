using UnityEngine;

namespace ProceduralGeneration.Utilities
{
    /// <summary>
    /// Provides utility methods for generating and sampling noise in 2D and 3D.
    /// This class now supports basic Perlin noise, fractal noise (fBm), and value noise.
    /// </summary>
    public static class NoiseUtility
    {
        /// <summary>
        /// Creates a 2D float array using Unity's Mathf.PerlinNoise for each sample.
        /// The resulting values are typically in [0..1].
        /// </summary>
        /// <param name="width">Number of samples in X dimension.</param>
        /// <param name="height">Number of samples in Y dimension.</param>
        /// <param name="scale">Noise frequency scale. Larger scale = slower noise changes.</param>
        /// <param name="offsetX">Horizontal offset for the noise.</param>
        /// <param name="offsetY">Vertical offset for the noise.</param>
        /// <returns>A 2D array [width, height] of float noise values.</returns>
        public static float[,] GeneratePerlinNoise2D(int width, int height, float scale, float offsetX, float offsetY)
        {
            float[,] noiseMap = new float[width, height];

            if (scale <= 0f) scale = 0.0001f;  // avoid zero or negative scale

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float sampleX = (x + offsetX) / scale;
                    float sampleY = (y + offsetY) / scale;
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseMap[x, y] = perlinValue;
                }
            }
            return noiseMap;
        }

        /// <summary>
        /// A simple 3D Perlin noise approximation, sampling 2D slices at incremental offsets.
        /// This is not "true" 3D Perlin noise, but works for certain 3D procedural tasks.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        /// <param name="scale">Noise scale.</param>
        /// <returns>Noise in [0..1].</returns>
        public static float PerlinNoise3D(float x, float y, float z, float scale)
        {
            float xy = Mathf.PerlinNoise(x / scale, y / scale);
            float yz = Mathf.PerlinNoise(y / scale, z / scale);
            float xz = Mathf.PerlinNoise(x / scale, z / scale);
            return (xy + yz + xz) / 3f;
        }

        /// <summary>
        /// Generates fractal noise (fractal Brownian motion) in 2D by combining multiple octaves of Perlin noise.
        /// This produces more detailed and natural-looking noise patterns.
        /// </summary>
        /// <param name="width">Number of samples in X dimension.</param>
        /// <param name="height">Number of samples in Y dimension.</param>
        /// <param name="scale">Base scale for the noise.</param>
        /// <param name="octaves">Number of noise layers to combine.</param>
        /// <param name="persistence">Controls amplitude decrease per octave (typically between 0 and 1).</param>
        /// <param name="lacunarity">Controls frequency increase per octave (typically > 1).</param>
        /// <param name="offsetX">Horizontal offset for the noise.</param>
        /// <param name="offsetY">Vertical offset for the noise.</param>
        /// <returns>A 2D array [width, height] of fractal noise values in [0..1].</returns>
        public static float[,] GenerateFractalNoise2D(int seed, int width, int height, float scale, int octaves, float persistence, float lacunarity, float offsetX, float offsetY)
        {
            float[,] noiseMap = new float[width, height];

            if (scale <= 0f) scale = 0.0001f;
            if (octaves < 1) octaves = 1;

            // Precompute octave offsets
            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetXPerOctave = offsetX + RandomRange(seed,-100000, 100000);
                float offsetYPerOctave = offsetY + RandomRange(seed, -100000, 100000);
                octaveOffsets[i] = new Vector2(offsetXPerOctave, offsetYPerOctave);
            }

            float maxAmplitude = 0f;
            float amplitude = 1f;
            float frequency = 1f;

            // First pass to calculate normalization factor.
            for (int i = 0; i < octaves; i++)
            {
                maxAmplitude += amplitude;
                amplitude *= persistence;
            }

            // Reset amplitude and frequency.
            amplitude = 1f;
            frequency = 1f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float noiseHeight = 0f;
                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x / scale * frequency) + octaveOffsets[i].x;
                        float sampleY = (y / scale * frequency) + octaveOffsets[i].y;
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;
                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }
                    // Normalize result to [0,1]
                    noiseMap[x, y] = (noiseHeight / maxAmplitude + 1) / 2f;
                    amplitude = 1f;
                    frequency = 1f;
                }
            }
            return noiseMap;
        }

        /// <summary>
        /// Generates value noise in 2D. Value noise is computed by generating a grid of random values
        /// and then interpolating between these values using bilinear interpolation.
        /// </summary>
        /// <param name="width">Number of samples in X dimension.</param>
        /// <param name="height">Number of samples in Y dimension.</param>
        /// <param name="gridSize">Size of the grid cells used for generating base noise values.</param>
        /// <returns>A 2D array [width, height] of value noise in [0..1].</returns>
        public static float[,] GenerateValueNoise2D(int seed, int width, int height, int gridSize)
        {
            float[,] noiseMap = new float[width, height];

            // Create a grid of random values.
            int gridWidth = Mathf.CeilToInt((float)width / gridSize) + 1;
            int gridHeight = Mathf.CeilToInt((float)height / gridSize) + 1;
            float[,] grid = new float[gridWidth, gridHeight];
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    grid[x, y] = RandomRange(seed, 0f, 1f);
                }
            }

            // Interpolate between grid points using bilinear interpolation.
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float gx = (float)x / gridSize;
                    float gy = (float)y / gridSize;
                    int ix = Mathf.FloorToInt(gx);
                    int iy = Mathf.FloorToInt(gy);
                    float fracX = gx - ix;
                    float fracY = gy - iy;

                    float v00 = grid[ix, iy];
                    float v10 = grid[Mathf.Min(ix + 1, gridWidth - 1), iy];
                    float v01 = grid[ix, Mathf.Min(iy + 1, gridHeight - 1)];
                    float v11 = grid[Mathf.Min(ix + 1, gridWidth - 1), Mathf.Min(iy + 1, gridHeight - 1)];

                    // Bilinear interpolation.
                    float interpX1 = Mathf.Lerp(v00, v10, fracX);
                    float interpX2 = Mathf.Lerp(v01, v11, fracX);
                    float value = Mathf.Lerp(interpX1, interpX2, fracY);

                    noiseMap[x, y] = value;
                }
            }
            return noiseMap;
        }

        /// <summary>
        /// Returns a random float between min and max.
        /// A helper method for noise generation.
        /// </summary>
        private static float RandomRange(int seed, float min, float max)
        {
            System.Random rng = new System.Random(seed);
            return (float)(rng.NextDouble() * (max - min) + min);
            return (float)(rng.NextDouble() * (max - min) + min);
        }
    }
}
