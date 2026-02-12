namespace Sober.ECS.Components
{
    public struct GravityComponent
    {
        public float Strength;

        public GravityComponent(float strength)
        {
            //TODO: hardcoded
            Strength = 50f;
        }
    }
}
