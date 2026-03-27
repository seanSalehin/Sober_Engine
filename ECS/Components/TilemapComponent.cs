using Sober.Rendering;

namespace Sober.ECS.Components
{
    public struct TilemapComponent
    {
        public int Width;   
        public int Height;
        public int TileSize;
        public int[] Tiles;
        public string TileKey;
        public Texture Texture;

        public TilemapComponent(int width, int height, int tileSize, int[] tiles, string tileKey, Texture texture)
        {
            Width = width;
            Height = height;
            TileSize = tileSize;
            Tiles = tiles;
            TileKey = tileKey;
            Texture = texture;
        }
    }
}
