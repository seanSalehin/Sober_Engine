namespace Sober.Rendering.Animation
{
    public sealed class AnimationStateMachine
    {
        private readonly Dictionary<string, AnimationClip> _clip = new();
        public string CurrentState { get; private set; } = "";

        public void Add(AnimationClip clip)
        {
            _clip[clip.Name] = clip;
        }

        public bool SetState(string name)
        {
            if (CurrentState == name)
            {
                return false;
            }
            else if (!_clip.ContainsKey(name))
            {
                return false;
            }
            CurrentState = name;
            return true;
        }

        public AnimationClip? CurrentClip()
        {
            _clip.TryGetValue(CurrentState, out var clip);
            if (clip != null)
            {
                return clip;
            }
            return null;
        }
    }
}
