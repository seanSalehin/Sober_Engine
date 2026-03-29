using OpenTK.Mathematics;
using Sober.Rendering.UI;
using Sober.ECS;
using Sober.ECS.Components;
using Sober.Rendering;

namespace Sober.Editor
{
    public sealed class InspectorRenderer
    {
        private readonly World _world;
        private readonly RuntimeEditor _editor;
        private readonly UIRenderer _ui;
        private readonly Texture _font;

        public InspectorRenderer(World world, RuntimeEditor editor, UIRenderer ui, Texture font)
        {
            _world = world;
            _editor = editor;
            _ui = ui;
            _font = font;
        }

        public void Draw(int screenW, int screenH)
        {
            if (!_editor.IsOpen()) return;

            _ui.Begin();

            var sidebar = new UITransform(Anchor.TopRight, new Vector2(-320, 0), new Vector2(320, screenH));
            _ui.DrawRect(sidebar, screenW, screenH, new Vector4(0.08f, 0.08f, 0.10f, 0.98f));

            if (_editor.SelectedId() != -1)
            {
                int id = _editor.SelectedId();
                float y = 40;

                void DrawProp(string label, string val)
                {
                    var lblTr = new UITransform(Anchor.TopRight, new Vector2(-290, y), new Vector2(150, 16));
                    _ui.DrawText(label, lblTr, screenW, screenH, new Vector4(0.6f, 0.6f, 0.6f, 1f), _font);

                    var valTr = new UITransform(Anchor.TopRight, new Vector2(-140, y), new Vector2(120, 16));
                    _ui.DrawText(val, valTr, screenW, screenH, new Vector4(1f, 1f, 1f, 1f), _font);
                    y += 24;
                }

                void DrawHeader(string title, Vector4 color)
                {
                    y += 10;
                    var titleTr = new UITransform(Anchor.TopRight, new Vector2(-300, y), new Vector2(280, 20));
                    _ui.DrawText(title.ToUpper(), titleTr, screenW, screenH, color, _font);

                    var line = new UITransform(Anchor.TopRight, new Vector2(-300, y + 26), new Vector2(280, 1));
                    _ui.DrawRect(line, screenW, screenH, color * 0.6f);
                    y += 36;
                }

                DrawHeader("SELECTED ENTITY", new Vector4(0.4f, 0.8f, 1f, 1f));
                DrawProp("ID", id.ToString());

                if (_world.GetStore<TransformComponent>().Has(id))
                {
                    var t = _world.GetStore<TransformComponent>().Get(id);
                    DrawHeader("TRANSFORM", new Vector4(1f, 0.7f, 0.1f, 1f));
                    DrawProp("Pos X", t.LocalPosition.X.ToString("0.00"));
                    DrawProp("Pos Y", t.LocalPosition.Y.ToString("0.00"));
                    DrawProp("Scale X", t.LocalScale.X.ToString("0.00"));
                    DrawProp("Scale Y", t.LocalScale.Y.ToString("0.00"));
                }

                if (_world.GetStore<LightComponent>().Has(id))
                {
                    var l = _world.GetStore<LightComponent>().Get(id);
                    DrawHeader("LIGHT", new Vector4(1f, 0.9f, 0.3f, 1f));
                    DrawProp("Radius", l.Radius.ToString("0.00"));
                    DrawProp("Intensity", l.Intensity.ToString("0.00"));
                }

                if (_world.GetStore<TriggerZoneComponent>().Has(id))
                {
                    var tr = _world.GetStore<TriggerZoneComponent>().Get(id);
                    DrawHeader("TRIGGER", new Vector4(1f, 0.3f, 0.3f, 1f));
                    DrawProp("Width", (tr.HalfSize.X * 2).ToString("0.0"));
                    DrawProp("Height", (tr.HalfSize.Y * 2).ToString("0.0"));
                    DrawProp("State", tr.IsTriggered ? "ON" : "OFF");
                }
            }

            _ui.End();
        }
    }
}