using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sober.Engine.Input
{
    public static class Input
    {
        //Mouse and Keyboard State Management
        private static KeyboardState _kb;
        private static MouseState _ms;
        private static KeyboardState _prevKb;
        private static MouseState _prevMs;

        public static Vector2 MousePosition => new Vector2(_ms.X, _ms.Y);
        public static Vector2 MouseDelta => new Vector2(_ms.X - _prevMs.X, _ms.Y - _prevMs.Y);
        public static float MouseWheelDelta => _ms.ScrollDelta.Y;

        public static void Update(KeyboardState kb, MouseState ms)
        {
            _prevKb = _kb;
            _prevMs = _ms;
            _kb = kb;
            _ms = ms;
        }

        //Input Methods for Keyboard and Mouse
        public static bool Down(Keys key)
        {
            return _kb.IsKeyDown(key);
        }
        public static void Pressed(Keys key)
        {
            if (_kb.IsKeyDown(key) && !_prevKb.IsKeyDown(key))
                return;
        }
        public static void Released(Keys key)
        {
            if (!_kb.IsKeyDown(key) && _prevKb.IsKeyDown(key))
                return;
        }
        public static bool MouseDown(MouseButton button)
        {
            return _ms.IsButtonDown(button);
        }
        public static bool MousePressed(MouseButton button)
        {
            return _ms.IsButtonDown(button) && !_prevMs.IsButtonDown(button);
        }
    }
}
