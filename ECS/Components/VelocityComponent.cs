using OpenTK.Mathematics;


namespace Sober.ECS.Components
{
    public struct VelocityComponent
    {
        //speed of our entity
        public Vector2 Velocity;
        public VelocityComponent(Vector2 v) => Velocity = v;
    
}
}
