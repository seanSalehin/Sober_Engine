using OpenTK.Mathematics;
using Sober.ECS.Components;
using Sober.Rendering.Tilemap;
using Sober.Assets;

namespace Sober.ECS.Systems
{
    public sealed class TilemapSystem : ISystem
    {
        private readonly World _world;
        private readonly TilemapRenderer _renderer;

        public TilemapSystem(World world, TilemapRenderer renderer)
        {
            _world = world;
            _renderer = renderer;
        }

        public void Update(float dt) { }

        public void Render()
        {
            var transformStore = _world.GetStore<TransformComponent>();
            var tilemapStore = _world.GetStore<TilemapComponent>();

            foreach (var kvp in tilemapStore.All())
            {
                int entityId = kvp.Key;
                var map = kvp.Value;
                var transform = transformStore.Get(entityId);

                var texture = AssetManager.GetTexture(map.TileKey, map.TileKey);
                var tileset = new Tileset(map.TileSize, texture.Width, texture.Height);

                for (int i = 0; i < map.Tiles.Length; i++)
                {
                    int atlasId = map.Tiles[i];
                    if (atlasId <= 0)
                        continue;

                    int x = i % map.Width;
                    int y = i / map.Width;
                    int flippedY = (map.Height - 1) - y;

                    float worldX = transform.LocalPosition.X + x * map.TileSize + map.TileSize * 0.5f;
                    float worldY = transform.LocalPosition.Y + flippedY * map.TileSize + map.TileSize * 0.5f;

                    var model =
                        Matrix4.CreateTranslation(worldX, worldY, 0f) *
                        Matrix4.CreateScale(map.TileSize, map.TileSize, 1f);

                    tileset.GetTileUV(atlasId, out var uvMin, out var uvMax);
                    _renderer.DrawTile(texture, model, uvMin, uvMax);
                }
            }
        }
    }
    }