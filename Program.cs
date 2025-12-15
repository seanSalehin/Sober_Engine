using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace Sober
{
    class Program
    {
        static void Main(string[] args)
        {
            var gws = GameWindowSettings.Default;
            var nws = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "Sober Engine"
            };
            using var engine = new Sober.Engine.Core.Engine(gws, nws);
            engine.Run();
        }
    }
}
