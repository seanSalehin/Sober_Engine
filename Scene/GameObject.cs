using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sober.Scene
{
    public class GameObject
    {
        public string Name { get; set; }
        public Transform Transform { get; } = new Transform();
        public GameObject(string name)
        {
            Name = name;
        }
    }
}
