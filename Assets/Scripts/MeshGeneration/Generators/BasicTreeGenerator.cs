using System;
using UnityEngine;
using MeshGeneration.Settings;
using Sirenix.OdinInspector;

namespace MeshGeneration.Generators
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class BasicTreeGenerator : MonoBehaviour
    {
        [SerializeField] private ProceduralTreeSettings settings;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private System.Random _random;

        private void Awake()
        {
            EnsureComponents();
        }

        private void OnValidate()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void EnsureComponents()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        [Button("Generate Tree")]
        public void GenerateTree()
        {
            _random = new System.Random(settings.RandomSeed);
            
            // Create separate meshes for trunk and leaves
            Mesh trunkMesh = GenerateTrunkMesh();
            Mesh[] leafMeshes = GenerateLeafMeshes();

            // Combine all meshes
            CombineInstance[] combines = new CombineInstance[1 + leafMeshes.Length];
            combines[0].mesh = trunkMesh;
            combines[0].transform = Matrix4x4.identity;

            for (int i = 0; i < leafMeshes.Length; i++)
            {
                combines[i + 1].mesh = leafMeshes[i];
                combines[i + 1].transform = Matrix4x4.identity;
            }

            Mesh finalMesh = new Mesh();
            finalMesh.CombineMeshes(combines, false);
            
            _meshFilter.mesh = finalMesh;
            
            // Set materials
            _meshRenderer.sharedMaterials = new[] { settings.BarkMaterial, settings.LeafMaterial };
        }

        private Mesh GenerateTrunkMesh()
        {
            const int segments = 8;
            float radius = 0.5f;
            
            Vector3[] vertices = new Vector3[(segments + 1) * 2];
            int[] triangles = new int[segments * 6];
            
            // Create vertices
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * (2 * Mathf.PI / segments);
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                
                vertices[i] = new Vector3(x, 0, z);
                vertices[i + segments + 1] = new Vector3(x, settings.TrunkHeight, z);
            }
            
            // Create triangles
            int tri = 0;
            for (int i = 0; i < segments; i++)
            {
                triangles[tri++] = i;
                triangles[tri++] = i + segments + 1;
                triangles[tri++] = (i + 1) % segments;
                
                triangles[tri++] = i + segments + 1;
                triangles[tri++] = ((i + 1) % segments) + segments + 1;
                triangles[tri++] = (i + 1) % segments;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }

        private Mesh[] GenerateLeafMeshes()
        {
            const int leafCount = 12;
            Mesh[] leafMeshes = new Mesh[leafCount];
            
            for (int i = 0; i < leafCount; i++)
            {
                float height = (float)_random.NextDouble() * settings.TrunkHeight * 0.7f + settings.TrunkHeight * 0.3f;
                float scale = Mathf.Lerp(settings.LeafScale.x, settings.LeafScale.y, (float)_random.NextDouble());
                float angle = (float)_random.NextDouble() * 360f;
                
                Vector3[] vertices = new Vector3[4];
                vertices[0] = new Vector3(-0.5f, 0, 0) * scale;
                vertices[1] = new Vector3(0.5f, 0, 0) * scale;
                vertices[2] = new Vector3(-0.5f, 1, 0) * scale;
                vertices[3] = new Vector3(0.5f, 1, 0) * scale;

                int[] triangles = { 0, 2, 1, 2, 3, 1 };
                Vector2[] uvs = {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1)
                };

                Mesh leafMesh = new Mesh();
                leafMesh.vertices = vertices;
                leafMesh.triangles = triangles;
                leafMesh.uv = uvs;
                
                // Transform the leaf
                Vector3 position = new Vector3(0, height, 0);
                Quaternion rotation = Quaternion.Euler(30f, angle, 0);
                Vector3[] newVertices = new Vector3[vertices.Length];
                for (int j = 0; j < vertices.Length; j++)
                {
                    newVertices[j] = rotation * vertices[j] + position;
                }
                leafMesh.vertices = newVertices;
                leafMesh.RecalculateNormals();
                
                leafMeshes[i] = leafMesh;
            }

            return leafMeshes;
        }

        [Button("Clear Tree")]
        public void ClearTree()
        {
            if (_meshFilter != null && _meshFilter.sharedMesh != null)
            {
                DestroyImmediate(_meshFilter.sharedMesh);
            }
        }
    }
}