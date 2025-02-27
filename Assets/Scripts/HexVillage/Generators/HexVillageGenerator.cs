using Sirenix.OdinInspector;
using UnityEngine;

namespace HexVillage.Generators
{
    [RequireComponent(typeof(HexGridGenerator))]
    [ExecuteAlways]
    public class HexVillageGenerator : MonoBehaviour
    {
        [SerializeField] private HexVillageSettings settings;
        public HexGridGenerator GridGenerator { get; private set; }

        private Transform _villageParent;

        private void OnValidate()
        {
            EnsureDependencies();
        }

        private void Awake()
        {
            EnsureDependencies();
        }

        private void EnsureDependencies()
        {
            _villageParent = transform;
            GridGenerator = GetComponent<HexGridGenerator>();
            GridGenerator.Initialise(settings);
        }

        [Button("Generate Village")]
        public void GenerateVillage()
        {
            GridGenerator.GenerateGrid();
        }
    }
}