using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using MeshType = Sober.Rendering.Mesh.Mesh;
using Sober.Rendering.Shader;


namespace Sober.Rendering
{
    internal class Render
    {
        
        public void BeginFrame()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void Draw(MeshType mesh, ShaderProgram shader, Matrix4?model = null)
        {

            shader.Bind();
            mesh.Bind();
            shader.SetMatrix4("u_Model", model ?? Matrix4.Identity);
            mesh.Draw();
            mesh.Unbind();
            shader.UnBind();
        }
        public void EndFrame()
        { 
        }
    }
}
