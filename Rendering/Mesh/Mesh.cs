using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Security.Cryptography.X509Certificates;


namespace Sober.Rendering.Mesh
    {
        internal class Mesh : IDisposable
        {
            private readonly int _vao;
            private readonly int _vbo;
            private readonly int _ebo;
            private readonly int _vertexCount;
            private readonly bool _hasIndices;

            public Mesh(float[] vertices, int[]? indices = null)
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
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);

                // vertex colors or texture
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));

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
                    //3 for positions and 4 for colors 
                    _vertexCount = vertices.Length / 7;
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