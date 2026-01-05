using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Sober.Rendering;
using Sober.Rendering.Mesh;
using Sober.Rendering.Shader;
using Sober.Scene;


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
        private SpriteRenderer _spriteRenderer;
        private Texture _spriteTexture;

        //Scene
        private Scene.Scene _scene;
        private GameObject _parent;
        private GameObject _child;

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

            // Shaders 
            _colorShader = new ShaderProgram("Assets/Shaders/color.vert",
                                 "Assets/Shaders/color.frag");
            _pulseShader = new ShaderProgram("Assets/Shaders/color.vert",
                                 "Assets/Shaders/pulse.frag");

            // Create meshes
            _triangle = MeshFactory.CreateTriangle();
            _quad = MeshFactory.CreateQuad();


            // Load sprite shader
            var spriteShader = new ShaderProgram(
                    "Assets/Shaders/sprite.vert",
                    "Assets/Shaders/sprite.frag"
                );

            // Create sprite renderer
            _spriteRenderer = new SpriteRenderer(spriteShader);

            // Load texture
            _spriteTexture = new Texture("Assets/Textures/sprite.png");

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);


            //load Scene => Transform, GameObject, Scene
            _scene = new Scene.Scene();
            _parent = new GameObject("Parent");
            _parent.Transform.LocalPosition = new Vector2(-0.2f, -0.2f);
            _parent.Transform.LocalScale = new Vector2(0.6f, 0.6f);
            _child = new GameObject("Child");
            _child.Transform.LocalPosition = new Vector2(0.8f, 0f); 
            _child.Transform.LocalScale = new Vector2(0.6f, 0.6f);
            _child.Transform.SetParent(_parent.Transform, keepWorld: false);
            _scene.Add(_parent);
            _scene.Add(_child);
        }

        protected override void OnUpdateFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            //update loop => changes frame in the game world
            base.OnUpdateFrame(args);
            _time += (float)args.Time;

            //Scene
            _parent.Transform.LocalRotation += (float)args.Time;
        }

        protected override void OnRenderFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            //render loop => called every frame, after OnUpdateFrame(), Prepares the screen for rendering new objects
            base.OnRenderFrame(args);
            _render.BeginFrame();

            //Triangle Test
            _render.Draw(
                _triangle,
                _colorShader,
                Matrix4.CreateScale(0.3f) *
                Matrix4.CreateTranslation(-0.6f, 0.4f, 0f)
                );

            //Square test
            _pulseShader.Bind();
            _pulseShader.SetFloat("u_Time", _time);
            _render.Draw(
                    _quad,
                    _pulseShader,
                    Matrix4.CreateScale(0.3f) *
                    Matrix4.CreateTranslation(0.6f, 0.4f, 0f)
                );


            //Draw Sprite (.png)
            _spriteRenderer.Draw(_spriteTexture, new Vector2(-0.25f, -0.25f), new Vector2(0.5f, 0.5f));

            _render.EndFrame();
            SwapBuffers();


            //Scene
            _spriteRenderer.Draw(_spriteTexture, _parent.Transform.WorldMatrix);
            _spriteRenderer.Draw(_spriteTexture, _child.Transform.WorldMatrix);

        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }



        protected override void OnUnload()
        {
            _triangle.Dispose();
            _quad.Dispose();
            _colorShader.Dispose();
            _pulseShader.Dispose();
            _spriteTexture.Dispose();
            _spriteRenderer = null;
            base.OnUnload();
        }

    }
}
