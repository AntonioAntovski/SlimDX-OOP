using System.Linq;
using Core;
using Core.Vertex;
using SlimDX;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;
using static Core.GeometryGenerator;

namespace Shapes
{
    public class Grid : TransformableShapeBase
    {
        private float width;
        private float depth;
        private int m;
        private int n;

        public Grid(Device device, float w, float d, int rows, int cols, float x, float y, float z)
        {
            width = w;
            depth = d;
            m = rows;
            n = cols;

            ShapeWorld = Matrix.Translation(x, y, z);

            BuildShapeBuffers(device);
        }

        public override void SetMaterial(Color4 ambient, Color4 diffuse, Color4 specular) => base.SetMaterial(ambient, diffuse, specular);

        public override void SetTexture(Device device, string path) => base.SetTexture(device, path);

        public override void Rotate(Vector3 vector) => base.Rotate(vector);

        public override void Transform(Matrix m) => base.Transform(m);

        public override void Translate(Vector3 vector) => base.Translate(vector);

        public override void BuildShapeBuffers(Device device)
        {
            var grid = CreateGrid(width, depth, m, n);

            VertexCount = grid.Vertices.Count;
            IndexCount = grid.Indices.Count;

            var vertices = grid.Vertices.Select(v => new Basic32(v.Position, v.Normal, v.TexC)).ToList();
            var indices = grid.Indices;

            var vbd = new BufferDescription(
                Basic32.Stride * VertexCount,
                ResourceUsage.Default,
                BindFlags.VertexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0);
            VertexBuffer = new Buffer(device, new DataStream(vertices.ToArray(), false, false), vbd);

            var ibd = new BufferDescription(
                sizeof(int) * IndexCount,
                ResourceUsage.Default,
                BindFlags.IndexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0);
            IndexBuffer = new Buffer(device, new DataStream(indices.ToArray(), false, false), ibd);
        }
    }
}
