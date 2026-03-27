using Sober.Assets;
using Sober.ECS.Components;
using Sober.Rendering.Tilemap;

namespace Sober.ECS.Systems
{
    public sealed class ParallaxSystem:ISystem
    {
        private readonly World _world;
        private readonly ParallaxRenderer _renderer;

        public ParallaxSystem(World world, ParallaxRenderer renderer)
        {
            _world = world;
            _renderer = renderer;
        }

        public void Update(float dt) { }

        public void Render()
        {
            var parallaxStore = _world.GetStore<ParallaxLayerComponent>();

            var camStore = _world.GetStore<CameraComponent>();

            float cameraX = 0f;

            foreach (var kv in camStore.All())
            {
                cameraX = kv.Value.Position.X;
                break;
            }
                foreach (var kvp in parallaxStore.All())
            {
                var layer = kvp.Value;
                var texture = AssetManager.GetTexture(layer.TextureKey, layer.TextureKey);

                _renderer.Draw(texture, cameraX, layer.Factor);
            }
        }
    }
}