using Core;
using SlimDX;
using SlimDX.Direct3D11;

namespace Shapes
{
    public abstract class ShapeWithTextureBase : ShapeBase, IMaterialAndTexture
    {
        public Material ShapeMaterial { get; set; }
        public ShaderResourceView ShapeTexture { get; set; }

        public virtual void SetMaterial(Color4 ambient, Color4 diffuse, Color4 specular)
        {
            ShapeMaterial = new Material
            {
                Ambient = ambient,
                Diffuse = diffuse,
                Specular = specular
            };
        }

        public virtual void SetTexture(Device device, string path)
        {
            ShapeTexture = ShaderResourceView.FromFile(device, path);
        }
    }
}
