using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sober.Scene
{
    public class Scene
    {
        private readonly List<GameObject> _objects = new();

        //Read the object
        private IReadOnlyList<GameObject> Objects => _objects;

        //add a GameObject to the scene
        public void Add(GameObject obj)=> _objects.Add(obj);
    }
}
