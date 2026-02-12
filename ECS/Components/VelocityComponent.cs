using OpenTK.Mathematics;


namespace Sober.ECS.Components
{
    public struct VelocityComponent
    {
        //speed of the entity
        public Vector2 Velocity;
        public float Speed;
        public VelocityComponent(float speed)
        {
            Velocity = Vector2.Zero;
            Speed = speed;
        }

    }
}
