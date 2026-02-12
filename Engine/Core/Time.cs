namespace Sober.Engine.Core
{
    public static class Time
    {
        //time of this frame - time of last frame
        public static float DeltaTime { get; private set; }
        //how long the game has been running
        public static float TotalTime { get; private set; }
        //60 FPS
        public static float FixedDeltaTime { get; private set; } = 1f / 60f;
        private static float _accumulator;

        public static void Update(float dt)
        {
            DeltaTime = dt;
            TotalTime += dt;
            _accumulator += dt;

            if (_accumulator > 0.25f) _accumulator = 0.25f;
        }

        public static bool ConsumeFixedStep()
        {
            if (_accumulator >= FixedDeltaTime)
            {
                _accumulator -= FixedDeltaTime;
                return true;
            }
            return false;
        }
        //updating FPS 
        public static void SetFixedHz(float hz)
        {
            FixedDeltaTime = 1f / hz;
        }
    }
}
