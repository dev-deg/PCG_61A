using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TerrainBasedGeneration.Settings;
using UnityEngine;
using UnityEngine.TerrainUtils;
using TerrainBasedGeneration.Utilities;
namespace TerrainBasedGeneration.Generators
{
    [ExecuteAlways]
    [RequireComponent(typeof(Terrain))]
    public class TerrainBasedGenerator : MonoBehaviour
    {
        [SerializeField, InlineEditor]
        private TerrainBasedGenerationSettingsSO settings;

        private Terrain _terrainComponent;
        private System.Random _rng;
        private void OnValidate()
        {
            if (_terrainComponent == null)
            {
                _terrainComponent = GetComponent<Terrain>();
            }
        }
        
        private void InitialiseRandomGenerator()
        {
            _rng = new System.Random(settings.Settings.RandomSeed); 
        }

        [Button("Generate Terrain", ButtonSizes.Large), GUIColor(0.4f,0.8f,1f)]
        private void GenerateTerrain()
        {
            _terrainComponent = GetComponent<Terrain>();
            if (settings == null)
            {
                Debug.LogError("Connot generate terrain without settings");
                return;
            }

            // 1. Initialise random generator
            InitialiseRandomGenerator();

            // 2. Setup terrain
            GameObject terrainGO = gameObject;
            Terrain terrain = TerrainBasedGeneration.Utilities.TerrainUtility.SetupTerrain(
                terrainGO,
                settings.Settings.TerrainWidth,
                settings.Settings.TerrainLength,
                settings.Settings.TerrainHeight
                );
            _terrainComponent = terrain;
            
            // 3. Generate heightmap
            float[,] heightmap = TerrainBasedGeneration.Utilities.TerrainUtility.GenerateHeightmap(
                settings.Settings.HeightMapResolution,
                settings.Settings.NoiseScale,
                settings.Settings.Octaves,
                settings.Settings.Persistance,
                settings.Settings.Lacunarity,
                settings.Settings.RandomSeed
            );
            
            // 4. Normalise the heightmap to ensure the values are between 0 and 1.
            heightmap = TerrainBasedGeneration.Utilities.TerrainUtility.NormalizeHeightmap(heightmap);

            //5. Smoothen the heightmap if it is set in the settings
            if (settings.Settings.EnableSmoothing)
            {
                heightmap = TerrainBasedGeneration.Utilities.TerrainUtility.SmoothHeightmap(heightmap,
                    settings.Settings.SmoothingRadius, settings.Settings.SmoothingIterations);
            }
            
            //6. Apply the heightmap to the terrain
            TerrainBasedGeneration.Utilities.TerrainUtility.ApplyHeightmapToTerrain(
                _terrainComponent,
                heightmap,
                settings.Settings.HeightCurve);
            
            //7. Apply textures to the terrain
            ApplyTerrainTextures();
            
            //8. Spawn grass objects
            // SpawnGrassObjects();
        }
        
        private void ApplyTerrainTextures()
        {
            if (_terrainComponent == null) return;

            TerrainData terrainData = _terrainComponent.terrainData;

            // Set terrain layers
            TerrainLayer[] terrainLayers = new TerrainLayer[3];

            // Create terrain layers with both diffuse and normal maps
            terrainLayers[0] = new TerrainLayer
            {
                diffuseTexture = Resources.Load<Texture2D>("GrassUV02"),
                normalMapTexture = Resources.Load<Texture2D>("GrassUV02_N")
            };

            terrainLayers[1] = new TerrainLayer
            {
                diffuseTexture = Resources.Load<Texture2D>("GroundCracked01"),
                normalMapTexture = Resources.Load<Texture2D>("GroundCracked01_N")
            };

            terrainLayers[2] = new TerrainLayer
            {
                diffuseTexture = Resources.Load<Texture2D>("Snow"),
                normalMapTexture = Resources.Load<Texture2D>("Snow_N")
            };

            // Set texture scale for each layer
            for (int i = 0; i < terrainLayers.Length; i++)
            {
                terrainLayers[i].tileSize = new Vector2(20, 20);
                terrainLayers[i].tileOffset = Vector2.zero;
            }

            // Apply layers to terrain
            terrainData.terrainLayers = terrainLayers;

            // Set the control texture (splatmap) resolution
            int resolution = terrainData.heightmapResolution;
            terrainData.alphamapResolution = resolution;

            // Get heightmap for splatmap generation
            float[,] heightmap = terrainData.GetHeights(0, 0, resolution, resolution);

            // Generate and apply splatmap
            float[,,] splatmap = ProceduralGeneration.Utilities.SplatMapUtility.GenerateSplatMap(heightmap);

            // Ensure the splatmap dimensions match the alphamap resolution
            if (splatmap.GetLength(0) != terrainData.alphamapResolution || 
                splatmap.GetLength(1) != terrainData.alphamapResolution)
            {
                Debug.LogError($"Splatmap dimensions ({splatmap.GetLength(0)}x{splatmap.GetLength(1)}) " +
                              $"do not match terrain alphamap resolution ({terrainData.alphamapResolution})");
                return;
            }

            // Apply the splatmap
            terrainData.SetAlphamaps(0, 0, splatmap);
        }
        
