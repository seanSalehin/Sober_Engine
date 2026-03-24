using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Sober.Rendering.Shader;

namespace Sober.Rendering.UI
{
    public sealed class UIRenderer : IDisposable
    {
        private readonly int _vao;
        private readonly int _vbo;
        private readonly ShaderProgram _shader;
        private readonly BitmapFont _font = new();

        public UIRenderer()
        {
            _shader = new ShaderProgram("Assets/Shaders/ui.vert", "Assets/Shaders/ui.frag");

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

            GL.BufferData(
                BufferTarget.ArrayBuffer,
                4 * sizeof(float) * 6 * 1024,
                IntPtr.Zero,
                BufferUsageHint.DynamicDraw);

            int stride = 4 * sizeof(float);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 2 * sizeof(float));

            GL.BindVertexArray(0);
        }

        public void Begin()
        {
            _shader.Bind();
            GL.BindVertexArray(_vao);
        }

        public void DrawRect(UITransform tr, int screenW, int screenH, Vector4 color)
        {
            tr.NdcRect(screenW, screenH, out Vector2 min, out Vector2 max);

            float[] vertices =
            {
                min.X, min.Y, 0f, 0f,
                max.X, min.Y, 1f, 0f,
                max.X, max.Y, 1f, 1f,

                min.X, min.Y, 0f, 0f,
                max.X, max.Y, 1f, 1f,
                min.X, max.Y, 0f, 1f
            };

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertices.Length * sizeof(float), vertices);

            _shader.SetInt("u_UseTexture", 0);
            _shader.SetVector4("u_Color", color);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

        public void DrawText(string text, UITransform tr, int screenW, int screenH, Vector4 color, Texture fontTexture)
        {
            if (string.IsNullOrEmpty(text)) return;

            float charWidth = tr.Size.X / text.Length;
            float charHeight = tr.Size.Y;

            GL.ActiveTexture(TextureUnit.Texture0);
            fontTexture.Bind();

            _shader.SetInt("u_UseTexture", 1);
            _shader.SetVector4("u_Color", color);
            _shader.SetInt("u_Font", 0);

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                UITransform charTr = new UITransform(
                    tr.Anchor,
                    new Vector2(tr.Position.X + i * charWidth, tr.Position.Y),
                    new Vector2(charWidth, charHeight)
                );

                charTr.NdcRect(screenW, screenH, out Vector2 min, out Vector2 max);

                var uvMin = _font.GetUvMin(c);
                var uvMax = _font.GetUvMax(c);

                float[] vertices =
                {
            min.X, min.Y, uvMin.X, uvMax.Y,
            max.X, min.Y, uvMax.X, uvMax.Y,
            max.X, max.Y, uvMax.X, uvMin.Y,

            min.X, min.Y, uvMin.X, uvMax.Y,
            max.X, max.Y, uvMax.X, uvMin.Y,
            min.X, max.Y, uvMin.X, uvMin.Y
        };

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertices.Length * sizeof(float), vertices);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }
        }

        public void End()
        {
            GL.BindVertexArray(0);
            _shader.UnBind();
        }

        public void Dispose()
        {
            _shader.Dispose();
            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
        }
    }
}