//updare transform and handle parent/chile transform, Computes world matrices
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Sober.ECS.Components;


namespace Sober.ECS.Systems
{
    public sealed class TransformSystem : ISystem
    {
        private readonly World _world;

        public TransformSystem(World world)
        {
            _world = world;
        }

        public void Render()
        {
        }

        public void Update(float dt)
        {
            var store = _world.GetStore<TransformComponent>();
            foreach(var s in store.All())
            {
                int id = s.Key;
                var t = s.Value;

                if (!t.Dirty) continue;

                t.LocalMatrix = Matrix4.CreateScale(t.LocalScale.X, t.LocalScale.Y, 1f) *
                                Matrix4.CreateRotationZ(t.LocalRotation) *
                                Matrix4.CreateTranslation(t.LocalPosition.X, t.LocalPosition.Y, 0f);

                if(t.ParentEntityId != 0 && store.Has(t.ParentEntityId))
                {
                    var parent = store.Get(t.ParentEntityId);
                    t.WorldMatrix = t.LocalMatrix * parent.WorldMatrix;
                }
                else
                {
                    t.WorldMatrix = t.LocalMatrix;
                }
                t.Dirty = false;
                store.Set(id, t);
            }
        }
    }
}
