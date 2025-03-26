using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TerrainBasedGeneration.Settings;
using UnityEngine;

namespace TerrainBasedGeneration.Generators
{
    [ExecuteAlways]
    public class TerrainBasedGenerator : MonoBehaviour
    {
        [SerializeField, InlineEditor]
        private TerrainBasedGenerationSettingsSO settings;
    }
}