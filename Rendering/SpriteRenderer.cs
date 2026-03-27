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
            //Activates the GPU shader
            shader.Bind();

            //model transformation matrix
            Matrix4 model = Matrix4.CreateTranslation(position.X, position.Y, 0f) * Matrix4.CreateScale(size.X, size.Y, 1f);

            shader.SetMatrix4("u_ViewProj", ECS.Systems.CameraSystem.CurrentViewProj);

            shader.SetMatrix4("u_Model", model);

            texture.Bind(TextureUnit.Texture0);
            GL.Uniform1(GL.GetUniformLocation(shader.Test_Data, "u_Texture"), 0);

            quad.Bind();
            quad.Draw();
            quad.Unbind();

        }

        public void Draw(Texture texture, OpenTK.Mathematics.Matrix4 worldMatrix)
        {
            //draw a textured quad using the object’s transform.
            shader.Bind();
            shader.SetMatrix4("u_Model", worldMatrix);
            shader.SetMatrix4("u_ViewProj", CameraSystem.CurrentViewProj);
            texture.Bind(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0);
            //where is the u_Texture uniform in this shader?
            int loc = OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(shader.Test_Data, "u_Texture");
            //uniform doesn’t exist
            if (loc != -1) OpenTK.Graphics.OpenGL4.GL.Uniform1(loc, 0);
            quad.Bind();
            quad.Draw();
            quad.Unbind();
        }
    }
}
