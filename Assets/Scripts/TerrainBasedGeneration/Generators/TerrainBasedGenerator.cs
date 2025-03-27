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
        }
    }
}