using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Sober.Engine.Core;
using Sober.Rendering;
using Sober.Rendering.Shader;

namespace Sober
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var gws = GameWindowSettings.Default;
                var nws = new NativeWindowSettings()
                {
                    Size = new Vector2i(800, 600),
                    Title = "Sober Engine"
                };
                using var engine = new Sober.Engine.Core.Engine(gws, nws);
                var shader = new ShaderProgram("Assets/Shaders/sprite.vert", "Assets/Shaders/sprite.frag");
                var spriteRenderer = new SpriteRenderer(shader);
                engine.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled Exception: " + ex);
                Console.ReadLine();
            }

        }
    }
}
