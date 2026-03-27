namespace Sober.ECS.Components
{
    public struct ParallaxLayerComponent
    {
        public float Factor;
        public string TextureKey;

        public ParallaxLayerComponent(float factor, string textureKey)
        {
            Factor = factor;
            TextureKey = textureKey;
        }
    }
}
