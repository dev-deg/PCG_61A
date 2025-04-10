using Sirenix.OdinInspector;
using UnityEngine;

namespace TerrainBasedGeneration.Generators
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer))]
    public class TerrainMeshGenerator : MonoBehaviour
    {
        [SerializeField, Range(10, 1000)] private int terrainWidth = 100;

        [SerializeField, Range(10, 1000)] private int terrainlength = 100;

        [SerializeField, Range(1, 100)] private float terrainHeight = 20f;

        [Tooltip("Heightmap Resolution")] [SerializeField, Range(10, 500)]
        private int resolution = 100;

        [SerializeField] private int randomSeed = 10;

        [SerializeField, Range(0.001f, 0.1f)] private float noiseScale = 0.03f;

        [SerializeField, Range(1, 8)] private int octaves = 4;

        [SerializeField, Range(0.1f, 1f)] private float persistance = 0.5f;

        [SerializeField, Range(1f, 4f)] private float lacunarity = 2f;

        [SerializeField] private bool flatTerrain = false;

        [SerializeField] private bool applySmoothing = true;

        [SerializeField, Range(1, 5)] private int smoothingRadius = 2;

        [SerializeField, Range(1, 5)] private int smoothingIterations = 2;

        [SerializeField] private Material terrainMaterial;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;
        private System.Random _rng;

        private void OnValidate()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshCollider = GetComponent<MeshCollider>();
        }

        private void InitialiseRng()
        {
            _rng = new System.Random(randomSeed);
        }

        [Button("Generate Terrain")]
        private void GenerateTerrain()
        {
            OnValidate();
            InitialiseRng();
            
            //Initialising the heightmap
        }
    }
}
