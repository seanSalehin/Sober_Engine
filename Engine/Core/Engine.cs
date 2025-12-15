using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Sober.Engine.Core
{
    public class Engine: GameWindow
    {

        //GameWindowSettings => update frequency, render frequency
        //NativeWindowSettings => window size, title
        public Engine(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }


        protected override void OnLoad()
        {
            //load behaviour and set background color of the window
            base.OnLoad();
            GL.ClearColor(Color4.CornflowerBlue);
        }

        protected override void OnUpdateFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            //update loop => changes frame in the game world
            base.OnUpdateFrame(args);
            // TODO: input and ECS
        }

        protected override void OnRenderFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            //render loop => called every frame, after OnUpdateFrame(), Prepares the screen for rendering new objects
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            SwapBuffers();
        }
    }
}
