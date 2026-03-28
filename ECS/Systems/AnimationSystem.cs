using Sober.ECS.Components;
using Sober.Scene;

namespace Sober.ECS.Systems
{
    public sealed class AnimationSystem : ISystem
    {
        private readonly World _world;

        public AnimationSystem (World world)
        {
            _world = world;
        }


        public void Render()
        {
        }

        public void Update(float dt)
        {
            var animatorStore = _world.GetStore<AnimatorComponent>();
            var velocityStore = _world.GetStore<VelocityComponent>();

            foreach (var element in animatorStore.All())
            {
                int entityId = element.Key;
                var anim = element.Value;

                if (velocityStore.Has(entityId))
                {
                    var velComp = velocityStore.Get(entityId);
                    string targetState = MathF.Abs(velComp.Velocity.X) > 0.1f ? "run" : "idle";

                    if (anim.StateMachine.SetState(targetState))
                    {
                        anim.Timer = 0f;
                        anim.FrameIndex = 0;
                    }
                }

                var clip = anim.StateMachine.CurrentClip();
                if (clip == null) continue;

                anim.Timer += dt;
                float frameTime = 1f / clip.Fps;

                if (anim.Timer >= frameTime)
                {
                    anim.Timer -= frameTime;
                    anim.FrameIndex++;

                    if (anim.FrameIndex >= clip.Frames.Length)
                    {
                        if (clip.Loop) anim.FrameIndex = 0;
                        else anim.FrameIndex = clip.Frames.Length - 1;
                    }
                }

                animatorStore.Set(entityId, anim);
            }
        }
    }
}
