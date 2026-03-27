using OpenTK.Mathematics;
using Sober.Rendering.Shader;


namespace Sober.Rendering.Tilemap
{
    public sealed class TilemapRenderer
    {
        private readonly ShaderProgram _shader;
        private readonly Mesh.Mesh _quad;

        public TilemapRenderer(ShaderProgram shader)
        {
            _shader = shader;
            _quad = Mesh.MeshFactory.CreateQuad();
        }

        public void DrawTile(Texture tileSetTex, Matrix4 model, Vector2 uvMin, Vector2 uvMax)
        {
            _shader.Bind();
            _shader.SetMatrix4("u_Model", model);
            _shader.SetMatrix4("u_ViewProj", Sober.ECS.Systems.CameraSystem.CurrentViewProj);
            _shader.SetFloat("u_UvMinX", uvMin.X);
            _shader.SetFloat("u_UvMinY", uvMin.Y);
            _shader.SetFloat("u_UvMaxX", uvMax.X);
            _shader.SetFloat("u_UvMaxY", uvMax.Y);
            tileSetTex.Bind(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0);
            OpenTK.Graphics.OpenGL4.GL.Uniform1(
                OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(_shader.Test_Data, "u_Tileset"), 0);
            _quad.Bind();
            _quad.Draw();
            _quad.Unbind();
        }
    }
}
