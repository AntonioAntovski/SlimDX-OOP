using Core;
using SlimDX;
using SlimDX.Direct3D11;

namespace Shapes
{
    interface IMaterialAndTexture
    {
        Material ShapeMaterial { get; set; }

        ShaderResourceView ShapeTexture { get; set; }

        void SetTexture(Device device, string path);

        void SetMaterial(Color4 ambient, Color4 diffuse, Color4 specular);
    }
}
