using Unity.Mathematics;

namespace Saivs.Animation
{
    public struct KeyfameData
    {
        public int LeftKeyframeIndex;
        public int RightKeyframeIndex;
        public float Weight;

        public KeyfameData(float time, float duration, float sampleRate)
        {
            float sampleIndex = math.clamp(time, 0, duration) * sampleRate;

            LeftKeyframeIndex = (int)math.floor(sampleIndex);
            RightKeyframeIndex = (int)math.ceil(sampleIndex);

            Weight = math.frac(sampleIndex);
        }
    }
}
