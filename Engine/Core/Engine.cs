// :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
//                S O B E R   E N G I N E
//                     Sean Salehin
// :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sober.Assets;
using Sober.Audio;
using Sober.ECS;
using Sober.ECS.Components;
using Sober.ECS.Events;
using Sober.ECS.Systems;
using Sober.Rendering;
using Sober.Rendering.Debug;
using Sober.Rendering.HotReload;
using Sober.Rendering.Mesh;
using Sober.Rendering.Particles;
using Sober.Rendering.PostProcessing;
using Sober.Rendering.Shader;
using Sober.Rendering.Tilemap;
using Sober.Rendering.UI;
using Sober.Scene;

namespace Sober.Engine.Core
{
    public class Engine: GameWindow
    {

        private Render _render;
        private float _time; 
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


        //hot reloaders + Post Processing
        private ShaderHotReloader _shaderHotReloader;
        private FrameBuffer _sceneFbo;
        private ScreenQuad _screenQuad;
        private ShaderProgram _postShader;

        //UI + HUD
        private UIRenderer _ui;
        private Texture _fontTex;
        private UISystem _uiSystem;

        //Audio
        private AudioManager _audio;
        private Entity  _audioEntity;
        private AudioSystem _audioSystem;

        //Tilemap
        private TilemapRenderer _tilemapRenderer;
        private TilemapSystem _tilemapSystem;
        private TileCollisionSystem _tileCollisionSystem;
        private ParallaxRenderer _parallaxRenderer;
        private ParallaxSystem _parallaxSystem;
        private ShaderProgram _tilemapShader;

        //debugging tool
        private bool _showDebug = true;

        //animation
        private AnimationSystem _animationSystem;


        //lightening  + scripting
        private ShaderProgram _lightShader;
        private FrameBuffer _lightFbo;
        private LightSystem _lightSystem;
        private ScriptSystem _scriptSystem;


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
            _animationSystem = new AnimationSystem(_world);

            _systems.Add(_animationSystem);
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
            //SpawnTestGroundAndPlayer();
            VSync = VSyncMode.On;

            //Physics + collision + Debug + FPS
            _eventBus = new EventBus();
            _physicsSystem = new PhysicsSystem(_world);
            _collisionSystem = new CollisionSystem(_world, _eventBus);
            _debugDraw = new DebugDraw();
            _debugDrawSystem = new DebugDrawSystem(_world, _debugDraw);
            _fpsSystem = new FpsSystem(title => Title = title);

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
            //_world.GetStore<ParticleEmitterComponent>().Set(
            //    playerId,
            //    new ParticleEmitterComponent(
            //                rate: 120f,
            //                lifeMin: 0.25f, lifeMax: 0.6f,
            //                speedMin: 0.2f, speedMax: 1.2f,
            //                sizeMin: 6f, sizeMax: 15f,
            //                color: new Vector4(0.3f, 0.7f, 1f, 1f)
            //           )
            //    );
            //var emitStore = _world.GetStore<ParticleEmitterComponent>();
            //var emit = emitStore.Get(playerId);
            //emit.Enabled = false;
            //emitStore.Set(playerId, emit);




            //hot reloaders + Post Processing
            _shaderHotReloader = new ShaderHotReloader("Assets/Shaders");
            _shaderHotReloader.Track(_spriteShader, "Assets/Shaders/sprite.vert", "Assets/Shaders/sprite.frag");
            _shaderHotReloader.Track(_particleShader, "Assets/Shaders/particle.vert", "Assets/Shaders/particle.frag");
            _sceneFbo = new FrameBuffer(Size.X, Size.Y);
            _screenQuad = new ScreenQuad();
            _postShader = new ShaderProgram("Assets/Shaders/post.vert", "Assets/Shaders/post.frag");
            _postShader.Bind();
            _postShader.SetInt("u_Scene", 0);
            _postShader.UnBind();
            _shaderHotReloader.Track(_postShader, "Assets/Shaders/post.vert", "Assets/Shaders/post.frag");


            //script
            _scriptSystem = new ScriptSystem(_world, (pos, count) => SpawnBurst(pos, count, BurstType.Sparks));
            _systems.Add(_scriptSystem);


            //lightening
            _lightShader = new ShaderProgram("Assets/Shaders/light.vert", "Assets/Shaders/light.frag");
            _lightFbo = new FrameBuffer(Size.X, Size.Y); 
            _lightSystem = new LightSystem(_world, _lightShader, _screenQuad, Size.X, Size.Y);

            _world.GetStore<LightComponent>().Set(playerId, new LightComponent(
                color: new Vector3(1.0f, 0.6f, 0.2f),
                radius: 0.15f,
                intensity: 1.5f
            ));

            var triggerEntity = _world.CreateEntity();
            var triggerTrans = TransformComponent.Default();
            triggerTrans.LocalPosition = new Vector2(2.0f, 0f); 
            _world.Add(triggerEntity, triggerTrans);
            _world.Add(triggerEntity, new TriggerZoneComponent(new Vector2(0.5f, 0.5f), "intro"));


            //UI + HUD

