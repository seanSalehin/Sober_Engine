//Create / destroy entities, add / remove components, Register & update systems, Run queries.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sober.ECS
{
    public sealed class World
    {
        private int _nextId = 1;

        //entity tracking (valid / invalid)
        private readonly Dictionary<int, int> _versions = new();
        private readonly Stack<int> _freeId = new();

        //conponent storage
        private readonly Dictionary<Type, object> _storage = new();

        public Entity CreateEntity()
        {
            int id;
            if (_freeId.Count > 0)
            {
                id = _freeId.Pop();
            }
            else
            {
                id = _nextId++;
            }
            int version = _versions.TryGetValue(id, out var v) ? v : 1;
            _versions[id] = version;
            return new Entity(id, version);
        }

        public bool IsAlive(Entity e)
        {
            return _versions.TryGetValue(e.Id, out var v) && v == e.Version;
        }

        public void DestroyEntity(Entity e)
        {
            if (!IsAlive(e)) return;
            _versions[e.Id]=e.Version+1;
            foreach (var storage in _storage.Values)
            {
                ((IComponentStore)storage).Remove(e.Id);
            }
            _freeId.Push(e.Id);
        }

        //Component management  
        internal interface IComponentStore
        {
            void Remove(int entityId);
        }

        public sealed class ComponentStore<T> : IComponentStore where T : struct
        {
            private readonly Dictionary<int, T> _data = new();
            public void Set(int entityId, T value)
            {
                _data[entityId] = value;
            }
            public ref T GetRef(int entityId)
            {
                throw new NotSupportedException("Get(entityId) + Set(entityId, value)");
            }
            public bool Has(int entityId)
            {
                return _data.ContainsKey(entityId);
            }
            public void Remove(int entityId)
            {
                _data.Remove(entityId);
            }
            public T Get(int entityId)
            {
                return _data[entityId];
            }   

            void IComponentStore.Remove(int entityId)
            {
                Remove(entityId);
            }
            public IEnumerable<KeyValuePair<int, T>> All()
            {
                return _data;
            }
        }


        public ComponentStore<T> GetStore<T>() where T : struct
        {
            var type = typeof(T);
            if (_storage.TryGetValue(type, out var existing))
            {
                return (ComponentStore<T>)existing;
            }
            var newStore = new ComponentStore<T>();
            _storage[type] = newStore;
            return newStore;
        }

        public void Add<T>(Entity e, T component) where T : struct
        {
            EnsureAlive(e);
            GetStore<T>().Set(e.Id, component);
        }

        public ref T Get<T>(Entity e) where T : struct
        {
            EnsureAlive(e);
            return ref GetStore<T>().GetRef(e.Id);
        }

        public bool Has<T>(Entity e) where T : struct
        {
            if (!IsAlive(e)) return false;
            return GetStore<T>().Has(e.Id);
        }

        public void Remove<T>(Entity e) where T : struct
        {
            if (!IsAlive(e)) return;
            GetStore<T>().Remove(e.Id);
        }

        private void EnsureAlive(Entity e)
        {
            if (!IsAlive(e))
                throw new InvalidOperationException($"Entity is not alive: {e}");
        }


    }
}
