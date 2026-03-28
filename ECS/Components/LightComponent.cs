using OpenTK.Mathematics;

namespace Sober.ECS.Components
{
    public  struct LightComponent
    {
        public Vector2 Offset;
        public Vector3 Color;
        public float Radius;
        public float Intensity;

        public LightComponent( Vector3 color, float radius, float intensity)
        {
            Offset = Vector2.Zero;
            Color= color;
            Radius = radius;
            Intensity = intensity;
        }
    }
}
