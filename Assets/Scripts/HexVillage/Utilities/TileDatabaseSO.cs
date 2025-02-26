using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HexVillage.Utilities
{

    [CreateAssetMenu(fileName = "TileDatabase", menuName = "ScriptableObjects/TileDatabase", order = 1)]
    public class TileDatabaseSO : ScriptableObject
    {
        [FoldoutGroup("Tiles")] [ListDrawerSettings(Expanded = true)]
        public List<GameObject> TerrainTiles = new List<GameObject>();

        [FoldoutGroup("Tiles")] [ListDrawerSettings(Expanded = true)]
        public List<GameObject> UnitTiles = new List<GameObject>();

        [FoldoutGroup("Tiles")] [ListDrawerSettings(Expanded = true)]
        public List<GameObject> RiverTiles = new List<GameObject>();

        [FoldoutGroup("Tiles")] [ListDrawerSettings(Expanded = true)]
        public List<GameObject> PathTiles = new List<GameObject>();

        [FoldoutGroup("Tiles")] [ListDrawerSettings(Expanded = true)]
        public List<GameObject> BuildingTiles = new List<GameObject>();

#if UNITY_EDITOR
        [Button("Load Prefabs")]
        public void LoadPrefabs()
        {
            // Clear existing lists
            TerrainTiles.Clear();
            UnitTiles.Clear();
            RiverTiles.Clear();
            PathTiles.Clear();
            BuildingTiles.Clear();

            // Define the folder where .fbx files are located
            string searchFolder = "Assets/HexAssets";

            // Find all GameObjects (assets) in the folder
            string[] guids = AssetDatabase.FindAssets("t:Model", new[] {searchFolder});
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                // Ensure that the asset is an FBX file
                if (path.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab != null)
                    {
                        string nameLower = prefab.name.ToLower();

                        // Check for unit, river, path, or building keywords.
                        if (nameLower.Contains("unit"))
                        {
                            UnitTiles.Add(prefab);
                        }
                        else if (nameLower.Contains("river"))
                        {
                            RiverTiles.Add(prefab);
                        }
                        else if (nameLower.Contains("path"))
                        {
                            PathTiles.Add(prefab);
                        }
                        else if (nameLower.Contains("building"))
                        {
                            BuildingTiles.Add(prefab);
                        }
                        else
                        {
                            // Anything else goes under Terrain
                            TerrainTiles.Add(prefab);
                        }
                    }
                }
            }

            // Mark the asset dirty so the changes are saved
            EditorUtility.SetDirty(this);
        }

        [Button("Export Prefabs")]
        public void ExportPrefabs()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("=== Terrain Tiles ===");
            foreach (var tile in TerrainTiles)
            {
                sb.AppendLine(tile != null ? tile.name : "null");
            }

            sb.AppendLine("\n=== Unit Tiles ===");
            foreach (var tile in UnitTiles)
            {
                sb.AppendLine(tile != null ? tile.name : "null");
            }

            sb.AppendLine("\n=== River Tiles ===");
            foreach (var tile in RiverTiles)
            {
                sb.AppendLine(tile != null ? tile.name : "null");
            }

            sb.AppendLine("\n=== Path Tiles ===");
            foreach (var tile in PathTiles)
            {
                sb.AppendLine(tile != null ? tile.name : "null");
            }

            sb.AppendLine("\n=== Building Tiles ===");
            foreach (var tile in BuildingTiles)
            {
                sb.AppendLine(tile != null ? tile.name : "null");
            }

            Debug.Log(sb.ToString());
        }
#endif
    }
}