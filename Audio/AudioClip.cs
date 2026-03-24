namespace Sober.Audio
{
    public sealed class AudioClip : IDisposable
    {

        public int BufferId { get; private set; }
        public AudioClip(int bufferId)
        {
            BufferId = bufferId;
        }
        public void Dispose()
        {
               OpenTK.Audio.OpenAL.AL.DeleteBuffer(BufferId);
        }
    }
}
