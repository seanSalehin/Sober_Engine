using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Sober.Rendering;
using Sober.Rendering.Mesh;
using Sober.Rendering.Shader;

namespace Sober.Engine.Core
{
    public class Engine: GameWindow
    {

        private Render _render;
        private Mesh _triangle;
        private Mesh _quad;
        private ShaderProgram _colorShader;
        private ShaderProgram _pulseShader;
        private float _time;


        //GameWindowSettings => update frequency, render frequency
        //NativeWindowSettings => window size, title
        public Engine(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }


        protected override void OnLoad()
        {
            //load behaviour and set background color of the window
            GL.Viewport(0, 0, Size.X, Size.Y);
            base.OnLoad();
            GLContext.Initialize();
            _render = new Render();

            //shaders
            _colorShader = new ShaderProgram("Assets/Shaders/color.vert", "Assets/Shaders/color.frag");
            _pulseShader = new ShaderProgram("Assets/Shaders/color.vert", "Assets/Shaders/pulse.frag");

            // Create meshes
            _triangle = MeshFactory.CreateTriangle();
            _quad = MeshFactory.CreateQuad();
        }

        protected override void OnUpdateFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            //update loop => changes frame in the game world
            base.OnUpdateFrame(args);
            _time += (float)args.Time;
        }

        protected override void OnRenderFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            //render loop => called every frame, after OnUpdateFrame(), Prepares the screen for rendering new objects
            base.OnRenderFrame(args);
            _render.BeginFrame();

            //Triangle Test
            _render.Draw(_triangle, _colorShader, Matrix4.CreateTranslation(-0.5f, 0f, 0f));

            //Square test
            _pulseShader.Bind();
            _pulseShader.SetFloat("u_Time", _time);
            _render.Draw(_quad, _pulseShader, Matrix4.CreateTranslation(0.5f, 0f, 0f));

            _render.EndFrame();
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }
    }
}
