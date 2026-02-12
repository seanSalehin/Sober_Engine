//message system for when collision happens
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

        //notify every system taht subscribed 
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

