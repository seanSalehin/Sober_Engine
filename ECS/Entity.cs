// only to attach components
namespace Sober.ECS
{
    public readonly struct Entity
    {
        public readonly int Id;
        public readonly int Version;
        public Entity(int id, int version)
        {
            Id = id;
            Version = version;
        }
        public override string ToString() => $"Entity(Id: {Id}, Version: {Version})";
    }
}
