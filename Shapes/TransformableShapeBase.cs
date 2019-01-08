using System;
using SlimDX;

namespace Shapes
{
    public abstract class TransformableShapeBase : ShapeWithTextureBase, ITransformable
    {
        public virtual void Rotate(Vector3 vector)
        {
            var rX = Matrix.RotationX(vector.X);
            var rY = Matrix.RotationX(vector.Y);
            var rZ = Matrix.RotationX(vector.Z);

            ShapeWorld *= rX * rY * rZ;
        }

        public virtual void Transform(Matrix m)
        {
            ShapeWorld *= m;
        }

        public virtual void Translate(Vector3 vector)
        {
            ShapeWorld = Matrix.Translation(vector);
        }
    }
}
