// ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
//                S O B E R   E N G I N E
//                     Sean Salehin
// ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sober.Assets;
using Sober.ECS;
using Sober.ECS.Components;
using Sober.ECS.Events;
using Sober.ECS.Systems;
using Sober.ECS.Systems;
using Sober.Engine.Core;
using Sober.Rendering;
using Sober.Rendering.Debug;
using Sober.Rendering.Mesh;
using Sober.Rendering.Shader;
using Sober.Scene;
using Sober.Rendering.Particles;

namespace Sober.Engine.Core
{
    public class Engine: GameWindow
    {

        private Render _render;
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


        //Physics + collision + Debug + FPS
        private PhysicsSystem _physicsSystem;
        private CollisionSystem _collisionSystem;
        private DebugDraw _debugDraw;
        private DebugDrawSystem _debugDrawSystem;
        private FpsSystem _fpsSystem;
        private EventBus _eventBus;


        //Particle system
        private ParticlePool _particlePool;
        private PartickeRenderer _particleRenderer;
        private ShaderProgram _particleShader;
        private ParticleSystem _particleSystem;
        private ParticleRenderSystem _particleRendereSystem;

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

            //Test scene with physics and collision
            SpawnTestGroundAndPlayer();
            VSync = VSyncMode.On;

            //Physics + collision + Debug + FPS
            _eventBus = new EventBus();
            _physicsSystem = new PhysicsSystem(_world);
            _collisionSystem = new CollisionSystem(_world, _eventBus);
            _debugDraw = new DebugDraw();
            _debugDrawSystem = new DebugDrawSystem(_world, _debugDraw);
             _fpsSystem = new FpsSystem(title => Title=title);

            //Particle system
            _particleShader = new ShaderProgram("Assets/Shaders/particle.vert", "Assets/Shaders/particle.frag");
                _particleRenderer = new PartickeRenderer(maxParticles: 8000);
                _particlePool = new ParticlePool(capacity: 8000);
                _particleSystem = new ParticleSystem(_world, _particlePool);
                _particleRendereSystem = new ParticleRenderSystem(_world, _particlePool, _particleRenderer, _particleShader);
                _systems.Add(_particleSystem);
                _systems.Add(_particleRendereSystem);


            //emitter test
            int playerId = _world.GetStore<PlayerTag>().All().First().Key;
            _world.GetStore<ParticleEmitterComponent>().Set(
                playerId,
                new ParticleEmitterComponent(
                            rate: 120f,
                            lifeMin: 0.25f, lifeMax: 0.6f,
                            speedMin: 0.2f, speedMax: 1.2f,
                            sizeMin: 6f, sizeMax: 15f,
                            color: new Vector4(0.3f, 0.7f, 1f, 1f)
                       )
                );
            var emitStore = _world.GetStore<ParticleEmitterComponent>();
            var emit = emitStore.Get(playerId);
            emit.Enabled = false;
            emitStore.Set(playerId, emit);
        }


        private void SpawnBurst(Vector2 pos, int count, BurstType type)
        {
            var e = _world.CreateEntity();

            var t = TransformComponent.Default();
            t.LocalPosition = pos;
            t.Dirty = true;

            _world.Add(e, t);
            _world.Add(e, new ParticleBurstRequest(count, type));
        }



        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            //update loop => changes frame in the game world
            Time.Update((float)args.Time);
            Input.Input.Update(KeyboardState, MouseState);

            //burst logic => space 
            if(Input.Input.Down(Keys.Space))
            {
                int playerId = _world.GetStore<PlayerTag>().All().First().Key;
                var playerT = _world.GetStore<TransformComponent>().Get(playerId);

                SpawnBurst(playerT.LocalPosition, 140, BurstType.Flash);
                SpawnBurst(playerT.LocalPosition, 350, BurstType.Sparks);
                SpawnBurst(playerT.LocalPosition, 50, BurstType.Smoke);
            }

