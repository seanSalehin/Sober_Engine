using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sober.Rendering;
using Sober.Rendering.Shader;

namespace Sober.Assets
{
    public static class AssetManager
    {
        //maps a string key (Player) to a Texture and a ShaderProgram
        private static Dictionary<string, Texture> texture = new();
        private static Dictionary<string, ShaderProgram> shaders = new();

        public static Texture LoadTexture(string key, string path)
        {
            //Loading a texture
            if (!texture.ContainsKey(key))
                texture[key] = new Texture(path);

            return texture[key];
        }

        public static ShaderProgram LoadShader(string key, string vert, string frag)
        {
            //Loading a shader
            if (!shaders.ContainsKey(key))
                shaders[key] = new ShaderProgram(vert, frag);

            return shaders[key];
        }

        public static void Clear()
        {
            foreach (var tex in texture.Values)
                tex.Dispose();

            texture.Clear();
            shaders.Clear();
        }
    }
}