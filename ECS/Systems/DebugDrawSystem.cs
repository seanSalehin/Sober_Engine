using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sober.Engine.Input;
using Sober.ECS.Components;
using Sober.Rendering.Debug;

namespace Sober.ECS.Systems
{
    // Ensure DebugDrawSystem implements ISystem
    public sealed class DebugDrawSystem : ISystem
    {
        private readonly World _world;
        private readonly DebugDraw _debugDraw;
        public bool Enabled { get; private set; } = false;

        public DebugDrawSystem(World world, DebugDraw debugDraw)
        {
            _world = world;
            _debugDraw = debugDraw;
        }

        public void Update()
        {
            // F3 (toggle debug)
            if (Input.Down(Keys.F3))
                Enabled = !Enabled;
        }

        public void Render()
        {
            if (!Enabled)
            {
                return;
            }

            var timeStore = _world.GetStore<TransformComponent>();
            var aabbStore = _world.GetStore<AabbColliderComponent>();
            var circleStore = _world.GetStore<CircleColliderComponent>();

            // draw AABB
            foreach (var element in aabbStore.All())
            {
                int id = element.Key;

                if (!timeStore.Has(id))
                {
                    continue;
                }

                var time = timeStore.Get(id);
                var aabb = element.Value;

                //Vector4(r, g, b, a)
                Vector4 color = aabb.IsTrigger ? new Vector4(0.2f, 0.8f, 1f, 1f) : new Vector4(0f, 1f, 0f, 1f);
                _debugDraw.DrawRectangle(time.LocalPosition, aabb.Halfsize, color);

            }


            // draw circles
            foreach (var element in circleStore.All())
            {
                int id = element.Key;
                if (!timeStore.Has(id))
                {
                    continue;
                }
                var time = timeStore.Get(id);
                var circle = element.Value;
                //Vector4(r, g, b, a)
                Vector4 color = circle.IsTrigger ? new Vector4(1f, 0.5f, 0.2f, 1f) : new Vector4(1f, 0f, 1f, 1f);
                _debugDraw.DrawCircle(time.LocalPosition, circle.Radius, color);

            }
        }

        // Explicitly implement ISystem.Update(float dt)
        void ISystem.Update(float dt)
        {
            Update();
        }
    }
}
