//Groups systems  (Render, PhysicsGroup)/ Controls update order 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sober.ECS.Systems
{
    public sealed class SystemGroup : ISystem
    {
        private readonly List<ISystem> _systems = new();
        public void Add(ISystem system)
        {
            _systems.Add(system);
        }
        public void Render()
        {
            foreach (var system in _systems)
            {
                system.Render();
            }
        }

        public void Update(float dt)
        {
            foreach(var system in _systems)
            {
                system.Update(dt);
            }
        }
    }
}
