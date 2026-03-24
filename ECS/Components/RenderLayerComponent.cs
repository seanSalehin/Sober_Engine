using Sober.Rendering.Layers;

namespace Sober.ECS.Components
{
    public struct RenderLayerComponent
    {
        public RenderLayers Layer;
        public RenderLayerComponent(RenderLayers layer)
        {
                       Layer = layer;
        }
    }
}
