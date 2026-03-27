using OpenTK.Mathematics;

namespace Sober.Rendering.Tilemap
{
    public sealed class Tileset
    {
        public int TileSize;
        public int TextureWidth;
        public int TextureHeight;

        public Tileset(int tileSize, int textureW, int textureH)
        {
            TileSize = tileSize;
            TextureWidth = textureW;
            TextureHeight = textureH;
        }

        public void GetTileUV(int tileId, out Vector2 uvMin, out Vector2 uvMax)
        {
            int tilesPerRow = TextureWidth / TileSize;
            int x = tileId % tilesPerRow;
            int y = tileId / tilesPerRow;
            uvMin = new Vector2(x * TileSize / (float)TextureWidth, y * TileSize / (float)TextureHeight);
            uvMax = new Vector2((x + 1) * TileSize / (float)TextureWidth, (y + 1) * TileSize / (float)TextureHeight);
        }
    }
}