            //debug (for collision) + fps
            _debugDrawSystem.Update();
            _fpsSystem.Update((float)args.Time);

            base.OnUpdateFrame(args);
          

            _systems.Update((float)args.Time);

            //Scenes
            _parent.Transform.LocalRotation += (float)args.Time;

            //Scene management
            if(Input.Input.Down(Keys.F1))
            {
                _sceneManager.LoadScene("Assets/Scene/scene_main.json");
            }
            if(Input.Input.Down(Keys.F2))
            {
                _sceneManager.LoadScene("Assets/Scene/scene_test.json");
            }

            //physics + collision 
            while (Time.ConsumeFixedStep())
            { 
                _physicsSystem.FixedUpdate(Time.FixedDeltaTime);
                _collisionSystem.FixedUpdate(Time.FixedDeltaTime);
            }
        }


        protected override void OnRenderFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            //render loop => called every frame, after OnUpdateFrame(), Prepares the screen for rendering new objects
            base.OnRenderFrame(args);
            _render.BeginFrame();
            _systems.Render();
            _debugDrawSystem.Render();
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnUnload()
        {
            _spriteShader.Dispose();
            _spriteTexture.Dispose();
            AssetManager.ClearAll();
            _debugDraw?.Dispose();
            _particleShader?.Dispose();
            _particleRenderer?.Dispose();
            base.OnUnload();
        }

        private void SpawnTestGroundAndPlayer()
        {

            float gravity = 20.0f;

            Vector2 floorScale = new Vector2(2.0f, 0.18f); 
            Vector2 floorPos = new Vector2(0f, -0.85f);

            Vector2 wallScale = new Vector2(0.12f, 2.0f);
            Vector2 leftWallPos = new Vector2(-1.02f, 0f);
            Vector2 rightWallPos = new Vector2(+1.02f, 0f);

            Vector2 platformScale = new Vector2(0.90f, 0.14f);
            Vector2 platformPos = new Vector2(0.10f, -0.10f);

            Vector2 blockScale = new Vector2(0.22f, 0.22f);
            Vector2 blockPos = new Vector2(-0.35f, -0.63f);

            Vector2 playerHalf = new Vector2(0.30f, 0.30f);

            int playerId = _world.GetStore<PlayerTag>().All().First().Key;

            var aabbStore = _world.GetStore<AabbColliderComponent>();
            var gravStore = _world.GetStore<GravityComponent>();

            aabbStore.Set(playerId, new AabbColliderComponent(playerHalf, CollisionLayers.Player, CollisionLayers.World));
            gravStore.Set(playerId, new GravityComponent(gravity));

            var tStore = _world.GetStore<TransformComponent>();
            var pt = tStore.Get(playerId);
            pt.LocalPosition = new Vector2(0f, 0.20f);
            pt.Dirty = true;
            tStore.Set(playerId, pt);

            CreateStaticBox("FLOOR", floorPos, floorScale);
            CreateStaticBox("LEFT_WALL", leftWallPos, wallScale);
            CreateStaticBox("RIGHT_WALL", rightWallPos, wallScale);
            CreateStaticBox("PLATFORM", platformPos, platformScale);
            CreateStaticBox("OBSTACLE", blockPos, blockScale);

            void CreateStaticBox(string name, Vector2 pos, Vector2 scale)
            {
                Vector2 half = new Vector2(scale.X * 0.5f, scale.Y * 0.5f);

                var e = _world.CreateEntity();
                _world.Add(e, TransformComponent.Default());

                var t = tStore.Get(e.Id);
                t.LocalPosition = pos;
                t.LocalScale = scale;
                t.Dirty = true;
                tStore.Set(e.Id, t);

                aabbStore.Set(e.Id, new AabbColliderComponent(half, CollisionLayers.World, CollisionLayers.Player));
                _world.GetStore<StaticBodyTag>().Set(e.Id, new StaticBodyTag());
            }
        }
    }
}