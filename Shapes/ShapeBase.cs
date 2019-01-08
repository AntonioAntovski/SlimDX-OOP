using SlimDX;
using SlimDX.Direct3D11;

namespace Shapes
{
    public abstract class ShapeBase : IShapeBase
    {
        public Buffer VertexBuffer { get; set; }
        public Buffer IndexBuffer { get; set; }
        public int VertexCount { get; set; }
        public int IndexCount { get; set; }
        public Matrix ShapeWorld { get; set; }

        public virtual void BuildShapeBuffers(Device device) { }
    }
}
