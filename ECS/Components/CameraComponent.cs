using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Sober.ECS.Components
{
    public struct CameraComponent
    {
        public Vector2 Position;
        public float Zoom;
        public float Rotation;
        public float Size;

        public Matrix4 ViewProj;
        public bool Dirty;
        public static CameraComponent Default()
        {
            return new CameraComponent
            {
                Position = Vector2.Zero,
                Zoom = 1f,
                Rotation = 0f,
                Size = 1.2f,
                ViewProj = Matrix4.Identity,
                Dirty = true
            };
        }
    }
}
