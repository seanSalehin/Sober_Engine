using System;
using OpenTK.Mathematics;
using Sober.ECS.Components;
using Sober.ECS.Events;
using Sober.Scene;

namespace Sober.ECS.Systems
{
    internal class ScriptSystem : ISystem
    {

        private readonly World _world;
        private readonly Action<Vector2, int> _spawnBurst;


        public void Render()
        {
        }

        public ScriptSystem(World world, Action<Vector2, int> spawnBurst)
        {
            _world = world;
            _spawnBurst = spawnBurst;
        }


        public void Update(float dt)
        {
            var triggerStore = _world.GetStore<TriggerZoneComponent>();
            var tStore = _world.GetStore<TransformComponent>();

            int playerId = _world.GetStore<PlayerTag>().All().First().Key;
            var playerPos = tStore.Get(playerId).LocalPosition;

            foreach (int id in Query.with<TransformComponent, TriggerZoneComponent>(_world))
            {
                var trigger = triggerStore.Get(id);
                if (trigger.IsTriggered) continue;

                var triggerPos = tStore.Get(id).LocalPosition;

                bool inX = MathF.Abs(playerPos.X - triggerPos.X) < trigger.HalfSize.X;
                bool inY = MathF.Abs(playerPos.Y - triggerPos.Y) < trigger.HalfSize.Y;

                if (inX && inY)
                {
                    trigger.IsTriggered = true;
                    triggerStore.Set(id, trigger);
                    ExecuteScript(trigger.ScriptId, triggerPos);
                }
            }
        }


        private void ExecuteScript(string scriptId, Vector2 triggerPos)
        {
            ScriptData script = ScriptLoader.Load($"Assets/Scripts/{scriptId}.json");
            foreach (var evt in script.Events)
            {
                if (evt.Type == "burst")
                {
                    _spawnBurst(triggerPos, evt.Count);
                }
                else if (evt.Type == "print")
                {
                    Console.WriteLine($"[SCRIPT EVENT]: {evt.Message}");
                }
            }
        }


        }
}
