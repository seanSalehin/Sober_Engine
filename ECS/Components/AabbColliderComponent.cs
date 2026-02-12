using OpenTK.Mathematics;

namespace Sober.ECS.Components
{
    public struct AabbColliderComponent
    {
        public Vector2 Halfsize;
        public bool IsTrigger;
        public int Layer;
        public int Mask;

        public AabbColliderComponent(Vector2 halfSize, int layer, int mask, bool isTrigger = false)
        {
            Halfsize = halfSize;
            Layer = layer;
            Mask = mask;
            IsTrigger = isTrigger;
        }
    }
}
