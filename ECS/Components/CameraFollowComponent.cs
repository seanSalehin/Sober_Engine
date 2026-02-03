using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Sober.ECS.Components
{
    public struct CameraFollowComponent
    {
        public int TargetEntity;
        public float Smoothing;
        public Vector2 DeadZone;
        public  CameraFollowComponent (int targetEntity)
        {
           TargetEntity = targetEntity;
              Smoothing = 8f;
              DeadZone = new Vector2(0.1f, 0.1f);
        }
    }
}
