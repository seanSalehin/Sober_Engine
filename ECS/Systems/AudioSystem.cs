using OpenTK.Audio.OpenAL;
using Sober.Assets;
using Sober.ECS.Components;

namespace Sober.ECS.Systems
{
    public sealed class AudioSystem : ISystem
    {
        private readonly World _world;

        public AudioSystem(World world)
        {
            _world = world;
        }

        public void Update(float dt)
        {
            var store = _world.GetStore<AudioSourceComponent>();

            foreach (var kvp in store.All())
            {
                int id = kvp.Key;
                var a = store.Get(id);

                if (!a.PlayRequested)
                    continue;

                var clip = AssetManager.GetAudioClip(a.ClipKey);

                AL.Source(a.SourceId, ALSourcei.Buffer, clip.BufferId);
                AL.Source(a.SourceId, ALSourceb.Looping, a.Loop);
                AL.Source(a.SourceId, ALSourcef.Gain, a.Volume);
                AL.SourcePlay(a.SourceId);

                a.PlayRequested = false;
                store.Set(id, a);
            }
        }

        public void Render() { }
    }
}