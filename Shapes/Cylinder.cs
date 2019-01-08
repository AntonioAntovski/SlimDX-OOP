using System.Linq;
using Core.Vertex;
using SlimDX;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;
using static Core.GeometryGenerator;

namespace Shapes
{
    public class Cylinder : AnimatedShape
    {
        private float bottomRadius;
        private float topRadius;
        private float height;
        private int sliceCount;
        private int stackCount;

        public Cylinder(Device device, float br, float tr, float h, int slice, int stack, float x, float y, float z)
        {
            bottomRadius = br;
            topRadius = tr;
            height = h;
            sliceCount = slice;
            stackCount = stack;

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
            var cylinder = CreateCylinder(bottomRadius, topRadius, height, sliceCount, stackCount);

            VertexCount = cylinder.Vertices.Count;
            IndexCount = cylinder.Indices.Count;

            var vertices = cylinder.Vertices.Select(v => new Basic32(v.Position, v.Normal, v.TexC)).ToList();
            var indices = cylinder.Indices;

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
