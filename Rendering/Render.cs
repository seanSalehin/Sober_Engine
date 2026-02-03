using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using MeshType = Sober.Rendering.Mesh.Mesh;
using Sober.Rendering.Shader;
using Sober.ECS.Systems;


namespace Sober.Rendering
{
    public class Render
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
            shader.SetMatrix4("u_ViewProj", CameraSystem.CurrentViewProj);
            mesh.Draw();
            mesh.Unbind();
            shader.UnBind();
        }
        public void EndFrame()
        { 
        }
    }
}
