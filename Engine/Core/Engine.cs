using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Sober.Rendering;
using Sober.Rendering.Mesh;
using Sober.Rendering.Shader;
using Sober.Scene;
using Sober.ECS;
using Sober.ECS.Systems;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sober.Assets;

namespace Sober.Engine.Core
{
    public class Engine: GameWindow
    {

        private Render _render;
        private Mesh _triangle;
        private Mesh _quad;
        private ShaderProgram _colorShader;
        private ShaderProgram _pulseShader;
        private float _time;  // for animation (pulsing effect)
        private SpriteRenderer _spriteRenderer;
        private Texture _spriteTexture;
        private World _world;
        private ShaderProgram _spriteShader;
        private SystemGroup _systems;
        private TransformSystem _transformSystem;
        private InputSystem _inputSystem;
        private RenderSystem _renderSystem;
        private SceneManager _sceneManager;


        //ECS Systems
        private MovementSystem _movementSystem;
        private CameraSystem _cameraSystem;
        private int _playerEntityId;
        private int _cameraEntityId;

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
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
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
            _spriteShader = new ShaderProgram("Assets/Shaders/sprite.vert", "Assets/Shaders/sprite.frag");

            // Create sprite renderer
            _spriteRenderer = new SpriteRenderer(_spriteShader);

            // Load texture
            _spriteTexture = new Texture("Assets/Textures/sprite.png");

            //ECS
            _world = new World();

            //ECS Systems
            _systems = new SystemGroup();
            _transformSystem = new TransformSystem(_world);
            _inputSystem = new InputSystem(_world);
            _renderSystem = new RenderSystem(_world, _render, _spriteRenderer);
            _movementSystem = new MovementSystem(_world);
            _cameraSystem = new CameraSystem(_world);

            _systems.Add(_inputSystem);
            _systems.Add(_transformSystem);
            _systems.Add(_renderSystem);
            _systems.Add(_movementSystem);
            _systems.Add(_cameraSystem);



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

            //Scene management
            _sceneManager = new SceneManager(_world, _render, _spriteRenderer);
            _sceneManager.LoadScene("Assets/Scene/scene_main.json");
        }


        protected override void OnUpdateFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            //update loop => changes frame in the game world
            Time.Update((float)args.Time);
            Input.Input.Update(KeyboardState, MouseState);
            base.OnUpdateFrame(args);
            _time += (float)args.Time;

            //animation for pulse shader
            _pulseShader.Bind();
            _pulseShader.SetFloat("u_Time", _time);
            _systems.Update((float)args.Time);

            //Scenes
            _parent.Transform.LocalRotation += (float)args.Time;

            //Scene management
            if(Sober.Engine.Input.Input.Down(Keys.F1))
            {
                _sceneManager.LoadScene("Assets/Scene/scene_main.json");
            }
           if(Sober.Engine.Input.Input.Down(Keys.F2))
            {
                    _sceneManager.LoadScene("Assets/Scene/scene_test.json");
            }
        }


        protected override void OnRenderFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            //render loop => called every frame, after OnUpdateFrame(), Prepares the screen for rendering new objects
            base.OnRenderFrame(args);
            _render.BeginFrame();
            _systems.Render();
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }



        protected override void OnUnload()
        {
            _colorShader.Dispose();
            _pulseShader.Dispose();
            _spriteShader.Dispose();
            _spriteTexture.Dispose();
            AssetManager.ClearAll();
            base.OnUnload();
        }

    }
}
