using OpenTK.Mathematics;
using Sober.Rendering.Mesh;
using Sober.Rendering.Shader;

namespace Sober.Rendering.Tilemap
{
    public sealed class ParallaxRenderer
    {

        private readonly ShaderProgram _shader;
        private readonly Mesh.Mesh _quad; 

        public ParallaxRenderer(ShaderProgram shader, Mesh.Mesh quad)
        {
            _shader = shader;
            _quad = quad;
        }
        public void Draw(Texture tex, float cameraX, float factor)
        {
            _shader.Bind();
            _quad.Bind();
            var offset = cameraX * factor;
            var model = Matrix4.CreateScale(2000f, 600f, 1f) *Matrix4.CreateTranslation(-offset, 0f, 0f);

            _shader.SetMatrix4("u_Model", model);
            _shader.SetMatrix4("u_ViewProj", ECS.Systems.CameraSystem.CurrentViewProj);

            tex.Bind(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0);
            OpenTK.Graphics.OpenGL4.GL.Uniform1(OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(_shader.Test_Data, "u_Texture"), 0);

            _quad.Bind();
            _quad.Draw();
            _quad.Unbind();
        }
    }
}
