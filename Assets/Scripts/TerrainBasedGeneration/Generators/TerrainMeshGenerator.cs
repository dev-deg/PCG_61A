using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TerrainBasedGeneration.Generators
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer))]
    public class TerrainMeshGenerator : MonoBehaviour
    {
        [SerializeField, Range(10, 1000)] private int terrainWidth = 100;

        [SerializeField, Range(10, 1000)] private int terrainLength = 100;

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
            float[,] heightmap = GenerateHeightmap();
            
            // Create a mesh from the heightmap
            Vector3 scale = new Vector3(
                terrainWidth / (float) (resolution - 1),
                terrainHeight,
                terrainLength / (float) (resolution - 1));
            
            Mesh terrainMesh = GenerateTerrainMesh(heightmap,scale, terrainHeight);
            
            //Assign the mesh to the mesh filter
            _meshFilter.sharedMesh = terrainMesh;
            _meshCollider.sharedMesh = terrainMesh;

            if (terrainMaterial != null)
            {
                _meshRenderer.sharedMaterial = terrainMaterial;
            }
        }

        private Mesh GenerateTerrainMesh(float[,] heightmap, Vector3 scale, float heightMultiplier)
        {
            if (heightmap == null)
                throw new ArgumentNullException(nameof(heightmap));
            
            // Get the width (number of columns) of the heightmap array
            int width = heightmap.GetLength(0);

            // Get the height (number of rows) of the heightmap array
            int height = heightmap.GetLength(1);

            // Calculate the total number of vertices in the mesh
            // Each point in the heightmap corresponds to a vertex
            int numVertices = width * height;

            // Calculate the total number of triangles in the mesh
            // Each grid cell in the heightmap is divided into two triangles
            // (width - 1) * (height - 1) gives the number of grid cells
            // Multiplying by 6 accounts for 2 triangles per cell, each with 3 vertices
            int numTriangles = (width - 1) * (height - 1) * 6;

            // Create an array to store the 3D positions of the vertices
            Vector3[] vertices = new Vector3[numVertices];

            // Create an array to store the UV coordinates for texture mapping
            Vector2[] uv = new Vector2[numVertices];

            // Create an array to store the indices of the vertices that form the triangles
            int[] triangles = new int[numTriangles];
            
            //Generate vertices and UV coordinates
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = x * height + y;
                    float vertexHeight = heightmap[x, y] * heightMultiplier;
                    vertices[index] = new Vector3(x * scale.x, vertexHeight, y * scale.z);
                    uv[index] = new Vector2((float)x / (width - 1), (float)y / (height - 1));
                }
            }
            
            //Generate the triangles
            int triangleIndex = 0;
            for (int x = 0; x < width - 1; x++)
            {
                for (int y = 0; y < height - 1; y++)
                {
                    // Get the indices of the four corners of the current grid cell
                    int topLeft = x * height + y;
                    int topRight = (x + 1) * height + y;
                    int bottomLeft = x * height + (y + 1);
                    int bottomRight = (x + 1) * height + (y + 1);

                    // Create two triangles for the current grid cell
                    triangles[triangleIndex++] = topLeft;
                    triangles[triangleIndex++] = bottomLeft;
                    triangles[triangleIndex++] = topRight;

                    triangles[triangleIndex++] = bottomLeft;
                    triangles[triangleIndex++] = bottomRight;
                    triangles[triangleIndex++] = topRight;
                }
            }

            Mesh terrainMesh = new Mesh {vertices = vertices, triangles = triangles, uv = uv};
            terrainMesh.RecalculateNormals();
            return terrainMesh;
        }
        
        private float[,] GenerateHeightmap()
        {
            // Generate the heightmap using the TerrainUtility
            float[,] heightmap = TerrainBasedGeneration.Utilities.TerrainUtility.GenerateHeightmap(
                resolution,       // Resolution of the heightmap
                noiseScale,       // Scale of the noise
                octaves,          // Number of octaves for fractal noise
                persistance,      // Persistence value for fractal noise
                lacunarity,       // Lacunarity value for fractal noise
                randomSeed        // Random seed for reproducibility
            );

            // If flat terrain is enabled, set all heights to zero
            if (flatTerrain)
            {
                for (int y = 0; y < resolution; y++)
                {
                    for (int x = 0; x < resolution; x++)
                    {
                        heightmap[x, y] = 0f;
                    }
                }
            }

            // Apply smoothing if enabled
            if (applySmoothing)
            {
                heightmap = TerrainBasedGeneration.Utilities.TerrainUtility.SmoothHeightmap(
                    heightmap, smoothingRadius, smoothingIterations
                );
            }

            return heightmap;
        }
    }
}
