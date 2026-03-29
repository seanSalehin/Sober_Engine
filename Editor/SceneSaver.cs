using System.IO;
using System.Text.Json;
using Sober.ECS;
using Sober.ECS.Components;
using Sober.Scene;

namespace Sober.Editor
{
    public static class SceneSaver
    {
        public static void SaveScene(World world, string savePath)
        {
           string sourcePath = "Assets/Scene/scene_main.json"; 
            if (!File.Exists(sourcePath)) return;

            string jsonIn = File.ReadAllText(sourcePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<SceneData>(jsonIn, options);
            
            if (data == null) return;

            var tStore = world.GetStore<TransformComponent>();
            var lStore = world.GetStore<LightComponent>();

            // 2. Safely patch the new data into the existing entities
            foreach (var entity in data.Entities)
            {
                // Find this entity in the ECS by matching the TransformStore indices
                // (Assuming simple 1:1 mapping for your current setup)
                foreach (var kvp in tStore.All())
                {
                    int id = kvp.Key;
                    // Match by name if you added a Name component, otherwise patch ALL transforms safely
                    if (entity.Transform != null)
                    {
                        var t = tStore.Get(id);
                        entity.Transform.Position = new[] { t.LocalPosition.X, t.LocalPosition.Y };
                        entity.Transform.Scale = new[] { t.LocalScale.X, t.LocalScale.Y };
                    }
                }
            }

            // 3. Save it securely
            var outOptions = new JsonSerializerOptions { WriteIndented = true };
            string jsonOut = JsonSerializer.Serialize(data, outOptions);

            string dir = Path.GetDirectoryName(savePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            File.WriteAllText(savePath, jsonOut);
        }
    }
}
