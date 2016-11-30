using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace WpfDx.Model
{
    internal class MeshHelper
    {
        public void Convert(out Vector3[] vertices, out int[] faces, Vector3[] mesh)
        {
            var faces_list = new List<int>();
            var dict = new Dictionary<Vector3, int>();
            var latest_index = 0;
            foreach (var vertex in mesh)
            {
                int vertex_index;
                if (!dict.TryGetValue(vertex, out vertex_index))
                {
                    vertex_index = latest_index++;
                    dict[vertex] = vertex_index;
                }

                faces_list.Add(vertex_index);
            }
            vertices = dict.Keys.ToArray();
            faces = faces_list.ToArray();
        }

        public void Convert(out double[] vertices, out int[] faces, Vector3[] mesh)
        {
            Vector3[] vert;
            Convert(out vert, out faces, mesh);

            vertices = vert.SelectMany(v => new [] {(double)v.X, v.Y, v.Z}).ToArray();
        }

        // here we have some duplication with meshbody
        public Vector3[] ComputeFaceNormals(Vector3[] mesh)
        {
            var face_normals = new Vector3[mesh.Length / 3];
            for (var i = 0; i < face_normals.Length; ++i)
            {
                var v1 = mesh[i * 3 + 0];
                var v2 = mesh[i * 3 + 1];
                var v3 = mesh[i * 3 + 2];
                face_normals[i] = Vector3.Normalize(Vector3.Cross(v2 - v1, v3 - v2));
            }
            return face_normals;
        }

        public Vector3[] ComputeVertexNormals(int[] faces, Vector3[] face_normals)
        {
            // 1. find what faces contain a certain vertex
            var dict = new Dictionary<int, List<int>>();
            for (var face_index = 0; face_index < faces.Length / 3; face_index++)
            {
                var vertex_index_0 = faces[face_index * 3 + 0];
                var vertex_index_1 = faces[face_index * 3 + 1];
                var vertex_index_2 = faces[face_index * 3 + 2];
                if (!dict.ContainsKey(vertex_index_0))
                    dict.Add(vertex_index_0, new List<int>());
                if (!dict.ContainsKey(vertex_index_1))
                    dict.Add(vertex_index_1, new List<int>());
                if (!dict.ContainsKey(vertex_index_2))
                    dict.Add(vertex_index_2, new List<int>());
                dict[vertex_index_0].Add(face_index);
                dict[vertex_index_1].Add(face_index);
                dict[vertex_index_2].Add(face_index);
            }
            // 2. Calculate average normal of all face normals around each vertex
            return dict.Values.Select(
                ff => ff.Select(
                    f => face_normals[f]).ToArray().Aggregate((a, b) => a + b)).ToArray();
        }
    }
}
