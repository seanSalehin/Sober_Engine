using OpenTK.Graphics.OpenGL4;


namespace Sober.Rendering.Mesh
    {
        public class Mesh : IDisposable
        {
            private readonly int _vao;
            private readonly int _vbo;
            private readonly int _ebo;
            private readonly int _vertexCount;
            private readonly bool _hasIndices;

            public Mesh(float[] vertices,  int vertexStride = 5, int[]? indices = null)
            {
                _vao = GL.GenVertexArray();
                _vbo = GL.GenBuffer();

                //get layout and storage for vertex
                GL.BindVertexArray(_vao);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

                //uploads vertex data to the GPU.
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                //Vertex  position 
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, vertexStride * sizeof(float), 0);


                // UV
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, vertexStride * sizeof(float), 3 * sizeof(float));

            //set up EBO
            if (indices != null)
                {
                    _hasIndices = true;
                    _ebo = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
                    _vertexCount = indices.Length;
                }
                else
                {
                    _hasIndices = false;
                    _vertexCount = vertices.Length / vertexStride;
            }
                GL.BindVertexArray(0);
            }
            public void Bind() => GL.BindVertexArray(_vao);
            public void Unbind() => GL.BindVertexArray(0);

            //Triangles
            public void Draw()
            {
                if (_hasIndices)
                    GL.DrawElements(PrimitiveType.Triangles, _vertexCount, DrawElementsType.UnsignedInt, 0);
                else
                    GL.DrawArrays(PrimitiveType.Triangles, 0, _vertexCount);
            }

            public void Dispose()
            {
                GL.DeleteBuffer(_vbo);
                if (_hasIndices) GL.DeleteBuffer(_ebo);
                GL.DeleteVertexArray(_vao);
            }
        }
    }