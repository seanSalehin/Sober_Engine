namespace Sober.ECS.Components
{
    public struct GravityComponent
    {
        public float Strength;

        public GravityComponent(float strength)
        {
            Strength = strength;
        }
    }
}
