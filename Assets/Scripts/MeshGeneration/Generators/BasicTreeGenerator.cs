// BasicTreeGenerator.cs
using UnityEngine;
using MeshGeneration.Settings;
using MeshGeneration.Utilities;
using Sirenix.OdinInspector;

namespace MeshGeneration.Generators
{
    
    [ExecuteAlways]
    public class BasicTreeGenerator : MonoBehaviour
    {
        [SerializeField] private ProceduralTreeSettings settings;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private System.Random _rng;
        private void Awake() => EnsureComponents();
        private void OnValidate() => EnsureComponents();

        private void EnsureComponents()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        [Button("Spawn Trees")]
        public void GenerateTree()
        {
            ClearTrees();
            _rng = new System.Random(settings.RandomSeed);
            GameObject procTree = GenerateProcedureTree();
            procTree.transform.parent = transform;
        }
        
        public GameObject GenerateProcedureTree()
        {
            GameObject treeParent = new GameObject("Procedural Tree");
            
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trunk.name = "Trunk";
            trunk.transform.SetParent(treeParent.transform);
            float trunkHeight = GetRandomFloatInRange(settings.TrunkHeight);
            trunk.transform.localScale = new Vector3(0.3f, trunkHeight, 0.3f);
            trunk.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            if (settings.BarkMaterial != null)
            {
                trunk.GetComponent<Renderer>().sharedMaterial = settings.BarkMaterial;
            }

            float leafBasePosition = trunk.transform.localPosition.y + 0.5f;
            //spawn 3 leave blocks
            for (int i = 0; i < 3; i++)
            {
                //To ensurer that the leave are scaling in size
                float baseXZ = 0.9f - (0.2f * i) + GetRandomFloatInRange(settings.LeafScaleWidth);
                float baseY = 0.6f - (0.1f * i) + GetRandomFloatInRange(settings.LeafScaleHeight);
                
                GameObject leaf = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leaf.name = $"Leaf_{i}";
                leaf.transform.SetParent(treeParent.transform);
                leaf.transform.localScale = new Vector3(baseXZ, baseY, baseXZ);

                float halfLeafHeight = baseY * 0.5f;
                float centerY = leafBasePosition + halfLeafHeight;
                leaf.transform.localPosition = new Vector3(0f, centerY, 0f);
                
                //updates the current leaf spawn position
                leafBasePosition = centerY + halfLeafHeight;
                
                if (settings.LeafMaterial != null)
                {
                    leaf.GetComponent<Renderer>().sharedMaterial = settings.LeafMaterial;
                }
            }
            
            return treeParent;
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
        
        
        [Button("Export Tree to OBJ")]
        #if UNITY_EDITOR
        private void ExportTreeToOBJ()
        {
            MeshExporter.ExportTreeToOBJ(gameObject);
        }
        #endif
    }
}