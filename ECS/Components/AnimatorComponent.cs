using Sober.Rendering.Animation;

namespace Sober.ECS.Components
{
    public struct AnimatorComponent
    {
        public AnimationStateMachine StateMachine;
        public string State;
        public float Timer;
        public int FrameIndex;
        public string SpritesheetKey;

        public AnimatorComponent(string state, string spritesheetKey)
        {
            State = state;
            Timer = 0f;
            SpritesheetKey = spritesheetKey;
            FrameIndex = 0;
        }
    }
}
