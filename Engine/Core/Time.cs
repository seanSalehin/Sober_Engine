using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sober.Engine.Core
{
    public static class Time
    {
        public static float DeltaTime { get; set; }
        public static float TotalTIme { get; set; }
        public static float FixedDeltaTime { get; set; } = 1f / 60f;
        public static float Accumulator { get; private set; }
        public static void Update(float dt)
        {
             TotalTIme += dt;
            Accumulator += dt;
            DeltaTime = dt;
          }
        public static bool ConsumeFixedStep()
        {
            if (Accumulator  >= FixedDeltaTime)
            {
                Accumulator -= FixedDeltaTime;
                return true;
            }
            return false;
        }
    }

}
