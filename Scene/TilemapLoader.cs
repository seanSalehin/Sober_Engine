using System.IO;
using System.Text.Json;

namespace Sober.Scene
{
    public sealed class TilemapData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileSize { get; set; }
        public int[] Tiles { get; set; } = Array.Empty<int>();
        public string TileSet { get; set; } = string.Empty;
    }

    public static class TilemapLoader
    {
        public static TilemapData Load(string relativePath)
        {
            var basePath = AppContext.BaseDirectory;
            var fullPath = Path.Combine(basePath, relativePath);
            var current = basePath;
            while (current != null)
            {
                if (Directory.GetFiles(current, "*.csproj").Length > 0)
                    break;

                current = Directory.GetParent(current)?.FullName;
            }

            if (current != null)
            {
                fullPath = Path.Combine(current, relativePath);
            }
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Tilemap file not found: {fullPath}");
            }
            var json = File.ReadAllText(fullPath);
            var data = JsonSerializer.Deserialize<TilemapData>(json);

            if (data == null)
                throw new Exception("Failed to deserialize tilemap");

            if (data.Width <= 0 || data.Height <= 0 || data.TileSize <= 0)
                throw new Exception("Invalid tilemap size");

            if (data.Tiles == null || data.Tiles.Length != data.Width * data.Height)
                throw new Exception("Tile data size mismatch");
              
            return data;
        }
    }
}
