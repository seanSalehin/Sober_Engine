using OpenTK.Mathematics;
using Sober.ECS.Components;
using Sober.ECS.Events;

namespace Sober.ECS.Systems
{
    public sealed class CollisionSystem
    {
        //AABB + Circle + Trigger  + LayerMask
        private readonly World _world;
        private readonly EventBus _bus;

        public CollisionSystem(World world, EventBus eventBus)
        {
            _world = world;
            _bus = eventBus;
        }

        public void FixedUpdate(float deltaTime)
        {
            var timeStore = _world.GetStore<TransformComponent>();
            var velocityStore = _world.GetStore<VelocityComponent>();
            var aabbSore = _world.GetStore<AabbColliderComponent>();
            var circleStore = _world.GetStore<CircleColliderComponent>();
            var stateStore = _world.GetStore<CollisionStateComponent>();
            var staticStore = _world.GetStore<StaticBodyTag>();


            //check if collision state exist for aabb and circle
            foreach (var element in aabbSore.All())
            {
                if (!stateStore.Has(element.Key))
                {
                    stateStore.Set(element.Key, CollisionStateComponent.Create());
                }
            }

            foreach (var element in circleStore.All())
            {
                if (!stateStore.Has(element.Key))
                {
                    stateStore.Set(element.Key, CollisionStateComponent.Create());
                }
            }

            //collision only for dynamic bodies (velocity)
            foreach (var element in velocityStore.All())
            {
                int a = element.Key;
                if (!timeStore.Has(a))
                {
                    continue;
                }

                if (staticStore.Has(a))
                {
                    continue;
                }

                bool hasAabb = aabbSore.Has(a);
                bool hasCircle = circleStore.Has(a);
                if (!hasAabb && !hasCircle)
                {
                    continue;
                }

                // AABB targets
                foreach (var aabbElement in aabbSore.All())
                {
                    int b = aabbElement.Key;
                    if (a == b)
                    {
                        continue;
                    }

                    if (!timeStore.Has(b))
                    {
                        continue;
                    }

                    if (hasAabb)
                    {
                        HandleAabbVsAabb(a, b, timeStore, velocityStore, aabbSore, stateStore);
                    }
                    if (hasCircle)
                    {
                        HandleCircleVsAabb(a, b, timeStore, velocityStore, circleStore, aabbSore, stateStore);
                    }
                }


                //Circle targets
                foreach (var circleElement in circleStore.All())
                {
                    int b = circleElement.Key;
                    if (a == b)
                    {
                        continue;
                    }
                    if (!timeStore.Has(b))
                    {
                        continue;
                    }
                    if (hasAabb)
                    {
                        HandleAabbVsCircle(a, b, timeStore, velocityStore, aabbSore, circleStore, stateStore);
                    }
                    if (hasCircle)
                    {
                        HandleCircleVsCircle(a, b, timeStore, velocityStore, circleStore, stateStore);
                    }
                }
            }
        }



        private bool LayerALlows(int aLayer, int aMask, int bLayer)
        {
            return (aMask & bLayer) != 0;
        }


        //check collision start and end and triggers events
        private void TrackEnterExit(int a, int b, bool isColliding, World.ComponentStore<CollisionStateComponent> stateStore)
        {
            var aState = stateStore.Get(a);
            var bState = stateStore.Get(b);
            bool aData = aState.Current.Contains(b);
            if (isColliding && !aData)
            {
                aState.Current.Add(b);
                bState.Current.Add(a);
                stateStore.Set(a, aState);
                stateStore.Set(b, bState);
                _bus.Publish(new CollisionEnterEvent(a, b));
            }
            else if (!isColliding && aData)
            {
                aState.Current.Remove(b);
                bState.Current.Remove(a);
                stateStore.Set(a, aState);
                stateStore.Set(b, bState);
                _bus.Publish(new CollisionExitEvent(a, b));
            }
        }

