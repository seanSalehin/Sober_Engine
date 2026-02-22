using OpenTK.Mathematics;
using Sober.ECS.Components;
using Sober.Rendering.Particles;

namespace Sober.ECS.Systems
{
    public sealed class ParticleSystem : ISystem
    {


        private readonly World _world;
        private readonly ParticlePool _pool;
        private readonly Random _range = new ();
        private float random_between_0_1() => (float)_range.NextDouble();
        private static float random_between_a_b(float a, float b, float t) => a + (b - a) * t;

        public ParticleSystem(World world, ParticlePool pool)
        {
            _world = world;
            _pool = pool;
        }
        
        //create a particle
        private void spawnOne(Vector2 position, ParticleEmitterComponent emit, ParticleKind kind)
        {

            if (!_pool.TrySpawn(out int index))
            {
                return;
            }
            ref var p = ref _pool.Raw[index];
            float life = random_between_a_b(emit.LifeMin, emit.LifeMax, random_between_0_1());
            float speed = random_between_a_b(emit.SpeedMin, emit.SpeedMax, random_between_0_1());
            float size = random_between_a_b(emit.SizeMin, emit.SizeMax, random_between_0_1());

            //random direction
            float angle = random_between_0_1() *MathHelper.TwoPi;
            Vector2 direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));


            if (emit.SpeedMax <= 1.3f && emit.LifeMin >= 1.0f) 
            {
                direction.Y = MathF.Abs(direction.Y); 
            }

            p.Alive = true;
            p.Position = position;
            p.Velocity = direction * speed;
            p.Kind = kind;
            p.Drag = 0f;
            p.Gravity = Vector2.Zero;
            if (kind == ParticleKind.Spark)
            {
                p.Gravity = new Vector2(0f, -3.5f);
                p.Drag = 1.2f;
            }
            if (kind == ParticleKind.Smoke)
            {
                p.Drag = 0.8f;
                p.Gravity = new Vector2(0f, 0.08f);
            }

            p.Lifetime = life;
            p.LifeMax = life;
            p.Size = size;
            p.Color = emit.ColorMin;
            p.StartAlpha = emit.ColorMin.W;
        }


        public void Update(float dt)
        {
            var raw = _pool.Raw;
            var alive = _pool.ALive;
            for (var i = 0; i < alive.Count; i++)
            {
                int index = alive[i];
                ref var p = ref raw[index];
                if (!p.Alive) continue;
                p.Lifetime -= dt;
                if (p.Lifetime <= 0f)
                {
                    p.Alive = false;
                    continue;
                }

                //add gravity

                if (p.Kind == ParticleKind.Spark)
                {
                    p.Velocity += p.Gravity * dt;
                    p.Velocity *= MathF.Exp(-p.Drag * dt);
                }

                if (p.Kind == ParticleKind.Smoke)
                {
                    p.Velocity += p.Gravity * dt;
                    p.Velocity *= MathF.Exp(-p.Drag * dt);
                    p.Velocity.X += MathF.Sin(p.Lifetime * 3.5f) * 0.020f;
                }


                p.Position += p.Velocity * dt;
                float fade_near_end = p.Lifetime / p.LifeMax;

                //colors
                float t = 1f - fade_near_end; 
                if (p.Kind == ParticleKind.Spark) 
                {
                    Vector3 hot = new Vector3(1.00f, 0.90f, 0.25f); 
                    Vector3 mid = new Vector3(1.00f, 0.45f, 0.05f); 
                    Vector3 end = new Vector3(0.20f, 0.05f, 0.02f); 

                    Vector3 c = (t < 0.5f)
                        ? Vector3.Lerp(hot, mid, t * 2f)
                        : Vector3.Lerp(mid, end, (t - 0.5f) * 2f);

                    p.Color.X = c.X;
                    p.Color.Y = c.Y;
                    p.Color.Z = c.Z;
                }
                else if (p.Kind == ParticleKind.Flash) 
                {
                    Vector3 start = new Vector3(1.0f, 0.95f, 0.60f);
                    Vector3 end = new Vector3(1.0f, 0.35f, 0.05f);
                    Vector3 c = Vector3.Lerp(start, end, t);

                    p.Color.X = c.X;
                    p.Color.Y = c.Y;
                    p.Color.Z = c.Z;
                }
                else if (p.Kind == ParticleKind.Smoke) 
                {
                    Vector3 start = new Vector3(0.08f, 0.07f, 0.06f);
                    Vector3 end = new Vector3(0.14f, 0.14f, 0.14f);
                    Vector3 c = Vector3.Lerp(start, end, t);

                    p.Color.X = c.X;
                    p.Color.Y = c.Y;
                    p.Color.Z = c.Z;
                }


                p.Color.W = p.StartAlpha * fade_near_end;
            }
            _pool.RemoveDeadFromALiveList();

            var emitStore  = _world.GetStore<ParticleEmitterComponent>();
            var transformStore = _world.GetStore<TransformComponent>();
            foreach (var entity in emitStore.All())
            {
                int id = entity.Key;
                var emit = emitStore.Get(id);
                if(!emit.Enabled) continue;
                if(!transformStore.Has(id)) continue;
                emit.Accumulator += emit.Rate * dt;

                int spawnCount = (int)emit.Accumulator;
                if(spawnCount > 0)
                {
                    emit.Accumulator -= spawnCount;
                }
                var transform = transformStore.Get(id);
                for (int span = 0; span< spawnCount; span++)
                {
                    spawnOne(transform.LocalPosition, emit, ParticleKind.Spark);
                }
                emitStore.Set(id, emit);
            }

            //burst
            var burstStore = _world.GetStore<ParticleBurstRequest>();
            foreach (var entity in burstStore.All())
            { 
                int id = entity.Key;
                if (!transformStore.Has(id)) continue;
                var transform = transformStore.Get(id);
                var burst_request = burstStore.Get(id);

                var emit = burst_request.Type switch
                {
                    //flash
                    BurstType.Flash => new ParticleEmitterComponent(
                    rate: 0,
                    lifeMin: 0.08f, lifeMax: 0.16f,
                    sizeMin: 60f, sizeMax: 140f,
                    speedMin: 0.0f, speedMax: 0.3f,
                        color: new Vector4(1.0f, 0.92f, 0.55f, 1.0f)
                    ),
                    //smoke
                    BurstType.Smoke => new ParticleEmitterComponent(
                        rate: 0,
                        lifeMin: 3.0f, lifeMax: 5.5f,
                        sizeMin: 120f, sizeMax: 260f,
                        speedMin: 0.2f, speedMax: 0.6f,
                        color: new Vector4(0.18f, 0.16f, 0.14f, 0.55f)
                    ),
                    //sparks-fire
                    _ => new ParticleEmitterComponent(
                        rate: 0,
                        lifeMin: 0.3f, lifeMax: 0.9f,
                        sizeMin: 8f, sizeMax: 20f,
                        speedMin: 1.5f, speedMax: 5.0f,
                        color: new Vector4(1.0f, 0.65f, 0.10f, 1.0f)
                    ),
                };
                for (int i = 0; i< burst_request.Count; i++)
                 {
                    var kind =
                    burst_request.Type == BurstType.Flash ? ParticleKind.Flash :
                    burst_request.Type == BurstType.Smoke ? ParticleKind.Smoke :
                    ParticleKind.Spark;
                    spawnOne(transform.LocalPosition, emit, kind);
                }
                burstStore.Remove(id);
            }
        }


        public void Render()
        {
        }
    }
}
