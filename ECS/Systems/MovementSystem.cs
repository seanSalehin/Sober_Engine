using OpenTK.Windowing.GraphicsLibraryFramework;
using Sober.ECS.Components;
using Sober.Engine.Input;

namespace Sober.ECS.Systems
{
    public sealed class MovementSystem : ISystem
    {
        private readonly World _world;

        public MovementSystem(World world)
        {
            _world = world;
        }

        public void Update(float dt)
        {
            var playerStore = _world.GetStore<PlayerTag>();
            var velocityStore = _world.GetStore<VelocityComponent>();
            var movementStore = _world.GetStore<PlayerMovementComponent>();

            foreach (var element in playerStore.All())
            {
                int id = element.Key;

                if (!velocityStore.Has(id) || !movementStore.Has(id))
                {
                    continue;
                }

                var velocity = velocityStore.Get(id);
                var movement = movementStore.Get(id);

                float move = 0f;

                if (Input.Down(Keys.A))
                {
                    move -= 1f;
                }

                if (Input.Down(Keys.D))
                {
                    move += 1f;
                }

                velocity.Velocity.X = move * movement.MoveSpeed;

                if (Input.Down(Keys.Space) && movement.IsGrounded)
                {
                    velocity.Velocity.Y = movement.JumpForce;
                    movement.IsGrounded = false;
                }

                velocityStore.Set(id, velocity);
                movementStore.Set(id, movement);
            }
        }

        public void Render()
        {
        }
    }
}