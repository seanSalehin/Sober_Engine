using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sober.ECS.Components;
using Sober.Engine.Core;

namespace Sober.ECS.Systems
{
    public sealed class MovementSystem : ISystem
    {

        private readonly World _world;

        public MovementSystem(World world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
        }

        public void Render()
        {
            //TODO
        }

        public void Update(float dt)
        {
            var tStore = _world.GetStore<TransformComponent>();
            var vStore = _world.GetStore<VelocityComponent>();

            foreach (int id in Query.with<TransformComponent, VelocityComponent>(_world))
            {
                var t = tStore.Get(id);
                var v = vStore.Get(id);
                t.LocalPosition += v.Velocity * Time.DeltaTime;
                t.Dirty = true;
                tStore.Set(id, t);
            }
        }
            
        }
    }

