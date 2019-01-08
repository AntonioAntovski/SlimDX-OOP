using System.Linq;
using Core.Vertex;
using SlimDX;
using SlimDX.Direct3D11;
using static Core.GeometryGenerator;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace Shapes
{
    public class Box : AnimatedShape
    {
        private float width;
        private float height;
        private float depth;

        public Box(Device device, float w, float h, float d, float x, float y, float z)
        {
            width = w;
            height = h;
            depth = d;

            ShapeWorld = Matrix.Translation(x, y, z);

            StepY = y;
            StepZ = z;
            Gravity = 1;

            BuildShapeBuffers(device);
        }

        public override void Bounce(float floor, float ceiling) => base.Bounce(floor, ceiling);

        public override void BounceAndTranslate(float floor, float ceiling, float translateTarget) => base.BounceAndTranslate(floor, ceiling, translateTarget);

        public override void Translate(Vector3 v) => base.Translate(v);

        public override void Transform(Matrix m) => base.Transform(m);

        public override void Rotate(Vector3 vector) => base.Rotate(vector);

        public override void BuildShapeBuffers(Device device)
        {
            var box = CreateBox(width, height, depth);

            VertexCount = box.Vertices.Count;
            IndexCount = box.Indices.Count;

            var vertices = box.Vertices.Select(v => new Basic32(v.Position, v.Normal, v.TexC)).ToList();
            var indices = box.Indices;

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

        public override void SetMaterial(Color4 ambient, Color4 diffuse, Color4 specular) => base.SetMaterial(ambient, diffuse, specular);

        public override void SetTexture(Device device, string path) => base.SetTexture(device, path);
    }
}
