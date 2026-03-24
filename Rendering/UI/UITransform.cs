using OpenTK.Mathematics;

namespace Sober.Rendering.UI
{


    public enum Anchor
    {
        TopLeft,
        TopCenter,
        TopRight,
        CenterLeft,
        Center,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }


    public  struct UITransform
    {
        public Anchor Anchor { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }


        public UITransform(Anchor anchor, Vector2 position, Vector2 size)
        {
            Anchor = anchor;
            Position = position;
            Size = size;
        }


        public void NdcRect (int screenWidth, int screenHeight, out Vector2 min, out Vector2 max)
        {

            Vector2 anchorPos;

            switch (Anchor)
            {
                case Anchor.TopLeft:
                    anchorPos = Vector2.Zero;
                    break;
                case Anchor.TopCenter:
                    anchorPos = new Vector2(screenWidth/2, 0);
                    break;
                case Anchor.TopRight:
                    anchorPos = new Vector2(screenWidth, 0);
                    break;
                case Anchor.CenterLeft:
                    anchorPos = new Vector2(0,  screenHeight / 2);
                    break;
                case Anchor.Center:
                    anchorPos = new Vector2(screenWidth/ 2, screenHeight/2);
                    break;
                case Anchor.CenterRight:
                    anchorPos   = new Vector2(screenWidth , screenHeight/2);
                    break;
                case Anchor.BottomLeft:
                    anchorPos   = new Vector2(0, screenHeight);
                    break;
                case Anchor.BottomCenter:
                    anchorPos = new Vector2(screenWidth / 2, screenHeight);
                    break;
                case Anchor.BottomRight:
                    anchorPos = new Vector2(screenWidth, screenHeight);
                    break;
                default:
                    anchorPos = Vector2.Zero;
                    break;
            }

            Vector2 rectPos = anchorPos + Position;
            
            Vector2 pixelMin  = rectPos;
            Vector2 pixelMax = rectPos + Size;
            float ndcX, ndcY, ndcX2, ndcY2;

            ndcX = (pixelMin.X / screenWidth) * 2 - 1;
            ndcY = 1 - (pixelMin.Y / screenHeight) * 2;
            ndcX2 = (pixelMax.X / screenWidth) * 2 - 1;
            ndcY2 = 1 - (pixelMax.Y / screenHeight) * 2;

            min = new Vector2(ndcX, ndcY);
            max = new Vector2(ndcX2, ndcY2);

            if(min.Y > max.Y)
            {
                var temp = min.Y;
                min.Y = max.Y;
                max.Y = temp;
            }

            if (min.X > max.X)
            {
                var temp = min.X;
                min.X = max.X;
                max.X = temp;
            }


            }

    }
}
