//Queries renderable entities/ Binds shaders, meshes, textures
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sober.ECS.Components;
using Sober.Rendering;

namespace Sober.ECS.Systems
{
    public sealed class RenderSystem : ISystem
    {
        private readonly World _world;
        private readonly Render _render;
        private readonly SpriteRenderer _spriteRenderer;


        public RenderSystem(World world, Render render, SpriteRenderer spriteRenderer)
        {
            _world = world;
            _render = render;
            _spriteRenderer = spriteRenderer;

        }

        public void Render()
        {
            _render.BeginFrame();

            //Draw Mesh
            foreach (int id in Query.with<TransformComponent, MeshRendererComponent>(_world))
            {
                var tStore = _world.GetStore<TransformComponent>();
                var mStore = _world.GetStore<MeshRendererComponent>();
                var t = tStore.Get(id);
                var m = mStore.Get(id);
                _render.Draw(m.Mesh, m.Shader, t.WorldMatrix);
            }

            //Draw Sprites
            foreach(int id in Query.with<TransformComponent, SpriteComponent>(_world))
            {
                var tStore = _world.GetStore<TransformComponent>();
                var sStore = _world.GetStore<SpriteComponent>();
                var t = tStore.Get(id);
                var s = sStore.Get(id);
                _spriteRenderer.Draw(s.Texture, t.WorldMatrix);
                _spriteRenderer.Draw(s.Texture, t.WorldMatrix);
            }
        }

        public void Update(float dt)
        {
        }
    }
}
