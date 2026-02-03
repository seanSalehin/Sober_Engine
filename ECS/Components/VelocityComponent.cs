using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;


namespace Sober.ECS.Components
{
    public struct VelocityComponent
    {
        //speed of our entity
        public Vector2 Velocity;
        public float Speed;

        public VelocityComponent( float speed)
        {
            Velocity = Vector2.Zero; ;
            Speed = speed;
        }
    }
}
