using OpenTK.Graphics.OpenGL4;

namespace Sober.Rendering
{
    internal static class GLContext
    {
        public static void Initialize()
        {
            Console.WriteLine($"GPU:{GL.GetString(StringName.Renderer)}");
            Console.WriteLine($"OpenGL Version:{GL.GetString(StringName.Version)}");
            Console.WriteLine($"OpenGL Shading Language Version:{GL.GetString(StringName.ShadingLanguageVersion)}");

            //Depth Test
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            //Blending
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            //Sets the background color of the screen.
            GL.ClearColor(0.392f, 0.584f, 0.929f, 1.0f);
        }
    }
}
