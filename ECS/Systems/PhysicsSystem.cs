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

            foreach (var element in velocityStore.All())
            {
                int id = element.Key;

                if (!timeStore.Has(id))
                {
                    continue;
                }

                var velocity = element.Value;
                var time = timeStore.Get(id);

                if (gravityStore.Has(id))
                {
                    var gravity = gravityStore.Get(id);

                    float gravityForce = gravity.Strength;
                    float fallMultiplier = 3.5f;
                    float maxFallSpeed = 18f;

                    if (velocity.Velocity.Y < 0f)
                    {
                        velocity.Velocity.Y -= gravityForce * fallMultiplier * deltaTime;
                    }
                    else
                    {
                        velocity.Velocity.Y -= gravityForce * deltaTime;
                    }

                    if (velocity.Velocity.Y < -maxFallSpeed)
                    {
                        velocity.Velocity.Y = -maxFallSpeed;
                    }
                }

                time.LocalPosition += velocity.Velocity * deltaTime;
                time.Dirty = true;

                timeStore.Set(id, time);
                velocityStore.Set(id, velocity);
            }
        }
    }
}