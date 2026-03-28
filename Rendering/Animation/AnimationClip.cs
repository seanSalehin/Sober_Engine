namespace Sober.Rendering.Animation
{
    public sealed class AnimationClip
    {
        public string Name = "";
        public int[] Frames = Array.Empty<int>();
        public float Fps = 8f;
        public bool Loop = true;
    }
}
