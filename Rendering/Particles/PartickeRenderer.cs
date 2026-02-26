using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Sober.Rendering.Particles
{
    public sealed class PartickeRenderer : IDisposable
    {

        private readonly int _vao;
        private readonly int _vbo;
        private readonly float[] _cpu;
        private readonly int _max;

        //In sober engin, I follow this pattern:[X  Y  R   G  B  A  Size] = position (x y), color (r g b a), size => 7 floats for each particles

        public PartickeRenderer(int maxParticles)
        {
            _max = maxParticles;
            _cpu = new float[_max * 7];
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            GL.BindVertexArray(_vao);
            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _cpu.Length * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);
            int scale = 7 * sizeof(float);


            //position (slot 0) 
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, scale, 0);

            //color (slot 1)
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, scale, 2 * sizeof(float));

            //size (slot 2)
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, scale, 6 * sizeof(float));

            GL.BindVertexArray(0);
        }


        public void Draw(ParticlePool pool, int aliveCount, Matrix4 viewProject, int shaderProgramId)
        {
            GL.UseProgram(shaderProgramId);

            int locViewProj = GL.GetUniformLocation(shaderProgramId, "u_ViewProj");
            if (locViewProj != -1)
                GL.UniformMatrix4(locViewProj, false, ref viewProject);

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.Enable(EnableCap.Blend);

            // PASS 1: Additive (Sparks + Flash)
            DrawPass(pool, aliveCount, additive: true);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            GL.DrawArrays(PrimitiveType.Points, 0, _lastCount);

            // PASS 2: Alpha (Smoke)
            DrawPass(pool, aliveCount, additive: false);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.DrawArrays(PrimitiveType.Points, 0, _lastCount);

            GL.BindVertexArray(0);
        }

        private int _lastCount;

        private void DrawPass(ParticlePool pool, int aliveCount, bool additive)
        {
            var particles = pool.Raw;
            var alive = pool.ALive;

            int write = 0;

            for (int i = 0; i < aliveCount; i++)
            {
                int index = alive[i];
                ref var p = ref particles[index];
                if (!p.Alive) continue;

                bool isAdd = (p.Kind == ParticleKind.Spark) || (p.Kind == ParticleKind.Flash);
                if (isAdd != additive) continue;

                _cpu[write++] = p.Position.X;
                _cpu[write++] = p.Position.Y;
                _cpu[write++] = p.Color.X;
                _cpu[write++] = p.Color.Y;
                _cpu[write++] = p.Color.Z;
                _cpu[write++] = p.Color.W;
                _cpu[write++] = p.Size;
            }

            int floatsUsed = write;
            int bytesUsed = floatsUsed * sizeof(float);
            _lastCount = floatsUsed / 7;

            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, bytesUsed, _cpu);
        }


        public void Dispose()
        {
            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
        }
    }
}
