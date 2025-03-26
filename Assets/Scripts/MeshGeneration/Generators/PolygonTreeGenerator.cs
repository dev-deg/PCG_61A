using System.Collections.Generic;
using UnityEngine;
using MeshGeneration.Settings;
using Sirenix.OdinInspector;

namespace MeshGeneration.Generators
{
    [ExecuteAlways]
    public class PolygonTreeGenerator : MonoBehaviour
    {
        [SerializeField] private ProceduralTreeSettings settings;
        private System.Random _rng;
        
        private void Awake() => EnsureComponents();
        private void OnValidate() => EnsureComponents();

        private void EnsureComponents()
        {
            if (!GetComponent<MeshFilter>()) gameObject.AddComponent<MeshFilter>();
            if (!GetComponent<MeshRenderer>()) gameObject.AddComponent<MeshRenderer>();
        }

        [Button("Generate Trees")]
        public void GenerateTrees()
        {
            ClearTrees();
            _rng = new System.Random(settings.RandomSeed);

            float areaSize = Mathf.Sqrt(settings.NumberOfTrees) * 3f;
            float minDistance = 1.5f;
            var positions = new List<Vector3>();

            positions.Add(Vector3.zero);
            SpawnTreeAtPosition(Vector3.zero);

            int tries = 0;
            while (positions.Count < settings.NumberOfTrees && tries < 1000)
            {
                float x = ((float)_rng.NextDouble() * 2 - 1) * areaSize;
                float z = ((float)_rng.NextDouble() * 2 - 1) * areaSize;
                Vector3 newPos = new Vector3(x, 0, z);

                bool canPlace = true;
                foreach (var pos in positions)
                {
                    if (Vector3.Distance(newPos, pos) < minDistance)
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (canPlace)
                {
                    positions.Add(newPos);
                    SpawnTreeAtPosition(newPos);
                }
                tries++;
            }
        }

        private void SpawnTreeAtPosition(Vector3 position)
        {
            GameObject tree = CreateTreeMesh();
            tree.transform.SetParent(transform);
            tree.transform.localPosition = position;
            float rotation = (float)_rng.NextDouble() * 360f;
            tree.transform.localRotation = Quaternion.Euler(0, rotation, 0);
        }

        private GameObject CreateTreeMesh()
        {
            GameObject treeObj = new GameObject("Procedural Tree");
            
            // Create trunk
            GameObject trunkObj = new GameObject("Trunk");
            trunkObj.transform.SetParent(treeObj.transform);
            MeshFilter trunkMeshFilter = trunkObj.AddComponent<MeshFilter>();
            MeshRenderer trunkRenderer = trunkObj.AddComponent<MeshRenderer>();
            
            float trunkHeight = GetRandomFloatInRange(settings.TrunkHeight);
            trunkMeshFilter.mesh = GenerateTrunkMesh(trunkHeight);
            trunkRenderer.material = settings.BarkMaterial;

            // Create leaves
            GameObject leavesObj = new GameObject("Leaves");
            leavesObj.transform.SetParent(treeObj.transform);
            MeshFilter leavesMeshFilter = leavesObj.AddComponent<MeshFilter>();
            MeshRenderer leavesRenderer = leavesObj.AddComponent<MeshRenderer>();
            
            leavesMeshFilter.mesh = GenerateLeavesMesh(trunkHeight);
            leavesRenderer.material = settings.LeafMaterial;

            return treeObj;
        }

        private Mesh GenerateTrunkMesh(float height)
{
    Mesh mesh = new Mesh();
    int segments = 4; // Using 4 segments for more angular look
    float baseRadius = 0.15f;
    float topRadius = baseRadius * 0.7f; // Slight taper

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    // Generate trunk vertices with random offsets
    for (int i = 0; i <= segments; i++)
    {
        float angle = ((float)i / segments) * Mathf.PI * 2;
        
        // Base vertices with random offset
        float baseOffset = (float)_rng.NextDouble() * 0.05f;
        float x = Mathf.Cos(angle) * (baseRadius + baseOffset);
        float z = Mathf.Sin(angle) * (baseRadius + baseOffset);
        vertices.Add(new Vector3(x, 0, z));

        // Middle vertices with random offset
        float midHeight = height * 0.5f;
        float midOffset = (float)_rng.NextDouble() * 0.08f;
        float midRadius = Mathf.Lerp(baseRadius, topRadius, 0.5f);
        vertices.Add(new Vector3(
            Mathf.Cos(angle) * (midRadius + midOffset),
            midHeight,
            Mathf.Sin(angle) * (midRadius + midOffset)));

        // Top vertices with random offset
        float topOffset = (float)_rng.NextDouble() * 0.05f;
        vertices.Add(new Vector3(
            Mathf.Cos(angle) * (topRadius + topOffset),
            height,
            Mathf.Sin(angle) * (topRadius + topOffset)));
    }

    // Generate triangles for each quad
    for (int i = 0; i < segments; i++)
    {
        int baseIndex = i * 3;
        int nextBaseIndex = ((i + 1) % segments) * 3;

        // Lower quad
        triangles.AddRange(new[] {
            baseIndex, baseIndex + 1, nextBaseIndex,
            nextBaseIndex, baseIndex + 1, nextBaseIndex + 1
        });

        // Upper quad
        triangles.AddRange(new[] {
            baseIndex + 1, baseIndex + 2, nextBaseIndex + 1,
            nextBaseIndex + 1, baseIndex + 2, nextBaseIndex + 2
        });
    }

    mesh.vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.RecalculateNormals();
    return mesh;
}

private Mesh GenerateLeavesMesh(float trunkHeight)
{
    Mesh mesh = new Mesh();
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    float leafBaseHeight = trunkHeight * 0.7f;
    int leafLayers = 3;

    for (int layer = 0; layer < leafLayers; layer++)
    {
        float layerRadius = 1f - (layer * 0.2f);
        float layerHeight = leafBaseHeight + (layer * 0.6f);
        int segments = 4 + layer; // Increasing segments per layer

        int baseIndex = vertices.Count;

        // Center vertex with random offset
        float centerOffset = (float)_rng.NextDouble() * 0.1f;
        vertices.Add(new Vector3(0, layerHeight + 0.5f + centerOffset, 0));

        // Create vertices in a circular pattern with random offsets
        for (int i = 0; i < segments; i++)
        {
            float angle = ((float)i / segments) * Mathf.PI * 2;
            float radiusOffset = (float)_rng.NextDouble() * 0.2f;
            float heightOffset = (float)_rng.NextDouble() * 0.3f;
            
            float x = Mathf.Cos(angle) * (layerRadius + radiusOffset);
            float z = Mathf.Sin(angle) * (layerRadius + radiusOffset);
            vertices.Add(new Vector3(x, layerHeight + heightOffset, z));

            // Create triangles for each segment
            if (i < segments - 1)
            {
                triangles.AddRange(new[] {
                    baseIndex, baseIndex + i + 1, baseIndex + i + 2
                });
            }
            else
            {
                triangles.AddRange(new[] {
                    baseIndex, baseIndex + segments, baseIndex + 1
                });
            }

            // Add random subdivisions
            if (_rng.NextDouble() > 0.5f)
            {
                float midX = (x + vertices[baseIndex].x) * 0.5f;
                float midZ = (z + vertices[baseIndex].z) * 0.5f;
                float midY = layerHeight + (float)_rng.NextDouble() * 0.4f;
                
                vertices.Add(new Vector3(midX, midY, midZ));
                int midPoint = vertices.Count - 1;
                
                if (i < segments - 1)
                {
                    triangles.AddRange(new[] {
                        baseIndex + i + 1, midPoint, baseIndex + i + 2
                    });
                }
            }
        }
    }

    mesh.vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.RecalculateNormals();
    return mesh;
}

        private float GetRandomFloatInRange(Vector2 range)
        {
            return (float)(_rng.NextDouble() * (range.y - range.x) + range.x);
        }

        [Button("Clear Trees")]
        public void ClearTrees()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
}