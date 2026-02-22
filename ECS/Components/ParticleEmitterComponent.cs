using OpenTK.Mathematics;

namespace Sober.ECS.Components
{
    public struct ParticleEmitterComponent
    {
        public bool Enabled;
        public float Rate;
        public float Accumulator;
        public float LifeMin;
        public float LifeMax;
        public float SizeMin;
        public float SizeMax;
        public Vector4 ColorMin;
        public float SpeedMin;
        public float SpeedMax;


        public ParticleEmitterComponent(float rate, float lifeMin, float lifeMax, float speedMin, float speedMax, float sizeMin, float sizeMax, Vector4 color)
        {
            Enabled = true;
            Rate = rate;
            Accumulator = 0f;
            LifeMin = lifeMin;
            LifeMax = lifeMax;
            SizeMin = sizeMin;
            SizeMax = sizeMax;
            ColorMin = color;
            SpeedMin = speedMin;
            SpeedMax = speedMax;
        }
    }
}
