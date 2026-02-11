using OpenTK.Mathematics;
using Sober.Assets;
using Sober.ECS;
using Sober.ECS.Components;
using Sober.Rendering;
using Sober.Rendering.Mesh;
using Sober.Rendering.Shader;

namespace Sober.Scene
{
    public sealed class SceneManager
    {
        private readonly World _world;
        private readonly Render _render;
        private readonly SpriteRenderer _spriteRenderer;
        private readonly Dictionary<string, Entity> _entityByName = new();
        private readonly Library _prefabs = new();

        //stores the currently loaded scene data 
        private SceneData? _activeScene;

        public SceneManager(World world, Render render, SpriteRenderer spriteRenderer)
        {
            _world = world;
            _render = render;
            _spriteRenderer = spriteRenderer;
        }

        private void UnloadActiveScene()
        {
            //delete all entities we created to replace it with the new scene
            foreach(var entity in _entityByName)
            {
                _world.DestroyEntity(entity.Value);
            }
            _entityByName.Clear();
            AssetManager.ClearAll();
            _activeScene = null;
        }


        private void LoadAssets(SceneData scene)
        {
            //preload assets defined by scene

            //shaders
            foreach(var shader in scene.Assets.Shaders)
            {
                string key = shader.Key;
                string vert = shader.Value.Vert;
                string frag = shader.Value.Frag;
                AssetManager.GetShader(key, vert, frag);
            }

            //textures
            foreach(var texture in scene.Assets.Textures)
            {
                string key = texture.Key;
                string path = texture.Value;
                AssetManager.GetTexture(key, path);
            }
        }


        public void LoadPrefabFiles(SceneData scene)
        {
            //TODO: this is hardcoded for now

            //player prefab
            var playerPrefab  = SceneLoader.Load("Assets/Prefabs/player_cat.json");
            if(playerPrefab.Entities.Count>0)
            {
                _prefabs.Register("player_cat", playerPrefab.Entities[0]);
            }

            //camera prefab
            var camPrefab = SceneLoader.Load("Assets/Prefabs/camera_follow.json");
            if (camPrefab.Entities.Count > 0)
            {
                _prefabs.Register("camera_follow", camPrefab.Entities[0]);
            }
        }


        private void BuildEntities(SceneData scene)
        {
            //create entities
            foreach (var entityDataRaw in scene.Entities)
            {
                var entityData = entityDataRaw;
                if (entityData.Prefab != null && _prefabs.TryGet(entityData.Prefab, out var prefab))
                {
                    //// apply prefab if specified
                    entityData = Library.Merge(prefab, entityData);
                }
                var e = _world.CreateEntity();
                _entityByName[entityData.Name] = e;


                //Transform
                if (entityData.Transform != null)
                {
                    var t = TransformComponent.Default();
                    t.LocalPosition = new Vector2(entityData.Transform.Position[0], entityData.Transform.Position[1]);
                    t.LocalRotation = entityData.Transform.Rotation;
                    t.LocalScale = new Vector2(entityData.Transform.Scale[0], entityData.Transform.Scale[1]);
                    t.Dirty = true;
                    _world.Add(e, t);
                }

                //player tag
                if (entityData.Player)
                {
                    _world.Add(e, new PlayerTag());
                }

                //Velocity
                if (entityData.Velocity != null)
                {
                    _world.Add(e, new VelocityComponent(entityData.Velocity.Speed));
                }


                //sprite 
                if (entityData.Sprite != null)
                {
                    var texture = AssetManager.GetTexture(entityData.Sprite.TextureKey, _activeScene!.Assets.Textures[entityData.Sprite.TextureKey]);
                    _world.Add(e, new SpriteComponent(texture));
                }


                //Mesh
                if (entityData.Mesh != null)
                {
                    Mesh mesh = entityData.Mesh.Mesh switch
                    {
                        "Triangle" => MeshFactory.CreateTriangle(),
                        "Quad" => MeshFactory.CreateQuad(),
                        _ => MeshFactory.CreateQuad(),
                    };
                    var shaderData = _activeScene!.Assets.Shaders[entityData.Mesh.ShaderKey];
                    ShaderProgram shader = AssetManager.GetShader(entityData.Mesh.ShaderKey, shaderData.Vert, shaderData.Frag);
                    _world.Add(e, new MeshRendererComponent(mesh, shader));
                }


                //camera follow
                if (entityData.Camera != null)
                {
                    var cam = CameraComponent.Default();
                    cam.Size = entityData.Camera.Size;
                    cam.Zoom = entityData.Camera.Zoom;
                    cam.Dirty = true;
                    _world.Add(e, cam);
                }
                if (entityData.Follow != null)
                {
                    //temporary dummy data (-1) 
                    _world.Add(e, new CameraFollowComponent(targetEntity: -1)
                    {
                        Smoothing = entityData.Follow.Smoothing,
                        DeadZone = new Vector2(entityData.Follow.DeadZone[0], entityData.Follow.DeadZone[1])
                    });
                }
            }
        }


        private void ResolveFollowTargets(SceneData scene)
        {
            // resolve camera follow targets by name
            var follow = _world.GetStore<CameraFollowComponent>();
            foreach (var entityData in scene.Entities)
            {
                if(entityData.Follow==null)
                {
                    continue;
                }
                int camId = _entityByName[entityData.Name].Id;
                if(!follow.Has(camId))
                {
                    continue;
                }
                if(!_entityByName.TryGetValue(entityData.Follow.TargetName, out var targetId))
                {
                    throw new Exception($"CameraFollow target not found: '{entityData.Follow.TargetName}'");
                }
                var followComponent = follow.Get(camId);
                followComponent.TargetEntity = targetId.Id;
                follow.Set(camId, followComponent);
            }

        }


        public void LoadScene(string path)
        {

            UnloadActiveScene();

            // load json (new scene)
            var scene = SceneLoader.Load(path);
            _activeScene = scene;

            LoadAssets(scene);
            LoadPrefabFiles(scene);
            BuildEntities(scene);
            ResolveFollowTargets(scene);
        }
    }
}
