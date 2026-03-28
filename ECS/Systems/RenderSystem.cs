//Queries renderable entities/ Binds shaders, meshes, textures
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


        private struct DrawItem
        {
            public int EntityId;
            public int Layer;
            public bool IsSprite;
        }

        public void Render()
        {
            _render.BeginFrame();

            var layerStore = _world.GetStore<RenderLayerComponent>();
            List<DrawItem> drawItems = new List<DrawItem>();
            var tStore = _world.GetStore<TransformComponent>();
            var mStore = _world.GetStore<MeshRendererComponent>();
            var sStore = _world.GetStore<SpriteComponent>();
            var animatorStore = _world.GetStore<AnimatorComponent>();


            //Draw Mesh
            foreach (int id in Query.with<TransformComponent, MeshRendererComponent>(_world))
            {
                var layer = layerStore.Has(id) ? (int)layerStore.Get(id).Layer : (int)Sober.Rendering.Layers.RenderLayers.World;
                drawItems.Add(new DrawItem
                {
                    EntityId = id,
                    Layer = layer,
                    IsSprite = false
                });
            }

            //Draw Sprites
            foreach (int id in Query.with<TransformComponent, SpriteComponent>(_world))
            {
                var layer = layerStore.Has(id) ? (int)layerStore.Get(id).Layer : (int)Rendering.Layers.RenderLayers.World;
                drawItems.Add(new DrawItem { EntityId = id, Layer = layer, IsSprite = true });
            }

            drawItems.Sort((a, b) => a.Layer.CompareTo(b.Layer));

            foreach (var item in drawItems)
            {
                if (item.IsSprite)
                {
                    var transform = tStore.Get(item.EntityId);
                    var sprite = sStore.Get(item.EntityId);

                    if (animatorStore.Has(item.EntityId))
                    {
                        var anim = animatorStore.Get(item.EntityId);
                        var clip = anim.StateMachine.CurrentClip();

                        // FORCE it to crop. If clip is null, default to frame 0.
                        int frameId = (clip != null) ? clip.Frames[anim.FrameIndex] : 0;
                        _spriteRenderer.Draw(sprite.Texture, transform.WorldMatrix, frameId);
                    }
                    else
                    {
                        _spriteRenderer.Draw(sprite.Texture, transform.WorldMatrix);
                    }
                }
                else
                {
                    var transform = tStore.Get(item.EntityId);
                    var meshRenderer = mStore.Get(item.EntityId);
                    _render.Draw(meshRenderer.Mesh, meshRenderer.Shader, transform.WorldMatrix);
                }
            }
        }




        public void Update(float dt)
        {
        }
    }
}
