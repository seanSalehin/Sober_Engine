namespace Sober.ECS.Components
{
    public struct CollisionStateComponent
    {
        public HashSet<int> Current;

        public static CollisionStateComponent Create()
        {
            //stores the IDs of entities that are currently colliding with this entity
            return new CollisionStateComponent
            {
                Current = new HashSet<int>()
            };
        }
    }
}