        //checks AABB collision, fires events, and pushes A out if overlapping (a goes inside b)
        public void HandleAabbVsAabb(
            int a,
            int b,
            World.ComponentStore<TransformComponent> timeStore,
            World.ComponentStore<VelocityComponent> velocityStore,
            World.ComponentStore<AabbColliderComponent> aabbStore,
            World.ComponentStore<CollisionStateComponent> stateStore)
        {
            var aData = aabbStore.Get(a);
            var bData = aabbStore.Get(b);
            var aTime = timeStore.Get(a);
            var bTime = timeStore.Get(b);


            //check if a allows to hit b and vice versa
            if (!LayerALlows(aData.Layer, aData.Mask, bData.Layer))
            {
                return;
            }
            if (!LayerALlows(bData.Layer, bData.Mask, aData.Layer))
            {
                return;
            }

            Vector2 aPosition = aTime.LocalPosition;
            Vector2 bPosition = bTime.LocalPosition;
            Vector2 delta = aPosition - bPosition;

            Vector2 overlap = new Vector2(
                (aData.Halfsize.X / 2 + bData.Halfsize.X / 2) - MathF.Abs(delta.X),
                (aData.Halfsize.Y / 2 + bData.Halfsize.Y / 2) - MathF.Abs(delta.Y)
            );

            bool hit = overlap.X > 0 && overlap.Y > 0;

            TrackEnterExit(a, b, hit, stateStore);

            if (!hit)
            {
                return;
            }

            if (aData.IsTrigger || bData.IsTrigger)
            {
                return;
            }

            //calculate smallest overlap direction and push A out of B
            Vector2 push = Vector2.Zero;
            if (overlap.X < overlap.Y)
            {
                push.X = delta.X < 0 ? -overlap.X : overlap.X;
            }
            else
            {
                push.Y = delta.Y < 0 ? -overlap.Y : overlap.Y;
            }

            aTime.LocalPosition += push;
            aTime.Dirty = true;
            timeStore.Set(a, aTime);

            //stop velocity on the axis that collided
            if (velocityStore.Has(a))
            {
                var v = velocityStore.Get(a);
                if (push.X != 0)
                {
                    v.Velocity.X = 0;
                }

                if (push.Y != 0)
                {
                    v.Velocity.Y = 0;
                }

                velocityStore.Set(a, v);
            }
        }


        private void HandleCircleVsAabb(
        int aCircle,
        int bAabb,
        World.ComponentStore<TransformComponent> timeStore,
        World.ComponentStore<VelocityComponent> velocityStore,
        World.ComponentStore<CircleColliderComponent> circleStore,
        World.ComponentStore<AabbColliderComponent> aabbStore,
        World.ComponentStore<CollisionStateComponent> stateStore
        )
        {
            var aData = circleStore.Get(aCircle);
            var bData = aabbStore.Get(bAabb);
            var aTime = timeStore.Get(aCircle);
            var bTime = timeStore.Get(bAabb);

            //check if a allows to hit b and vice versa
            if (!LayerALlows(aData.Layer, aData.Mask, bData.Layer))
            {
                return;
            }
            if (!LayerALlows(bData.Layer, bData.Mask, aData.Layer))
            {
                return;
            }
            Vector2 aPosition = aTime.LocalPosition;
            Vector2 bPosition = bTime.LocalPosition;

            //shortest point on AABB to the center of the circle.
            Vector2 min = bPosition - bData.Halfsize;
            Vector2 max = bPosition + bData.Halfsize;
            Vector2 closest = new Vector2
                (
                    MathF.Max(min.X, MathF.Min(aPosition.X, max.X)),
                    MathF.Max(min.Y, MathF.Min(aPosition.Y, max.Y))
                );

            //measure distance
            Vector2 delta = aPosition - closest;
            float distance = delta.LengthSquared;
            bool hit = distance < aData.Radius * aData.Radius;

            TrackEnterExit(aCircle, bAabb, hit, stateStore);

            if (!hit)
            {
                return;
            }

            if (aData.IsTrigger || bData.IsTrigger)
            {
                return;
            }
            float dist = MathF.Sqrt(distance);

            //points from the other object to the circle. (0=overlap , 1=touching, >1 separated)
            Vector2 collisionDerection = dist > 0 ? delta / dist : new Vector2(0, 1);

            aTime.LocalPosition += collisionDerection * (aData.Radius - dist);
            aTime.Dirty = true;
            timeStore.Set(aCircle, aTime);

            //stop velocity on the axis that collided
            if (velocityStore.Has(aCircle))
            {
                var v = velocityStore.Get(aCircle);
                float dot = Vector2.Dot(v.Velocity, collisionDerection);
                if (dot < 0)
                {
                    v.Velocity -= collisionDerection * dot;
                    velocityStore.Set(aCircle, v);
                }
            }

        }




