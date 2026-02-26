using System;
using OpenTK.Graphics.OpenGL4;

namespace Sober.Rendering.PostProcessing
{
    public sealed class ScreenQuad: IDisposable
    {
        private readonly int _vao;
        private readonly int _vbo;

        public ScreenQuad()
        {
            var VAO = GL.GenVertexArray();
            var VBO = GL.GenBuffer();
            _vao= VAO;
            _vbo = VBO;
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

            float[] vertices =
            {
                // positions (x, y)   // u, v
                -1.0f,  1.0f, 0.0f, 1.0f,
                -1.0f, -1.0f, 0.0f, 0.0f,
                 1.0f, -1.0f, 1.0f, 0.0f,
                -1.0f,  1.0f, 0.0f, 1.0f,
                 1.0f, -1.0f, 1.0f, 0.0f,
                 1.0f,  1.0f, 1.0f, 1.0f
            };
            
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        }

        public void Draw()
        {
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
                GL.DeleteBuffer(_vbo);
                GL.DeleteVertexArray(_vao);
        }
    }
}
