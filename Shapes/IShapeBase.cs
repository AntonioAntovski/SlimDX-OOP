using SlimDX;
using SlimDX.Direct3D11;

namespace Shapes
{
    public interface IShapeBase
    {
        Buffer VertexBuffer { get; set; }
        Buffer IndexBuffer { get; set; }
        int VertexCount { get; set; }
        int IndexCount { get; set; }       
        Matrix ShapeWorld { get; set; }

        void BuildShapeBuffers(Device device);
    }
}
