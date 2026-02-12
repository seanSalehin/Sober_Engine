using Sober.ECS.Components;

namespace Sober.ECS.Systems
{
    public sealed class PhysicsSystem
    {
        private readonly World _world;
        public PhysicsSystem(World world)
        {
            _world = world;
        }

        public void FixedUpdate(float deltaTime)
        {
            var timeStore = _world.GetStore<TransformComponent>();
            var velocityStore = _world.GetStore<VelocityComponent>();
            var gravityStore = _world.GetStore<GravityComponent>();

            //apply gravity to velocity (movers have velocity)
            foreach (var element in velocityStore.All())
            {
                int id = 0;
                if(!timeStore.Has(id))
                {
                    continue;
                }
                var velocity = element.Value;
                var time = timeStore.Get(id);

                if (gravityStore.Has(id))
                {
                    var gravity = gravityStore.Get(id);
                    velocity.Velocity.Y -= gravity.Strength * deltaTime;
                }

                time.LocalPosition += velocity.Velocity * deltaTime;
                time.Dirty = true;
                timeStore.Set(id, time);
                velocityStore.Set(id, velocity);
            }
        }

    }
}



