//Store Mesh Refrence ,Shader, Render flags, Material
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sober.Rendering.Mesh;
using Sober.Rendering.Shader;

namespace Sober.ECS.Components
{
    public struct MeshRendererComponent
    {
        public Mesh Mesh;
        public ShaderProgram Shader;

        public MeshRendererComponent(Mesh mesh, ShaderProgram shader)
        {
            Mesh = mesh;
            Shader = shader;
        }
    }
}
