using OpenTK.Windowing.GraphicsLibraryFramework;
using Sober.ECS.Components;
using Sober.Engine.Input;
using OpenTK.Mathematics;



namespace Sober.ECS.Systems
{
    public sealed class InputSystem : ISystem
    {
        //keyboard/mouse/gamepad
        public readonly World _world;

        public InputSystem(World world)
        {
            _world = world;
        }

        public void Render()
        {
            //TODO
        }

        public void Update(float dt)
        {
            var tStore = _world.GetStore<TransformComponent>();
            var vStore = _world.GetStore<VelocityComponent>();
            var pStore = _world.GetStore<PlayerTag>();

            //Find all entities with PlayerTag + Transform +  and Velocity
            foreach (var kvp in pStore.All())
            {
                int id = kvp.Key;
                if (!tStore.Has(id) || !vStore.Has(id))
                {
                    continue;
                }
                var  vel = vStore.Get(id);
                Vector2 dir = Vector2.Zero;

                //WASD + Arrow Keys

                if(Input.Down(Keys.W) || Input.Down(Keys.Up))
                {
                    dir.Y += 1;
                }
                if(Input.Down(Keys.S) || Input.Down(Keys.Down))
                {
                    dir.Y -= 1;
                }
                if(Input .Down(Keys.A) || Input.Down(Keys.Left))
                {
                    dir.X -= 1;
                }
                if(Input.Down(Keys.D) || Input.Down(Keys.Right))
                {
                    dir.X += 1;
                }

                //diagonal speed = straight speed
                if (dir.LengthSquared > 0)
                {
                    dir = Vector2.Normalize(dir);
                }

                //converts direction into actual movement speed.
                vel.Velocity = dir * vel.Speed;
                vStore.Set(id, vel);
            }
        }
    }
}
