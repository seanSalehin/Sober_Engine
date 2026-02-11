using Sober.Rendering;
using Sober.Rendering.Shader;

namespace Sober.Assets
{
    public static class AssetManager
    {
        //maps a string key (Player) to a Texture and a ShaderProgram
        private static readonly Dictionary<string, Texture> _textures = new();
        private static readonly Dictionary<string, ShaderProgram> _shaders = new();

        //load a texture
        public static Texture GetTexture(string key, string path)
        {
            if (!_textures.TryGetValue(key, out var tex))
            {
                tex = new Texture(path);
                _textures[key] = tex;
            }
            return tex;
        }

        //load a shader
        public static ShaderProgram GetShader(string key, string vert, string frag)
        {
            if (!_shaders.TryGetValue(key, out var sh))
            {
                sh = new ShaderProgram(vert, frag);
                _shaders[key] = sh;
            }
            return sh;
        }

        public static void ClearAll()
        {
            foreach (var t in _textures.Values) t.Dispose();
            foreach (var s in _shaders.Values) s.Dispose();

            _textures.Clear();
            _shaders.Clear();
        }
    }
}