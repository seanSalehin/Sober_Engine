namespace Sober.Rendering.Mesh
{
    internal static class MeshFactory
    {
        public static Mesh CreateTriangle()
        {
            float[] vertices = {
                // positions        // colors
                // x      y      z         u      v
                  0.0f,  0.5f,  0f,    0.5f, 1f,       //top center
                   0.5f, -0.5f,  0f,    1f,   0f,      // bottom right
                  -0.5f, -0.5f,  0f,    0f,   0f,     // bottom left
            };
            return new Mesh(vertices);
        }



        public static Mesh CreateQuad()
        {
            float[] vertices = {
                // positions       
                // x      y    z   // U  V
                -0.5f,  0.5f, 0f,   0f, 1f, // top-left
                 0.5f,  0.5f, 0f,   1f, 1f, // top-right
                 0.5f, -0.5f, 0f,   1f, 0f, // bottom-right
                -0.5f, -0.5f, 0f,   0f, 0f  // bottom-left
            };
            int[] indices = { 0, 1, 2, 2, 3, 0 };
            return new Mesh(vertices, vertexStride: 5, indices: indices);
        }




        public static Mesh CreateRectangle(float width, float height)
        {
            float halfWidth = width / 2;
            float halfHeight = height / 2;
            float[] vertices = {
                // positions       // colors
                -halfWidth,  halfHeight, 0f,   1f,0f,0f,1f,  // top left
                 halfWidth,  halfHeight, 0f,   0f,1f,0f,1f,  // top right
                 halfWidth, -halfHeight, 0f,   0f,0f,1f,1f,  // bottom right
                -halfWidth, -halfHeight, 0f,   1f,1f,0f,1f, // bottom left
            };
            int[] indices = { 0, 1, 2, 2, 3, 0 };
            return new Mesh(vertices, indices: indices);
        }



        public static Mesh CreateHexagon(float radius)
        {
            float[] vertices = {
          // positions                            // colors
        // x                 y           z
        radius,          0f,         0f,        1f,0f,0f,1f,                   // right
        radius * 0.5f,   radius * 0.866f, 0f, 0f,1f,0f,1f,   // top right
        -radius * 0.5f,  radius * 0.866f, 0f, 0f,0f,1f,1f,   // top left
        -radius,         0f,         0f,     1f,1f,0f,1f,                   // left
        -radius * 0.5f, -radius * 0.866f, 0f, 1f,0f,1f,1f,   // bottom left
        radius * 0.5f,  -radius * 0.866f, 0f, 0f,1f,1f,1f,   // bottom right
    };
            int[] indices = { 0, 1, 2, 2, 3, 4, 4, 5, 0 };
            return new Mesh(vertices, indices: indices);
        }




        public static Mesh CreateCircle()
        {
            float[] vertices = {
        // positions        // colors
        // x           y           z
        0f,           0f,         0f,     1f,1f,1f,1f,   // center
        0.5f,         0f,         0f,     1f,0f,0f,1f,   // right
        0.433f,       0.25f,      0f,     0f,1f,0f,1f,   // top right
        0.25f,        0.433f,     0f,     0f,0f,1f,1f,   // top
        0f,           0.5f,       0f,     1f,1f,0f,1f,   // top left
        -0.25f,       0.433f,     0f,     1f,0f,1f,1f,   // left
        -0.433f,      0.25f,      0f,     0f,1f,1f,1f,   // bottom left
        -0.5f,        0f,         0f,     1f,0f,0f,1f,   // bottom
        -0.433f,     -0.25f,      0f,     0f,1f,0f,1f,   // bottom right
        -0.25f,      -0.433f,     0f,     0f,0f,1f,1f,   // right
        0f,          -0.5f,       0f,     1f,1f,0f,1f,   // bottom right
        0.25f,       -0.433f,     0f,     1f,0f,1f,1f,   // bottom
        0.433f,      -0.25f,      0f,     0f,1f,1f,1f,   // right
    };
            int[] indices = { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5, 0, 5, 6, 0, 6, 7, 0, 7, 8, 0, 8, 9, 0, 9, 10, 0, 10, 11, 0, 11, 12, 0, 12, 1 };
            return new Mesh(vertices, indices: indices);
        }

    }
}
