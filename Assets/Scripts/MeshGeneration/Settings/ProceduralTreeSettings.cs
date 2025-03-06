using UnityEngine;
using Sirenix.OdinInspector;

namespace MeshGeneration.Settings
{
    public class ProceduralTreeSettings : MonoBehaviour
    {
        [FoldoutGroup("Trunk Settings")] [SerializeField, Range(1f, 20f)]
        private float trunkHeight = 5f;

        [FoldoutGroup("Leaf Settings")] [SerializeField, MinMaxSlider(0.1f, 2f)]
        private Vector2 leafScale = new Vector2(0, 1f);

        [FoldoutGroup("Generation Settings")] [SerializeField]
        private int randomSeed = 12345;

        [FoldoutGroup("Material Settings")] [SerializeField]
        private Material barkMaterial;

        [FoldoutGroup("Material Settings")] [SerializeField]
        private Material leafMaterial;

        public float TrunkHeight => trunkHeight;

        public Vector2 LeafScale => leafScale;

        public int RandomSeed => randomSeed;

        public Material BarkMaterial => barkMaterial;

        public Material LeafMaterial => leafMaterial;
    }
}