
namespace Sober.ECS.Components
{

    public enum BurstType
    {
        Sparks,
        Smoke,
        Flash
    }

    public struct ParticleBurstRequest
    {
        public int Count;
        public BurstType Type;
        public ParticleBurstRequest(int count, BurstType type = BurstType.Sparks)
        {
            Count = count;
            Type = type;
        }
    }
}
