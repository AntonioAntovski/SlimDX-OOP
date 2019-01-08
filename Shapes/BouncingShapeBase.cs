using SlimDX;
using SlimDX.Direct3D11;

namespace Shapes
{
    public abstract class BouncingShapeBase : ShapeWithTextureBase, IBouncing
    {
        public bool Bouncing { get; set; }
        public bool BounceTranslate { get; set; }
        public float BounceFloor { get; set; }
        public float BounceCeiling { get; set; }
        public float StepY { get; set; }
        public int Gravity { get; set; }
        public float TranslateTarget { get; set; }
        public float StepZ { get; set; }

        public virtual void Bounce(float floor, float ceiling)
        {
            if (floor != ceiling)
            {
                Bouncing = true;
                BounceFloor = floor;
                BounceCeiling = ceiling;

                ShapeWorld = Matrix.Translation(ShapeWorld.M41, StepY, ShapeWorld.M43);
                StepY += Gravity * 0.01f;

                if (ShapeWorld.M42 < floor || ShapeWorld.M42 > ceiling)
                {
                    Gravity = -Gravity;
                    switch (Gravity)
                    {
                        case 1:
                            StepY = floor;
                            break;
                        case -1:
                            StepY = ceiling;
                            break;
                    }
                }
            }
        }

        public virtual void BounceAndTranslate(float floor, float ceiling, float translateTarget)
        {
            BounceTranslate = true;
            BounceFloor = floor;
            BounceCeiling = ceiling;
            TranslateTarget = translateTarget;

            ShapeWorld = Matrix.Translation(ShapeWorld.M41, StepY, StepZ);
            StepY += Gravity * 0.01f;

            if (ShapeWorld.M42 < floor || ShapeWorld.M42 > ceiling)
            {
                Gravity = -Gravity;
                switch (Gravity)
                {
                    case 1:
                        StepY = floor;
                        break;
                    case -1:
                        StepY = ceiling;
                        break;
                }
            }

            if (ShapeWorld.M43 > translateTarget)
            {
                StepZ = translateTarget;
                BounceTranslate = false;
                ShapeWorld.set_Rows(3, new Vector4(ShapeWorld.M41, floor, ShapeWorld.M43, ShapeWorld.M44));
            }
            else
            {
                StepZ += 0.01f;
            }
        }

        public override void SetMaterial(Color4 ambient, Color4 diffuse, Color4 specular) => base.SetMaterial(ambient, diffuse, specular);

        public override void SetTexture(Device device, string path) => base.SetTexture(device, path);
    }
}
