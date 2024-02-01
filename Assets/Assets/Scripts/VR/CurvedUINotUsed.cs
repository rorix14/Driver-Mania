using System;
using UnityEngine;

namespace VR
{
    public class CurvedUINotUsed : MonoBehaviour
    {
        [Range(2, 512)] public int meshresolution = 10;

        [Range(5, 100)] public float curveradius = 20f;

        [Range(5, 100)] public float height = 30f;

        [Range(30, 270)] public float chordangle = 180f;

        [SerializeField, HideInInspector] MeshFilter meshFilter;

        private MeshGenerator face;

        private void OnValidate()
        {
            Initialize();
        }

        void Initialize()
        {
            if (gameObject.GetComponent<MeshRenderer>() == null)
            {
                gameObject.AddComponent<MeshRenderer>().sharedMaterial =
                    Resources.Load("Assets/Materials/CurvedUIMaterial", typeof(Material)) as Material;
            }
         
            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }

            meshFilter.sharedMesh = MeshGenerator.GenerateMesh(meshresolution, curveradius, height, chordangle);
        }
    }

    public class MeshGenerator
    {
        public static Mesh GenerateMesh(int resolution, float radius, float height, float chordangle)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[resolution * resolution];
            int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
            int triIndex = 0;
            Vector2[] uv = (mesh.uv.Length == vertices.Length) ? mesh.uv : new Vector2[vertices.Length];
            float angleStep = (float)((Math.PI / 180f) * chordangle) / resolution;
            float heightStep = height / resolution;

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    int i = x + y * resolution;
                    float angle = x * angleStep;

                    Vector2 percent = new Vector2(x, y) / (resolution - 1);
                    vertices[i] = new Vector3(radius * Mathf.Cos(angle), y * heightStep, radius * Mathf.Sin(angle));
                    uv[i] = new Vector2(percent.x, percent.y);

                    if (x != resolution - 1 && y != resolution - 1)
                    {
                        triangles[triIndex] = i;
                        triangles[triIndex + 1] = i + resolution + 1;
                        triangles[triIndex + 2] = i + resolution;

                        triangles[triIndex + 3] = i;
                        triangles[triIndex + 4] = i + 1;
                        triangles[triIndex + 5] = i + resolution + 1;
                        triIndex += 6;
                    }
                }
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.uv = uv;
            return mesh;
        }
    }
}