            _ui = new UIRenderer();
            _fontTex = new Texture("Assets/Textures/font.png");
            _uiSystem = new UISystem(_ui, _fontTex, Size.X, Size.Y);


            //Audio

            _audio = new AudioManager();
            _audio.Initialize();

            _audioSystem = new AudioSystem(_world);
            _systems.Add(_audioSystem);

            AssetManager.GetAudioClip("hit", "Assets/Audio/explosion.wav");

            _audioEntity = _world.CreateEntity();
            _world.Add(_audioEntity, new AudioSourceComponent
            {
                SourceId = _audio.CreateSource(),
                ClipKey = "hit",
                Loop = false,
                Volume = 1f,
                PlayRequested = false
            });


            //tilemap
            _tilemapShader = new ShaderProgram("Assets/Shaders/tilemap.vert", "Assets/Shaders/tilemap.frag");
            _tilemapRenderer = new TilemapRenderer(_tilemapShader);

            _parallaxRenderer = new ParallaxRenderer(_spriteShader, MeshFactory.CreateQuad());

            _tilemapSystem = new TilemapSystem(_world, _tilemapRenderer);
            _tileCollisionSystem = new TileCollisionSystem(_world);
            _parallaxSystem = new ParallaxSystem(_world, _parallaxRenderer);

            //_systems.Add(_parallaxSystem);
            _systems.Add(_tilemapSystem);

            var data = TilemapLoader.Load("Assets/Tilemaps/level1.json");

            var mapEntity = _world.CreateEntity();

            Vector2 mapOrigin = new Vector2(-10f, -6f);

            _world.Add(mapEntity, new TransformComponent
            {
                LocalPosition = mapOrigin,
                LocalScale = Vector2.One,
                LocalRotation = 0f,
                Dirty = true
            });

            var mapTexture = AssetManager.GetTexture(data.TileSet, data.TileSet);

            _world.Add(mapEntity, new TilemapComponent(
                data.Width,
                data.Height,
                data.TileSize,
                data.Tiles,
                data.TileSet,
                mapTexture
            ));

            int[] solid = new int[data.Tiles.Length];
            for (int i = 0; i < data.Tiles.Length; i++)
            {
                solid[i] = data.Tiles[i] > 0 ? 1 : 0;
            }

            _world.Add(mapEntity, new TileCollisionComponent(
                data.Width,
                data.Height,
                data.TileSize,
                solid
            ));

            var staticAabbStore = _world.GetStore<AabbColliderComponent>();
            var staticTransformStore = _world.GetStore<TransformComponent>();
            var staticBodyStore = _world.GetStore<StaticBodyTag>();

            for (int y = 0; y < data.Height; y++)
            {
                int x = 0;

                while (x < data.Width)
                {
                    int start = x;

                    while (x < data.Width)
                    {
                        int i = y * data.Width + x;
                        if (data.Tiles[i] <= 0)
                            break;

                        x++;
                    }

                    int end = x - 1;

                    if (end >= start)
                    {
                        int flippedY = (data.Height - 1) - y;

                        float width = (end - start + 1) * data.TileSize;
                        float height = data.TileSize;

                        float centerX = mapOrigin.X + start * data.TileSize + width * 0.5f;
                        float centerY = mapOrigin.Y + flippedY * data.TileSize + height * 0.5f;

                        var e = _world.CreateEntity();
                        _world.Add(e, TransformComponent.Default());

                        var t = staticTransformStore.Get(e.Id);
                        t.LocalPosition = new Vector2(centerX, centerY);
                        t.LocalScale = new Vector2(width, height);
                        t.LocalRotation = 0f;
                        t.Dirty = true;
                        staticTransformStore.Set(e.Id, t);

                        staticAabbStore.Set(
                            e.Id,
                            new AabbColliderComponent(
                                new Vector2(width * 0.5f, height * 0.5f),
                                CollisionLayers.World,
                                CollisionLayers.Player
                            )
                        );

                        staticBodyStore.Set(e.Id, new StaticBodyTag());
                    }

                    x++;

                    while (x < data.Width)
                    {
                        int i = y * data.Width + x;
                        if (data.Tiles[i] > 0)
                            break;

                        x++;
                    }
                }
            }


            var tStore = _world.GetStore<TransformComponent>();
            var pt = tStore.Get(playerId);
            pt.LocalPosition = new Vector2(-7.5f, -0.5f);
            pt.LocalScale = new Vector2(0.60f, 0.60f);
            pt.Dirty = true;
            tStore.Set(playerId, pt);

            var aabbStore = _world.GetStore<AabbColliderComponent>();
            aabbStore.Set(playerId, new AabbColliderComponent(
                new Vector2(0.24f, 0.24f),
                CollisionLayers.Player,
                CollisionLayers.World
            ));

            var gravStore = _world.GetStore<GravityComponent>();
            gravStore.Set(playerId, new GravityComponent(70f));

            //jump
            var moveStore = _world.GetStore<PlayerMovementComponent>();
            moveStore.Set(playerId, new PlayerMovementComponent(8f, 25f));

