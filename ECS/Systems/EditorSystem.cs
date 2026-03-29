using System.Linq;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sober.ECS.Components;
using Sober.ECS.Events;
using Sober.Editor;
using Sober.Scene;
using Input = Sober.Engine.Input.Input;

namespace Sober.ECS.Systems
{
    public sealed class EditorSystem:ISystem
    {
        private readonly RuntimeEditor _editor;
        private readonly World _world;
        private readonly SceneManager _sceneManager;
        private readonly EventBus _eventBus;

        public EditorSystem(RuntimeEditor editor, World world, SceneManager sceneManager, EventBus eventBus)
        {
            _editor = editor;
            _world = world;
            _sceneManager = sceneManager;
            _eventBus = eventBus;
        }
        public void Render()
        {
        }

        public void Update(float dt)
        {

            var transforms = _world.GetStore<TransformComponent>().All();
            var entityList = transforms.Select(element => element.Key).ToList();


            //open/close editor
            if (Input.Down(Keys.F4))
             {
                    _editor.Toggle();
                    if (_editor.IsOpen() && _editor.SelectedId() == -1 && entityList.Count > 0)
                    {
                        _editor.Select(entityList[0]);
                    }
            }

            //save to json
            if (Input.Down(Keys.F5))
            {
                SceneSaver.SaveScene(_world, "Assets/Scene/scene_main_edited.json");
                Console.WriteLine("Scene saved to Assests/Scene/scene_main_edited.json");
            }

            //reload scene
            if (Input.Down(Keys.F6))
            {
                _sceneManager.LoadScene("Assets/Scene/scene_main_edited.json");
                Console.WriteLine(" Scene reloaded.");
            }

            //toggle debug draw
            if (Input.Down(Keys.F7))
            {
                _eventBus.Publish(new ToggleDebugEvent());
            }


            if (!_editor.IsOpen())
            {
                return;
            }


            if (entityList.Count > 0)
            {
                int currentIndex = entityList.IndexOf(_editor.SelectedId());
                if (Input.Down(Keys.RightBracket))
                {
                    int next = (currentIndex + 1) % entityList.Count;
                    _editor.Select( entityList[next]);
                }
                else if (Input.Down(Keys.LeftBracket))
                {
                    int prev = (currentIndex - 1 + entityList.Count) % entityList.Count;
                    _editor.Select( entityList[prev]);
                }
            }


            //live editing

            if (_editor.SelectedId() == -1) return;
            int id = _editor.SelectedId();
           float speed = Input.Down(Keys.LeftShift) ? 2.0f : 0.5f; 
            float dtSpeed = speed * dt;


            if (_world.Has<TransformComponent>(new Entity(id, 1)))
            {
                var tStore = _world.GetStore<TransformComponent>();
                var t = tStore.Get(id);
                if (Input.Down(Keys.L)) t.LocalPosition.X += dtSpeed;
                if (Input.Down(Keys.J)) t.LocalPosition.X -= dtSpeed;
                if (Input.Down(Keys.I)) t.LocalPosition.Y += dtSpeed;
                if (Input.Down(Keys.K)) t.LocalPosition.Y -= dtSpeed;
                t.Dirty = true;
                tStore.Set(id, t);
            }


            if (_world.GetStore<LightComponent>().Has(id))
            {
                var lStore = _world.GetStore<LightComponent>();
                var l = lStore.Get(id);
                if (Input.Down(Keys.R)) l.Radius += dtSpeed;
                if (Input.Down(Keys.F)) l.Radius -= dtSpeed;
                if (Input.Down(Keys.T)) l.Intensity += dtSpeed;
                if (Input.Down(Keys.G)) l.Intensity -= dtSpeed;
                lStore.Set(id, l);
            }


        }
    }
}
