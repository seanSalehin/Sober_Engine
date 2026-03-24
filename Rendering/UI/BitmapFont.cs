using OpenTK.Mathematics;

namespace Sober.Rendering.UI
{
    public sealed class BitmapFont
    {
        public int Cols = 16;
        public int Rows = 16;

        public Vector2 GetUvMin(char c)
        {
            int index = c;
            int col = index % Cols;
            int row = index / Cols;
            return new Vector2(col / (float)Cols, row / (float)Rows);
        }

        public Vector2 GetUvMax(char c)
        {
            var min = GetUvMin(c);
            return min + new Vector2(1f / Cols, 1f / Rows);
        }
    }
}
