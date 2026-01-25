//Store texture
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sober.Rendering;

namespace Sober.ECS.Components
{
    public struct SpriteComponent
    {
        public Texture Texture { get; set; }
        public SpriteComponent(Texture texture)
        {
        Texture = texture;
        }
    }
}
