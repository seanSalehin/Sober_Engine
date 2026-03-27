using OpenTK.Mathematics;
using Sober.ECS.Components;

namespace Sober.ECS.Systems
{
    public sealed class TileCollisionSystem
    {
        private readonly World _world;

        public TileCollisionSystem(World world)
        {
            _world = world;
        }

        public void FixedUpdate(float dt)
        {
            var transformStore = _world.GetStore<TransformComponent>();
            var velocityStore = _world.GetStore<VelocityComponent>();
            var aabbStore = _world.GetStore<AabbColliderComponent>();
            var playerStore = _world.GetStore<PlayerTag>();
            var tileCollisionStore = _world.GetStore<TileCollisionComponent>();

            foreach (var playerElement in playerStore.All())
            {
                int playerId = playerElement.Key;

                if (!transformStore.Has(playerId) || !velocityStore.Has(playerId) || !aabbStore.Has(playerId))
                    continue;

                var transform = transformStore.Get(playerId);
                var velocity = velocityStore.Get(playerId);
                var aabb = aabbStore.Get(playerId);

                foreach (var element in tileCollisionStore.All())
                {
                    int mapEntityId = element.Key;
                    var map = element.Value;

                    if (!transformStore.Has(mapEntityId))
                        continue;

                    var mapTransform = transformStore.Get(mapEntityId);

                    ResolveAxisX(ref transform, ref velocity, aabb, map, mapTransform);
                    ResolveAxisY(ref transform, ref velocity, aabb, map, mapTransform);
                }

                transform.Dirty = true;
                transformStore.Set(playerId, transform);
                velocityStore.Set(playerId, velocity);
            }
        }

        private void ResolveAxisX(
            ref TransformComponent transform,
            ref VelocityComponent velocity,
            AabbColliderComponent aabb,
            TileCollisionComponent map,
            TransformComponent mapTransform)
        {
            float left = transform.LocalPosition.X - aabb.Halfsize.X;
            float right = transform.LocalPosition.X + aabb.Halfsize.X;
            float bottom = transform.LocalPosition.Y - aabb.Halfsize.Y;
            float top = transform.LocalPosition.Y + aabb.Halfsize.Y;

            float localLeft = left - mapTransform.LocalPosition.X;
            float localRight = right - mapTransform.LocalPosition.X;
            float localBottom = bottom - mapTransform.LocalPosition.Y;
            float localTop = top - mapTransform.LocalPosition.Y;

            int minTileX = Math.Max(0, (int)MathF.Floor(localLeft / map.TileSize));
            int maxTileX = Math.Min(map.Width - 1, (int)MathF.Floor(localRight / map.TileSize));
            int minTileY = Math.Max(0, (int)MathF.Floor(localBottom / map.TileSize));
            int maxTileY = Math.Min(map.Height - 1, (int)MathF.Floor(localTop / map.TileSize));

            for (int y = minTileY; y <= maxTileY; y++)
            {
                for (int x = minTileX; x <= maxTileX; x++)
                {
                    int index = y * map.Width + x;
                    if (index < 0 || index >= map.Solid.Length)
                        continue;

                    if (map.Solid[index] == 0)
                        continue;

                    int flippedY = (map.Height - 1) - y;

                    float tileLeft = mapTransform.LocalPosition.X + x * map.TileSize;
                    float tileRight = tileLeft + map.TileSize;
                    float tileBottom = mapTransform.LocalPosition.Y + flippedY * map.TileSize;
                    float tileTop = tileBottom + map.TileSize;

                    bool overlap =
                        right > tileLeft &&
                        left < tileRight &&
                        top > tileBottom &&
                        bottom < tileTop;

                    if (!overlap)
                        continue;

                    if (velocity.Velocity.X > 0f)
                    {
                        transform.LocalPosition.X = tileLeft - aabb.Halfsize.X;
                    }
                    else if (velocity.Velocity.X < 0f)
                    {
                        transform.LocalPosition.X = tileRight + aabb.Halfsize.X;
                    }

                    velocity.Velocity.X = 0f;

                    left = transform.LocalPosition.X - aabb.Halfsize.X;
                    right = transform.LocalPosition.X + aabb.Halfsize.X;
                }
            }
        }

        private void ResolveAxisY(
            ref TransformComponent transform,
            ref VelocityComponent velocity,
            AabbColliderComponent aabb,
            TileCollisionComponent map,
            TransformComponent mapTransform)
        {
            float left = transform.LocalPosition.X - aabb.Halfsize.X;
            float right = transform.LocalPosition.X + aabb.Halfsize.X;
            float bottom = transform.LocalPosition.Y - aabb.Halfsize.Y;
            float top = transform.LocalPosition.Y + aabb.Halfsize.Y;

            float localLeft = left - mapTransform.LocalPosition.X;
            float localRight = right - mapTransform.LocalPosition.X;
            float localBottom = bottom - mapTransform.LocalPosition.Y;
            float localTop = top - mapTransform.LocalPosition.Y;

            int minTileX = Math.Max(0, (int)MathF.Floor(localLeft / map.TileSize));
            int maxTileX = Math.Min(map.Width - 1, (int)MathF.Floor(localRight / map.TileSize));
            int minTileY = Math.Max(0, (int)MathF.Floor(localBottom / map.TileSize));
            int maxTileY = Math.Min(map.Height - 1, (int)MathF.Floor(localTop / map.TileSize));

            for (int y = minTileY; y <= maxTileY; y++)
            {
                for (int x = minTileX; x <= maxTileX; x++)
                {
                    int index = y * map.Width + x;
                    if (index < 0 || index >= map.Solid.Length)
                        continue;

                    if (map.Solid[index] == 0)
                        continue;

                    int flippedY = (map.Height - 1) - y;

                    float tileLeft = mapTransform.LocalPosition.X + x * map.TileSize;
                    float tileRight = tileLeft + map.TileSize;
                    float tileBottom = mapTransform.LocalPosition.Y + flippedY * map.TileSize;
                    float tileTop = tileBottom + map.TileSize;

                    bool overlap =
                        right > tileLeft &&
                        left < tileRight &&
                        top > tileBottom &&
                        bottom < tileTop;

                    if (!overlap)
                        continue;

                    if (velocity.Velocity.Y > 0f)
                    {
                        transform.LocalPosition.Y = tileBottom - aabb.Halfsize.Y;
                    }
                    else if (velocity.Velocity.Y < 0f)
                    {
                        transform.LocalPosition.Y = tileTop + aabb.Halfsize.Y;
                    }

                    velocity.Velocity.Y = 0f;

                    bottom = transform.LocalPosition.Y - aabb.Halfsize.Y;
                    top = transform.LocalPosition.Y + aabb.Halfsize.Y;
                }
            }
        }
    }
}