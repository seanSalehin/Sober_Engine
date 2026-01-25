using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Sober.Rendering;
using Sober.Rendering.Mesh;
using Sober.Rendering.Shader;
using Sober.Scene;
using Sober.ECS;
using Sober.ECS.Components;
using Sober.ECS.Systems;

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
            _inputSystem = new InputSystem();
            _renderSystem = new RenderSystem(_world, _render, _spriteRenderer);
            _systems.Add(_inputSystem);
            _systems.Add(_transformSystem);
            _systems.Add(_renderSystem);


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


            //Entity samples (create entities in the ECS world)

            // Triangle entity
            var tri = _world.CreateEntity();
            _world.Add(tri, TransformComponent.Default());
            var triT = _world.GetStore<TransformComponent>().Get(tri.Id);
            triT.LocalPosition = new Vector2(-0.6f, 0.0f);
            triT.LocalScale = new Vector2(0.6f, 0.6f);
            triT.Dirty = true;
            _world.GetStore<TransformComponent>().Set(tri.Id, triT);

            _world.Add(tri, new MeshRendererComponent(
                MeshFactory.CreateTriangle(),
                _colorShader
            ));

            // Pulse quad entity
            var pulse = _world.CreateEntity();
            _world.Add(pulse, TransformComponent.Default());
            var pT = _world.GetStore<TransformComponent>().Get(pulse.Id);
            pT.LocalPosition = new Vector2(0.6f, 0.0f);
            pT.LocalScale = new Vector2(0.5f, 0.5f);
            pT.Dirty = true;
            _world.GetStore<TransformComponent>().Set(pulse.Id, pT);

            _world.Add(pulse, new MeshRendererComponent(
                MeshFactory.CreateQuad(),
                _pulseShader
            ));

            // Sprite entity (cat)
            var cat = _world.CreateEntity();
            _world.Add(cat, TransformComponent.Default());
            var cT = _world.GetStore<TransformComponent>().Get(cat.Id);
            cT.LocalPosition = new Vector2(0.0f, -0.2f);
            cT.LocalScale = new Vector2(0.7f, 0.7f);
            cT.Dirty = true;
            _world.GetStore<TransformComponent>().Set(cat.Id, cT);

            _world.Add(cat, new SpriteComponent(_spriteTexture));
        }
        

        protected override void OnUpdateFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            //update loop => changes frame in the game world
            base.OnUpdateFrame(args);
            _time += (float)args.Time;

            //animation for pulse shader
            _pulseShader.Bind();
            _pulseShader.SetFloat("u_Time", _time);
            _systems.Update((float)args.Time);

            //Scene
            _parent.Transform.LocalRotation += (float)args.Time;
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
            base.OnUnload();
        }

    }
}
