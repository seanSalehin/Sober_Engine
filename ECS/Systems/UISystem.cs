using OpenTK.Mathematics;
using Sober.Rendering;
using Sober.Rendering.UI;

namespace Sober.ECS.Systems
{
    public sealed class UISystem : ISystem
    {
        private readonly UIRenderer _ui;
        private readonly int _screenW;
        private readonly int _screenH;
        private readonly Texture _fontTex;
        private float hpPercent = 0.6f;

        public UISystem(UIRenderer ui, Texture fontTex, int w, int h)
        {
            _ui = ui;
            _fontTex = fontTex;
            _screenW = w;
            _screenH = h;
        }

        public void Update(float dt) { }

        public void Render()
        {
            _ui.Begin();

            float offsetY = 20f;

            hpPercent = MathHelper.Clamp(hpPercent, 0f, 1f);

            int hpValue = (int)(hpPercent * 100);
            string leftLabel = "HP";
            string centerText = $"{hpValue}%";

            //panel
            var panel = new UITransform(
                Anchor.TopLeft,
                new Vector2(18, 18 + offsetY),
                new Vector2(320, 42)
            );
            _ui.DrawRect(panel, _screenW, _screenH, new Vector4(0.05f, 0.05f, 0.06f, 0.95f));

            //inner
            var inner = new UITransform(
                Anchor.TopLeft,
                new Vector2(20, 20 + offsetY),
                new Vector2(316, 38)
            );
            _ui.DrawRect(inner, _screenW, _screenH, new Vector4(0.10f, 0.10f, 0.12f, 1f));

            //label box
            var labelBox = new UITransform(
                Anchor.TopLeft,
                new Vector2(24, 24 + offsetY),
                new Vector2(52, 30)
            );
            _ui.DrawRect(labelBox, _screenW, _screenH, new Vector4(0.14f, 0.14f, 0.17f, 1f));

            //bar bg
            var barBg = new UITransform(
                Anchor.TopLeft,
                new Vector2(82, 24 + offsetY),
                new Vector2(250, 30)
            );
            _ui.DrawRect(barBg, _screenW, _screenH, new Vector4(0.12f, 0.12f, 0.14f, 1f));

            //bar
            Vector4 hpColor = new Vector4(
                1f - hpPercent * 0.85f,
                0.20f + hpPercent * 0.75f,
                0.10f,
                1f
            );

            var barFill = new UITransform(
                Anchor.TopLeft,
                new Vector2(84, 26 + offsetY),
                new Vector2((250 - 4) * hpPercent, 26)
            );
            _ui.DrawRect(barFill, _screenW, _screenH, hpColor);

            //highlight
            var highlight = new UITransform(
                Anchor.TopLeft,
                new Vector2(84, 26 + offsetY),
                new Vector2((250 - 4) * hpPercent, 8)
            );
            _ui.DrawRect(highlight, _screenW, _screenH, new Vector4(1f, 1f, 1f, 0.10f));

            //hp shadow
            var hpShadow = new UITransform(
                Anchor.TopLeft,
                new Vector2(35, 31 + offsetY),
                new Vector2(28, 16)
            );
            _ui.DrawText(leftLabel, hpShadow, _screenW, _screenH, new Vector4(0f, 0f, 0f, 1f), _fontTex);

            //hp text
            var hpTextTr = new UITransform(
                Anchor.TopLeft,
                new Vector2(34, 30 + offsetY),
                new Vector2(28, 16)
            );
            _ui.DrawText(leftLabel, hpTextTr, _screenW, _screenH, new Vector4(0.92f, 0.92f, 0.95f, 1f), _fontTex);

            //percent shadow
            var percentShadow = new UITransform(
                Anchor.TopLeft,
                new Vector2(185, 31 + offsetY),
                new Vector2(44, 16)
            );
            _ui.DrawText(centerText, percentShadow, _screenW, _screenH, new Vector4(0f, 0f, 0f, 1f), _fontTex);

            //percent text
            var percentText = new UITransform(
                Anchor.TopLeft,
                new Vector2(184, 30 + offsetY),
                new Vector2(44, 16)
            );
            _ui.DrawText(centerText, percentText, _screenW, _screenH, new Vector4(1f, 1f, 1f, 1f), _fontTex);

            _ui.End();
        }
    }
}