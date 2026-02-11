using System.Text.Json;

namespace Sober.Scene
{
    public static class SceneLoader
    {
        //loads a scene from a JSON file and turns it into a SceneData object
        public static SceneData Load(string relativePath)
        {
            //if assets copied
            var binPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relativePath));
            if (File.Exists(binPath))
                return Deserialize(binPath);

            //  project root - if the file is not found in the bin folder, try looking in the project folder
            var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\"));
            var projectPath = Path.Combine(projectRoot, relativePath);
            if (File.Exists(projectPath))
                return Deserialize(projectPath);

            throw new FileNotFoundException(
                $"Scene file not found.\nTried:\n- {binPath}\n- {projectPath}"
            );
        }

        private static SceneData Deserialize(string fullPath)
        {
            var json = File.ReadAllText(fullPath);
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<SceneData>(json, opts);
            if (data == null) throw new Exception("Failed to parse scene JSON: " + fullPath);
            return data;
        }
    }
}
