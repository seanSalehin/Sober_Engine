// only to attach components
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sober.ECS
{
    public readonly struct Entity
    {
        public readonly int Id;
        public readonly int Version;
        public Entity(int id, int version)
        {
            Id = id;
            Version = version;
        }
        public override string ToString() => $"Entity(Id: {Id}, Version: {Version})";
    }
}
