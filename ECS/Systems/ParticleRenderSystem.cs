using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Sober.ECS.Components;
using Sober.Rendering.Particles;
using Sober.Rendering.Shader;

namespace Sober.ECS.Systems
{
    public sealed class ParticleRenderSystem : ISystem
    {
        private readonly World _world;
        private readonly ParticlePool _pool;
        private readonly PartickeRenderer _renderer;
        private readonly ShaderProgram _shader;

        public ParticleRenderSystem(World world, ParticlePool pool, PartickeRenderer renderer, ShaderProgram shader)
        {
            _world = world;
            _pool = pool;
            _renderer = renderer;
            _shader = shader;
        }
        public void Render()
        {
            //find the first moving camera
            Matrix4 viewProj = Matrix4.Identity; 
            var cameraStore = _world.GetStore<CameraComponent>();
            foreach(var entity in cameraStore.All())
            {
                int id = entity.Key;
                var camera = cameraStore.Get(id);
                viewProj = camera.ViewProj;
                break;
            }

            GL.Enable(EnableCap.ProgramPointSize);
            GL.PointSize(15f);   


            _renderer.Draw(_pool, _pool.ALive.Count,  viewProj, _shader.Test_Data);
        }

        public void Update(float dt)
        {
        }
    }
}
