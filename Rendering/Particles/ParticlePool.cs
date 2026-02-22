using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Sober.Rendering.Particles
{

    public enum ParticleKind
    {
        Flash,
        Spark,
        Smoke
    }

    public struct Particle
    {
        public bool Alive;
        public Vector2 Position;
        public Vector2 Velocity;
        public float Lifetime;
        public float LifeMax;
        public float Size;
        public Vector4 Color;
        public float StartAlpha;
        public float Drag;
        public Vector2 Gravity;
        public ParticleKind Kind;
    }

    public sealed class ParticlePool
    {
        private readonly Particle[] _particle;
        private readonly Stack<int> _free = new();
        private readonly List<int> _alive = new();
        public IReadOnlyList<int> ALive => _alive;
        public Particle[] Raw => _particle;

        public ParticlePool(int capacity)
        {
            _particle = new Particle[capacity];
            for (int i = capacity - 1; i >= 0; i--)
                _free.Push(i);
        }

        public bool TrySpawn(out int index)
        {
            if (_free.Count == 0)
            {
                index = -1;
                return false;
            }
            index = _free.Pop();
            _alive.Add(index);
            return true;
        }

        public void Kill (int index)
        {
            _particle[index].Alive = false;
            for (int i=_alive.Count() - 1; i >= 0; i--)
            {
                if (_alive[i] == index)
                {
                    _alive.RemoveAt(i);
                    break;
                }
            }
            _free.Push(index);
        }

        public void RemoveDeadFromALiveList()
        {
            for (int i =_alive.Count - 1; i >= 0; i--)
            {
                int index = _alive[i];
                if (!_particle[index].Alive)
                {
                    _alive.RemoveAt(i);
                    _free.Push(index);
                }
            }
        }
    }
}
