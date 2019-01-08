namespace Shapes
{
    public interface IBouncing
    {
        bool Bouncing { get; set; }
        bool BounceTranslate { get; set; }
        float BounceFloor { get; set; }
        float BounceCeiling { get; set; }
        float TranslateTarget { get; set; }
        float StepY { get; set; }
        float StepZ { get; set; }
        int Gravity { get; set; }


        void Bounce(float floor, float ceiling);

        void BounceAndTranslate(float floor, float ceiling, float translateTarget);
    }
}
