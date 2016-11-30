using System.Linq;
using System.Numerics;

namespace DemoAnimat.Model
{
    internal class Mesh
    {
        private readonly MeshHelper _mesh_helper = new MeshHelper();
        public Vector3[] FaceNormals { get; }
        public Vector3[] VertexNormals { get; }
        public Vector3[] MeshData { get; }
        public Vector3[] Vertices { get; }
        public int[] Faces { get; }
        public Mesh(Vector3[] mesh_data)
        {
            MeshData = mesh_data;
            FaceNormals = _mesh_helper.ComputeFaceNormals(MeshData);

            Vector3[] vertices;
            int[] faces;
            _mesh_helper.Convert(out vertices, out faces, MeshData);
            Vertices = vertices;
            Faces = faces;

            VertexNormals = _mesh_helper.ComputeVertexNormals(Faces, FaceNormals);
        }

        public double[] VerticesAsDoubleArray => Vertices.SelectMany(v => new [] {(double)v.X, v.Y, v.Z}).ToArray();
    }
}
