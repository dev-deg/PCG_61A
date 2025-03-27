using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TerrainBasedGeneration.Settings
{
    [Serializable]
    public class TerrainBasedGenerationSettings
    {
        [FoldoutGroup("Terrain Dimensions")]
        [Range(100,2000)]
        [SerializeField]
        private int terrainWidth = 500;
        
        [FoldoutGroup("Terrain Dimensions")]
        [Range(100,2000)]
        [SerializeField]
        private int terrainLength = 500;
        
        [FoldoutGroup("Terrain Dimensions")]
        [Range(10,600)]
        [SerializeField]
        private int terrainHeight = 100;
        
        [FoldoutGroup("Heightmap Resolution")]
        [Range(33,4097)]
        [SerializeField]
        private int heightMapResolution = 513;
        
        [FoldoutGroup("Randomisation")]
        [SerializeField]
        private int randomSeed = 123;
        
        [FoldoutGroup("Noise Settings")]
        [Range(0.001f,0.1f)]
        [SerializeField]
        private float noiseScale = 0.05f;
        
        [FoldoutGroup("Noise Settings")]
        [Tooltip("Represents the number of layers of noise that will be combined to create the final noise. Higher values result in more detailed noise patterns.")]
        [Range(1,8)]
        [SerializeField]
        private int octaves = 4;
        
        [FoldoutGroup("Noise Settings")]
        [Tooltip("Controls the amplitude of each octave. Lower values make higher octaves contribute less to the final noise, resulting in smoother noise.")]
        [Range(0.1f,1f)]
        [SerializeField]
        private float persistance = 0.5f;
        
        [FoldoutGroup("Noise Settings")]
        [Tooltip("Controls the frequency of each octave. Higher values increase the frequency, resulting in more detailed noise patterns.")]
        [Range(1,4)]
        [SerializeField]
        private float lacunarity = 2.5f;
        
        [FoldoutGroup("Terrain Features")]
        [Range(0.1f,5f)]
        [SerializeField]
        private float heightMultiplier = 1.5f;
        
        [FoldoutGroup("Terrain Features")]
        [Tooltip("Curve that controls how height values are mapped (allows for plateaus, cliffs, ect.)")]
        [SerializeField]
        private AnimationCurve heightCurve = AnimationCurve.Linear(0,0,1,1);
        
        [FoldoutGroup("Smoothing Settings")]
        [SerializeField]
        private Boolean enableSmoothing = true;
        
        [FoldoutGroup("Smoothing Settings")]
        [Range(1,5)]
        [SerializeField]
        private int smoothingRadius = 1;
        
        [FoldoutGroup("Smoothing Settings")]
        [Range(1,5)]
        [SerializeField]
        private int smoothingIterations = 1;
        
        [FoldoutGroup("Texture Settings")]
        [SerializeField]
        private Texture2D grassTexture;
        
        [FoldoutGroup("Texture Settings")]
        [SerializeField]
        private Texture2D grassNormalMap;

        [FoldoutGroup("Texture Settings")]
        [SerializeField]
        private Texture2D rockTexture;
        
        [FoldoutGroup("Texture Settings")]
        [SerializeField]
        private Texture2D rockNormalMap;

        public Texture2D GrassTexture => grassTexture;

        public Texture2D GrassNormalMap => grassNormalMap;

        public Texture2D RockTexture => rockTexture;

        public Texture2D RockNormalMap => rockNormalMap;

        public int TerrainWidth => terrainWidth;

        public int TerrainLength => terrainLength;

        public int TerrainHeight => terrainHeight;

        public int HeightMapResolution => heightMapResolution;

        public int RandomSeed => randomSeed;

        public float NoiseScale => noiseScale;

        public int Octaves => octaves;

        public float Persistance => persistance;

        public float Lacunarity => lacunarity;

        public float HeightMultiplier => heightMultiplier;

        public AnimationCurve HeightCurve => heightCurve;

        public bool EnableSmoothing => enableSmoothing;

        public int SmoothingRadius => smoothingRadius;

        public int SmoothingIterations => smoothingIterations;
    }
}