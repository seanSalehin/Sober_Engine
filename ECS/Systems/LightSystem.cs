using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Sober.ECS.Components;
using Sober.Rendering.PostProcessing;
using Sober.Rendering.Shader;

namespace Sober.ECS.Systems
{
    public sealed class LightSystem : ISystem
    {
        private readonly World _world;
        private readonly ShaderProgram _lightShader;
        private readonly ScreenQuad _screenQuad;
        private int _screenWidth;
        private int _screenHeight;

        public LightSystem(World world, ShaderProgram lightShader, ScreenQuad screenQuad, int screenWidth, int screenHeight)
        {
            _world = world;
            _lightShader = lightShader;
            _screenQuad = screenQuad;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
        }

        public void Resize(int width, int height)
        {
            _screenWidth = width;
            _screenHeight = height;
        }

        public void Update(float dt) { }

        public void Render() { }

        public void RenderLightingPass(int sceneTextureId)
        {
            _lightShader.Bind();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, sceneTextureId);
            _lightShader.SetInt("u_Texture", 0);

            _lightShader.SetFloat("u_AspectRatio", (float)_screenWidth / _screenHeight);

            int lightCount = 0;

            foreach (int id in Query.with<TransformComponent, LightComponent>(_world))
            {
                if (lightCount >= 16) break;

                var t = _world.GetStore<TransformComponent>().Get(id);
                var l = _world.GetStore<LightComponent>().Get(id);

                Vector4 clipSpace = new Vector4(t.LocalPosition.X + l.Offset.X, t.LocalPosition.Y + l.Offset.Y, 0f, 1f) * CameraSystem.CurrentViewProj;
                Vector3 ndc = clipSpace.Xyz / clipSpace.W;
                Vector2 uv = new Vector2(ndc.X * 0.5f + 0.5f, ndc.Y * 0.5f + 0.5f);

                _lightShader.SetVector2($"u_LightUVs[{lightCount}]", uv);
                _lightShader.SetVector3($"u_LightColors[{lightCount}]", l.Color);
                _lightShader.SetFloat($"u_LightRadii[{lightCount}]", l.Radius);
                _lightShader.SetFloat($"u_LightIntensities[{lightCount}]", l.Intensity);

                lightCount++;
            }

            _lightShader.SetInt("u_LightCount", lightCount);

            GL.Disable(EnableCap.DepthTest);
            _screenQuad.Draw();
            GL.Enable(EnableCap.DepthTest);
        }
    
        }
    }
