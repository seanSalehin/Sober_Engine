using System.IO;
using System.Text.Json;

namespace Sober.Scene
{
    public sealed class ScriptData
    {
        public string Id { get; set; } = "";
        public ScriptEvent[] Events { get; set; } = Array.Empty<ScriptEvent>();
    }

    public sealed class ScriptEvent
    {
        public string Type { get; set; } = "";
        public string? Message { get; set; }
        public int Count { get; set; }
    }

    public static class ScriptLoader
    {
        public static ScriptData Load(string path)
        {
            string json = File.ReadAllText(path);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<ScriptData>(json, options) ?? new ScriptData();
        }
    }
}