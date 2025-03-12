using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace MeshGeneration.Settings
{
    [Serializable]
    public class ProceduralTreeSettings
    {
        [FoldoutGroup("Trunk Settings")] [SerializeField, MinMaxSlider(1, 5f)]
        private Vector2 trunkHeight = new Vector2(1,2);

        [FoldoutGroup("Leaf Settings")] [SerializeField, MinMaxSlider(-0.1f, 0.1f)]
        private Vector2 leafScaleWidth = new Vector2(-0.05f, 0.05f);
        
        [FoldoutGroup("Leaf Settings")] [SerializeField, MinMaxSlider(-0.1f, 0.1f)]
        private Vector2 leafScaleHeight = new Vector2(-0.05f, 0.05f);

        [FoldoutGroup("Generation Settings")] [SerializeField]
        private int randomSeed = 12345;

        [FoldoutGroup("Material Settings")] [SerializeField]
        private Material barkMaterial;

        [FoldoutGroup("Material Settings")] [SerializeField]
        private Material leafMaterial;

        public Vector2 TrunkHeight => trunkHeight;

        public Vector2 LeafScaleWidth => leafScaleWidth;

        public Vector2 LeafScaleHeight => leafScaleHeight;
        
        public int RandomSeed => randomSeed;

        public Material BarkMaterial => barkMaterial;

        public Material LeafMaterial => leafMaterial;
    }
}