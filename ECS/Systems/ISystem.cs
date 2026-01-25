using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sober.ECS.Systems
{
    public interface ISystem
    {
        void Update(float dt); //delta time parameter
        void Render();
    }
}