    //     private void SpawnGrassObjects()
    //     {
    //         if (_terrainComponent == null) return;
    //
    //         TerrainData terrainData = _terrainComponent.terrainData;
    //         int resolution = terrainData.heightmapResolution;
    //
    //         // Get heightmap for splatmap generation
    //         float[,] heightmap = terrainData.GetHeights(0, 0, resolution, resolution);
    //
    //         // Generate splatmap using existing utility
    //         float[,,] splatmap = ProceduralGeneration.Utilities.SplatMapUtility.GenerateSplatMap(heightmap);
    //
    //         // Load the grass prefab
    //         GameObject grassPrefab = Resources.Load<GameObject>("Grass_A");
    //         if (grassPrefab == null)
    //         {
    //             Debug.LogError("Could not load Grass_A prefab from Resources folder");
    //             return;
    //         }
    //
    //         // Create a parent object for all grass instances
    //         GameObject grassParent = new GameObject("Grass_Instances");
    //         grassParent.transform.parent = transform;
    //
    //         // Parameters for grass placement
    //         float grassThreshold = 0.8f; // Higher threshold for grass-dominant areas
    //         float spawnChance = 0.01f;
    //         float slopeThreshold = 0.3f; // Maximum slope for grass placement
    //
    //         // Get terrain size for position calculation
    //         Vector3 terrainSize = terrainData.size;
    //
    //         for (int y = 0; y < resolution; y++)
    //         {
    //             for (int x = 0; x < resolution; x++)
    //             {
    //                 float slope = CalculateSlope(heightmap, x, y, resolution, resolution);
    //
    //                 // Check if this point is predominantly grass (layer 0) and the slope is gentle
    //                 if (splatmap[x, y, 0] >= grassThreshold && slope < slopeThreshold && UnityEngine.Random.value < spawnChance)
    //                 {
    //                     // Convert coordinates to world position
    //                     float xPos = (float)x / resolution * terrainSize.x;
    //                     float zPos = (float)y / resolution * terrainSize.z;
    //
    //                     // Get the terrain height at this position
    //                     float yPos = _terrainComponent.SampleHeight(new Vector3(xPos, 0, zPos));
    //
    //                     // Create grass instance
    //                     Vector3 spawnPosition = new Vector3(xPos, yPos, zPos) + transform.position;
    //                     GameObject grassInstance = Instantiate(grassPrefab, spawnPosition, 
    //                         Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0), grassParent.transform);
    //
    //                     // Scale the grass up
    //                     float scale = UnityEngine.Random.Range(1.8f, 2.2f);
    //                     grassInstance.transform.localScale *= scale;
    //                 }
    //             }
    //         }
    //     }
    //
    //     private float CalculateSlope(float[,] heightmap, int x, int y, int width, int height)
    //     {
    //         float centerHeight = heightmap[x, y];
    //         float totalDifference = 0f;
    //         int samples = 0;
    //
    //         // Check adjacent cells
    //         for (int offsetX = -1; offsetX <= 1; offsetX++)
    //         {
    //             for (int offsetY = -1; offsetY <= 1; offsetY++)
    //             {
    //                 if (offsetX == 0 && offsetY == 0) continue;
    //
    //                 int sampleX = Mathf.Clamp(x + offsetX, 0, width - 1);
    //                 int sampleY = Mathf.Clamp(y + offsetY, 0, height - 1);
    //
    //                 totalDifference += Mathf.Abs(heightmap[sampleX, sampleY] - centerHeight);
    //                 samples++;
    //             }
    //         }
    //
    //         return totalDifference / samples;
    //     }
    }
}