namespace Sober.ECS.Components
{
    public struct TileCollisionComponent
    {
        public int Width;
        public int Height;
        public int[] Solid;
        public int TileSize;

        public TileCollisionComponent(int width, int height, int tileSize, int[] solid)
        {
            Width = width;
            Height = height;
            TileSize = tileSize;
            Solid = solid;
        }

    }
}