        private void HandleCircleVsCircle(
        int a,
        int b,
        World.ComponentStore<TransformComponent> timeStore,
        World.ComponentStore<VelocityComponent> velocityStore,
        World.ComponentStore<CircleColliderComponent> circleStore,
        World.ComponentStore<CollisionStateComponent> stateStore)
        {
            var aData = circleStore.Get(a);
            var bData = circleStore.Get(b);
            var aTime = timeStore.Get(a);
            var bTime = timeStore.Get(b);

            //check if a allows to hit b and vice versa
            if (!LayerALlows(aData.Layer, aData.Mask, bData.Layer))
            {
                return;
            }
            if (!LayerALlows(bData.Layer, bData.Mask, aData.Layer))
            {
                return;
            }

            Vector2 delta = aTime.LocalPosition - bTime.LocalPosition;
            float distance = delta.LengthSquared;
            float radiusSum = aData.Radius + bData.Radius;
            bool hit = distance <= radiusSum * radiusSum;
            TrackEnterExit(a, b, hit, stateStore);

            if (!hit)
            {
                return;
            }

            if (aData.IsTrigger || bData.IsTrigger)
            {
                return;
            }

            float dist = MathF.Sqrt(distance);
            Vector2 collisionDerection = dist > 0 ? delta / dist : new Vector2(0, 1);

            float penetration = radiusSum - dist;

            aTime.LocalPosition += collisionDerection * penetration;
            aTime.Dirty = true;
            timeStore.Set(a, aTime);

            //stop velocity on the axis that collided
            if (velocityStore.Has(a))
            {
                var v = velocityStore.Get(a);
                float dot = Vector2.Dot(v.Velocity, collisionDerection);
                if (dot < 0)
                {
                    v.Velocity -= collisionDerection * dot;
                    velocityStore.Set(a, v);
                }
            }


        }


        private void HandleAabbVsCircle(
        //a => aabb , b => circle
        int a,
        int b,
        World.ComponentStore<TransformComponent> timeStore,
        World.ComponentStore<VelocityComponent> velocityStore,
        World.ComponentStore<AabbColliderComponent> aabbStore,
        World.ComponentStore<CircleColliderComponent> circleStore,
        World.ComponentStore<CollisionStateComponent> stateStore)
        {

            var aData = aabbStore.Get(a);
            var bData = circleStore.Get(b);
            var aTime = timeStore.Get(a);
            var bTime = timeStore.Get(b);

            //check if a allows to hit b and vice versa
            if (!LayerALlows(aData.Layer, aData.Mask, bData.Layer))
            {
                return;
            }
            if (!LayerALlows(bData.Layer, bData.Mask, aData.Layer))
            {
                return;
            }

            Vector2 aPosition = aTime.LocalPosition;
            Vector2 bPosition = bTime.LocalPosition;

            //shortest point on AABB to the center of the circle.
            Vector2 min = aPosition - aData.Halfsize;
            Vector2 max = aPosition + aData.Halfsize;
            Vector2 closest = new Vector2
                (
                    MathF.Max(min.X, MathF.Min(bPosition.X, max.X)),
                    MathF.Max(min.Y, MathF.Min(bPosition.Y, max.Y))
                );

            //measure distance
            Vector2 delta = closest - bPosition;
            float distance = delta.LengthSquared;
            bool hit = distance < bData.Radius * bData.Radius;
            TrackEnterExit(a, b, hit, stateStore);
            if (!hit)
            {
                return;
            }
            if (aData.IsTrigger || bData.IsTrigger)
            {
                return;
            }

            float dist = MathF.Sqrt(distance);
            Vector2 collisionDerection = dist > 0 ? delta / dist : new Vector2(0, 1);
            float penetration = bData.Radius - dist;

            //push AABB opposite direction
            aTime.LocalPosition += collisionDerection * penetration;
            aTime.Dirty = true;
            timeStore.Set(a, aTime);


            //stop velocity on the axis that collided
            if (velocityStore.Has(a))
            {
                var v = velocityStore.Get(a);
                float dot = Vector2.Dot(v.Velocity, collisionDerection);
                if (dot < 0)
                {
                    v.Velocity -= collisionDerection * dot;
                    velocityStore.Set(a, v);
                }
            }
        }
    }
}
