using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Helpers;
using UnityEngine;

namespace Assets
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    class BuildingPolygon : MonoBehaviour
    {
        public void Initialize(List<Vector3> verts)
        {
            GetComponent<MeshFilter>().mesh = CreateMesh(verts);
        }

        private static Mesh CreateMesh(List<Vector3> verts)
        {
            var tris = new Triangulator(verts.Select(x => x.ToVector2xz()).ToArray());
            var mesh = new Mesh();

            var vertices = verts.Select(x => new Vector3(x.x, 20f, x.z)).ToList();
            var indices = tris.Triangulate().ToList();
            //var uv = new List<Vector2>();

            var numberOfPointsPerRing = vertices.Count;
            for (int index = 0; index < numberOfPointsPerRing; index++)
            {
                var v = vertices[index];
                vertices.Add(new Vector3(v.x, 0, v.z));
            }

            for (int i = 0; i < numberOfPointsPerRing - 1; i++)
            {
                indices.Add(i);
                indices.Add(i + numberOfPointsPerRing);
                indices.Add(i + numberOfPointsPerRing + 1);
                indices.Add(i);
                indices.Add(i + numberOfPointsPerRing + 1);
                indices.Add(i + 1);
            }

            indices.Add(numberOfPointsPerRing - 1);
            indices.Add(numberOfPointsPerRing);
            indices.Add(0);

            indices.Add(numberOfPointsPerRing - 1);
            indices.Add(numberOfPointsPerRing + numberOfPointsPerRing - 1);
            indices.Add(numberOfPointsPerRing);


            //Lower pointposition hinzufügen
            foreach (Vector3 point in verts)
            {
                vertices.Add(new Vector3(point.x, 0, point.z));
            }


            int pointAbove = numberOfPointsPerRing;
            for (int i = 0; i < numberOfPointsPerRing; i++)
            {
                //right upper triangle
                indices.Add(i);             //right down
                indices.Add(i + 1 + pointAbove);//left up
                indices.Add(i + pointAbove); //right up

                //left lower triangle
                indices.Add(i + 1);             //right down
                indices.Add(i + 1 + pointAbove);//left up
                indices.Add(i); //right up

            }



            mesh.vertices = vertices.ToArray();
            mesh.triangles = indices.ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}
