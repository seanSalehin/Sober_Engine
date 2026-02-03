using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Sober.ECS.Components;
using Sober.Engine.Core;
using Sober.Engine.Input;

namespace Sober.ECS.Systems
{
    public sealed class CameraSystem : ISystem
    {

        private readonly World _world;
        public static Matrix4 CurrentViewProj { get; private set; } = Matrix4.Identity;
        public CameraSystem(World world)
        {
            _world = world;
        }

        public void Render()
        {
            //TODO
        }

        public void Update(float dt)
        {
            var camStore = _world.GetStore<CameraComponent>();
            var followStore = _world.GetStore<CameraFollowComponent>();
            var tSore = _world.GetStore<TransformComponent>();

            foreach (var kv in camStore.All())
            {
                int canId = kv.Key;
                var cam = kv.Value;
                if (followStore.Has(canId))
                {
                    var follow = followStore.Get(canId);
                    if (tSore.Has(follow.TargetEntity))
                    {
                        var targetT = tSore.Get(follow.TargetEntity);
                        Vector2 targetPos = targetT.WorldMatrix.ExtractTranslation().Xy;
                        Vector2 delta = targetPos - cam.Position;

                        //dead-zone check
                        if (System.MathF.Abs(delta.X) < follow.DeadZone.X)
                        {
                            delta.X = 0;
                        }
                        if (System.MathF.Abs(delta.Y) < follow.DeadZone.Y)
                        {
                            delta.Y = 0;
                        }

                        // smoothing

                        cam.Position += delta * (1f - System.MathF.Exp(-follow.Smoothing * Time.DeltaTime));
                        cam.Dirty = true;

                    }
                }
                // Zoom with mouse wheel
                float wheel = Input.MouseWheelDelta;
                if (wheel != 0)
                {
                    cam.Zoom *= (wheel > 0) ? 1.1f : 0.9f;
                    cam.Zoom = MathHelper.Clamp(cam.Zoom, 0.2f, 4f);
                    cam.Dirty = true;
                }
                if (cam.Dirty)
                {
                    float aspect = 16f / 9f;             //TODO: get actual aspect ratio
                    float halfH = cam.Size / cam.Zoom;
                    float halfW = halfH * aspect;

                    var proj = Matrix4.CreateOrthographicOffCenter(-halfW, halfW, -halfH, halfH, -1f, 1f);
                    var view = Matrix4.CreateTranslation(-cam.Position.X, -cam.Position.Y, 0f) * Matrix4.CreateRotationZ(-cam.Rotation);
                    cam.ViewProj = view * proj;
                    CurrentViewProj = cam.ViewProj;
                    cam.Dirty = false;
                    camStore.Set(canId, cam);
                }
                break;
            }
        }

    }


        internal static class MatrixExtensions
        {
            public static Vector3 ExtractTranslation(this Matrix4 m) => m.ExtractTranslation();
        }
    }
