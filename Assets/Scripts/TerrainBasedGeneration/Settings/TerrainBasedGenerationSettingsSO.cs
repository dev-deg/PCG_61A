using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TerrainBasedGeneration.Settings;
using UnityEngine;

[CreateAssetMenu(fileName="TerrainSettings", menuName="TerrainBasedGeneration/TerrainBasedGenerationSettingsSO", order=0)]
public class TerrainBasedGenerationSettingsSO : ScriptableObject
{
    [SerializeField, HideLabel, BoxGroup("Terrain GenerationSettings")]
    private TerrainBasedGenerationSettings settings = new TerrainBasedGenerationSettings();
    public TerrainBasedGenerationSettings Settings => settings;
}