            var emitStore = _world.GetStore<ParticleEmitterComponent>();
            emitStore.Set(
                playerId,
                new ParticleEmitterComponent(
                    rate: 35f,
                    lifeMin: 0.18f, lifeMax: 0.40f,
                    speedMin: 0.15f, speedMax: 0.65f,
                    sizeMin: 5f, sizeMax: 11f,
                    color: new Vector4(1.0f, 0.72f, 0.18f, 0.70f)
                )
            );

            var emit = emitStore.Get(playerId);
            emit.Enabled = false;
            emitStore.Set(playerId, emit);

            int cameraId = _world.GetStore<CameraComponent>().All().First().Key;
            var camStore = _world.GetStore<CameraComponent>();
            var cam = camStore.Get(cameraId);
            cam.Size = 6.2f;
            cam.Zoom = 1.0f;
            cam.Dirty = true;
            camStore.Set(cameraId, cam);

            //animation
            //int catId = _world.GetStore<PlayerTag>().All().First().Key;
            //var catAnim = new AnimatorComponent("Assets/Textures/cat_sheet.png", "Assets/Textures/cat_sheet.png");

            //catAnim.StateMachine = new Rendering.Animation.AnimationStateMachine();

            //var animIdle = new Rendering.Animation.AnimationClip { Name = "idle", Frames = new int[] { 0, 1, 2, 3 }, Fps = 4 };
            //var animRun = new Rendering.Animation.AnimationClip { Name = "run", Frames = new int[] { 48, 49, 50, 51, 52, 53, 54, 55 }, Fps = 12 };

            //catAnim.StateMachine.Add(animIdle);
            //catAnim.StateMachine.Add(animRun);
            //catAnim.StateMachine.SetState("idle");

            //_world.GetStore<AnimatorComponent>().Set(catId, catAnim);
            //_world.GetStore<VelocityComponent>().Set(catId, new VelocityComponent());

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
            _debugDrawSystem.Update();


            //debug (for collision) + fps
            _fpsSystem.Update((float)args.Time);

            base.OnUpdateFrame(args);
          

            _systems.Update((float)args.Time);

            //Scenes
            _parent.Transform.LocalRotation += (float)args.Time;

            //Scene management
            if(Input.Input.Down(Keys.F1))
            {
                //_sceneManager.LoadScene("Assets/Scene/scene_main.json");
            }
            if(Input.Input.Down(Keys.F2))
            {
                //_sceneManager.LoadScene("Assets/Scene/scene_test.json");
            }

            //physics + collision+ tilemap
            while (Time.ConsumeFixedStep())
            { 
                _physicsSystem.FixedUpdate(Time.FixedDeltaTime);
                _collisionSystem.FixedUpdate(Time.FixedDeltaTime);
                //_tileCollisionSystem.FixedUpdate(Time.FixedDeltaTime);

            }


            //hot reloader
            _shaderHotReloader.PumpReloads();


            //audio (explosion)
            if (Input.Input.Down(Keys.F))
            {
                var audioStore = _world.GetStore<AudioSourceComponent>();
                var a = audioStore.Get(_audioEntity.Id);
                a.PlayRequested = true;
                audioStore.Set(_audioEntity.Id, a);

                int playerId = _world.GetStore<PlayerTag>().All().First().Key;
                var playerT = _world.GetStore<TransformComponent>().Get(playerId);

                SpawnBurst(playerT.LocalPosition, 140, BurstType.Flash);
                SpawnBurst(playerT.LocalPosition, 350, BurstType.Sparks);
                SpawnBurst(playerT.LocalPosition, 50, BurstType.Smoke);
            }
        }


        protected override void OnRenderFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            //post processing
            _sceneFbo.Bind();
            _render.BeginFrame();
            _systems.Render();

            GL.Disable(EnableCap.DepthTest);
            _debugDrawSystem.Render();
            GL.Enable(EnableCap.DepthTest);

            _render.EndFrame();
            _sceneFbo.Unbind(Size.X, Size.Y);

            //lightening pass
            _lightFbo.Bind();
            _render.BeginFrame();
            _lightSystem.RenderLightingPass(_sceneFbo.ColorTexture);
            _render.EndFrame();
            _lightFbo.Unbind(Size.X, Size.Y);

            _postShader.Bind();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _lightFbo.ColorTexture);

            GL.Disable(EnableCap.DepthTest);
            _screenQuad.Draw();
            _uiSystem.Render();
            GL.Enable(EnableCap.DepthTest);

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);

            //resize framebuffer for post processing
            _sceneFbo.Resize(Size.X, Size.Y);

            //lightening pass
            _lightFbo.Resize(Size.X, Size.Y);    
            _lightSystem.Resize(Size.X, Size.Y);
        }

        protected override void OnUnload()
        {
            _spriteShader.Dispose();
            _spriteTexture.Dispose();
            AssetManager.ClearAll();
            _debugDraw?.Dispose();
            _particleShader?.Dispose();
            _particleRenderer?.Dispose();
            _postShader?.Dispose();
            _sceneFbo?.Dispose();
            _shaderHotReloader?.Dispose();
            _screenQuad?.Dispose();
            _ui?.Dispose();
            _fontTex?.Dispose();
            _audio?.Dispose();
            AssetManager.ClearAll();
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