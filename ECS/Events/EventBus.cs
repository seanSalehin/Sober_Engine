//message system
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Sober.ECS.Events
{
    public sealed class EventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();
        public void Subscribe<T>(Action<T> handler)
        {
            var Type = typeof(T);
            if(!_handlers.TryGetValue(Type, out var list))
            {
                list = new List<Delegate>();
                _handlers[Type] = list;
            }
            list.Add(handler);
        }

        //notify everyone who subscribed to T When an event of type T happens
        public void Publish<T>(T evt)
        {
            var Type = typeof(T);
            if (!_handlers.TryGetValue(Type, out var list)) return;
            foreach(var d in list)
            {
                ((Action<T>)d)(evt);
            }
        }
    }
}

