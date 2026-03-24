using Sober.Audio;
using Sober.Rendering;
using Sober.Rendering.Shader;

namespace Sober.Assets
{
    public static class AssetManager
    {
        //maps a string key (Player) to a Texture and a ShaderProgram
        private static readonly Dictionary<string, Texture> _textures = new();
        private static readonly Dictionary<string, ShaderProgram> _shaders = new();
        private static readonly Dictionary<string, AudioClip> _audio = new();

        //Audio
        public static AudioClip GetAudioClip(string key, string path)
        {
            if (!_audio.TryGetValue(key, out var clip))
            {
                clip = WavLoader.LoadWav(path);
                _audio[key] = clip;
            }
            return clip;
        }

        public static AudioClip GetAudioClip(string key)
        {
            return _audio[key];
        }

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
            foreach(var a in _audio.Values) a.Dispose();

            _textures.Clear();
            _shaders.Clear();
            _audio.Clear();
        }
    }
}