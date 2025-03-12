namespace MeshGeneration.Utilities
{
    using System.IO;
    using System.Text;
    using UnityEngine;
    using Sirenix.OdinInspector;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    public static class MeshExporter
    {
        [Button("Export Tree to OBJ")]
        #if UNITY_EDITOR
        public static void ExportTreeToOBJ(GameObject parent)
        {
            if (parent.transform.childCount == 0) return;

            Mesh combinedMesh = CombineChildMeshes(parent);
            combinedMesh.RecalculateNormals();

            string exportFolder = "Assets/ExportedTrees";
            if (!AssetDatabase.IsValidFolder(exportFolder))
                AssetDatabase.CreateFolder("Assets", "ExportedTrees");

            string objData = MeshToOBJString(combinedMesh);
            string path = Path.Combine(exportFolder, $"{parent.name}.obj");
            File.WriteAllText(path, objData, Encoding.UTF8);

            AssetDatabase.Refresh();
            Debug.Log($"Mesh exported to {path}");
        }
        #endif

        private static Mesh CombineChildMeshes(GameObject parent)
        {
            MeshFilter[] filters = parent.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[filters.Length];

            for (int i = 0; i < filters.Length; i++)
            {
                if (filters[i].sharedMesh == null) continue;

                // Create a temporary mesh to modify
                Mesh meshCopy = UnityEngine.Object.Instantiate(filters[i].sharedMesh);

                // Invert normals on the copy
                Vector3[] normals = meshCopy.normals;
                for (int n = 0; n < normals.Length; n++)
                    normals[n] = -normals[n];
                meshCopy.normals = normals;

                // Flip triangle winding
                int[] triangles = meshCopy.triangles;
                for (int t = 0; t < triangles.Length; t += 3)
                {
                    int temp = triangles[t];
                    triangles[t] = triangles[t + 2];
                    triangles[t + 2] = temp;
                }
                meshCopy.triangles = triangles;
                meshCopy.RecalculateNormals();

                combine[i].mesh = meshCopy;
                combine[i].transform = filters[i].transform.localToWorldMatrix;
            }

            Mesh finalMesh = new Mesh();
            finalMesh.CombineMeshes(combine, true, true);
            finalMesh.Optimize();
            return finalMesh;
        }

        private static string MeshToOBJString(Mesh mesh)
        {
            StringBuilder sb = new StringBuilder();

            // Add mesh name
            sb.Append("# Exported Mesh\n");

            // Write vertices
            foreach (Vector3 v in mesh.vertices)
                sb.AppendFormat("v {0:F6} {1:F6} {2:F6}\n", v.x, v.y, v.z);

            // Write UVs
            foreach (Vector2 uv in mesh.uv)
                sb.AppendFormat("vt {0:F6} {1:F6}\n", uv.x, uv.y);

            // Write normals
            foreach (Vector3 n in mesh.normals)
                sb.AppendFormat("vn {0:F6} {1:F6} {2:F6}\n", n.x, n.y, n.z);

            // Write faces
            sb.Append("\n# Faces\n");
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                sb.AppendFormat("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                    mesh.triangles[i] + 1,
                    mesh.triangles[i + 1] + 1,
                    mesh.triangles[i + 2] + 1);
            }

            return sb.ToString();
        }
    }
}