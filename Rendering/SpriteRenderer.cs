using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Sober.ECS.Systems;
using Sober.Rendering.Mesh;
using Sober.Rendering.Shader;

namespace Sober.Rendering
{
    public class SpriteRenderer
    {
        //rectangle mesh
        private readonly Sober.Rendering.Mesh.Mesh quad;
        //draws the texture on the quad
        private readonly ShaderProgram shader;

        public SpriteRenderer(ShaderProgram shader)
        {
            this.shader = shader;
            this.quad=MeshFactory.CreateQuad();
        }


        public void Draw(Texture texture, Vector2 position, Vector2 size)
        {
            shader.Bind();
            Matrix4 model = Matrix4.CreateTranslation(position.X, position.Y, 0f) * Matrix4.CreateScale(size.X, size.Y, 1f);
            shader.SetMatrix4("u_ViewProj", CameraSystem.CurrentViewProj);
            shader.SetMatrix4("u_Model", model);

            shader.SetVector4("u_UVOffset", new Vector4(0f, 0f, 1f, 1f));

            texture.Bind(TextureUnit.Texture0);
            GL.Uniform1(GL.GetUniformLocation(shader.Test_Data, "u_Texture"), 0);

            quad.Bind();
            quad.Draw();
            quad.Unbind();
        }

        public void Draw(Texture texture, Matrix4 worldMatrix)
        {
            shader.Bind();
            shader.SetMatrix4("u_Model", worldMatrix);
            shader.SetMatrix4("u_ViewProj", CameraSystem.CurrentViewProj);

            shader.SetVector4("u_UVOffset", new Vector4(0f, 0f, 1f, 1f));

            texture.Bind(TextureUnit.Texture0);
            int loc = GL.GetUniformLocation(shader.Test_Data, "u_Texture");
            if (loc != -1) GL.Uniform1(loc, 0);
            quad.Bind();
            quad.Draw();
            quad.Unbind();
        }


        public void Draw(Texture texture, Matrix4 worldMatrix, int frameId)
        {
            shader.Bind();
            shader.SetMatrix4("u_Model", worldMatrix);
            shader.SetMatrix4("u_ViewProj", CameraSystem.CurrentViewProj);

            float sheetWidth = 8.0f;
            float sheetHeight = 80.0f;

            int col = frameId % (int)sheetWidth;
            int row = frameId / (int)sheetWidth;

            float uvWidth = 1.0f / sheetWidth;
            float uvHeight = 1.0f / sheetHeight;

            float u = col * uvWidth;
            float v = 1.0f - (row * uvHeight);

            shader.SetVector4("u_UVOffset", new Vector4(u, v, uvWidth, -uvHeight));

            texture.Bind(TextureUnit.Texture0);

            int loc = GL.GetUniformLocation(shader.Test_Data, "u_Texture");
            if (loc != -1) GL.Uniform1(loc, 0);

            quad.Bind();
            quad.Draw();
            quad.Unbind();
        }
    }
}
