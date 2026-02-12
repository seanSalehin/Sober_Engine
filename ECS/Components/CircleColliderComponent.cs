namespace Sober.ECS.Components
{
    public struct CircleColliderComponent
    {
        public float Radius;
        public bool IsTrigger;
        public int Layer;
        public int Mask;
        public CircleColliderComponent(float radius, int layer, int mask, bool isTrigger = false)
        {
            Radius = radius;
            Layer = layer;
            Mask = mask;
            IsTrigger = isTrigger;
        }
    }
}
