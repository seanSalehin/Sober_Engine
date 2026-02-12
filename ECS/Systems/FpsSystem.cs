namespace Sober.ECS.Systems
{
    public sealed class FpsSystem
    {
        private float _timer;
        private int _frames;
        private readonly Action<string> _setTitle;

        public FpsSystem(Action<string> setTitle)
        {
            _setTitle = setTitle;
        }

        public void Update(float deltaTime)
        {
            _timer += deltaTime;
            _frames++;
            if (_timer >= 0.25f)
            {
                float fps = _frames / _timer;
                _setTitle($"Sober Engine  |  FPS: {fps:0}");
                _frames = 0;
                _timer = 0f;
            }
        }
    }
}
