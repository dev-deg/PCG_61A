using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TerrainBasedGeneration.Settings;
using UnityEngine;

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
            if (settings == null)
            {
                Debug.LogError("Connot generate terrain without settings");
                return;
            }

            InitialiseRandomGenerator();
            
            
        }
    }
}