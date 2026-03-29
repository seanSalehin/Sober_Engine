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

            float u = (col / (float)Cols) + 0.002f;
            float v = (row / (float)Rows) + 0.002f;

            return new Vector2(u, v);
        }

        public Vector2 GetUvMax(char c)
        {
            int index = c;
            int col = index % Cols;
            int row = index / Cols;

            float u = ((col + 1) / (float)Cols) - 0.002f;
            float v = ((row + 1) / (float)Rows) - 0.002f;

            return new Vector2(u, v);
        }
    }
}