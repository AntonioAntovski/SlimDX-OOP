using SlimDX;

namespace Shapes
{
    public interface ITransformable
    { 
        void Transform(Matrix m);

        void Translate(Vector3 vector);

        void Rotate(Vector3 vector);
    }
}
