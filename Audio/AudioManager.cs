using OpenTK.Audio.OpenAL;

namespace Sober.Audio
{
    public sealed class AudioManager : System.IDisposable
    {
        private ALDevice _device;
        private ALContext _context;

        public void Initialize()
        {
            _device = ALC.OpenDevice(null);
            _context = ALC.CreateContext(_device, (int[])null);
            ALC.MakeContextCurrent(_context);

            AL.Listener(ALListener3f.Position, 0f, 0f, 0f);
            AL.Listener(ALListener3f.Velocity, 0f, 0f, 0f);

            //directions: up and forward
            float[] orientation = new float[]
            {
                0f, 0f, -1f,  
                0f, 1f, 0f    
            };

            AL.Listener(ALListenerfv.Orientation, orientation);

            //master volume
            AL.Listener(ALListenerf.Gain, 1f);
        }

        public int CreateSource()
        {
            int src = AL.GenSource();
            return src;
        }

        public void Dispose()
        {
            ALC.MakeContextCurrent(ALContext.Null);
            ALC.DestroyContext(_context);
            ALC.CloseDevice(_device);
        }
    }
}