using OpenTK.Graphics.OpenGL4;
using StbImageSharp;


namespace Sober.Rendering
{
    public class Texture :IDisposable
    {

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Handle { get; private set; }

        public Texture(string path)
        {
            using var stream = File.OpenRead(path);

            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            Width = image.Width;
            Height = image.Height;

            Handle = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, Handle);

            GL.TexImage2D(
                    TextureTarget.Texture2D, 0 , PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data
                );

            //Texture sampling parameters (UV)
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0);

        }

        public void Bind(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public void Dispose()
        {
            GL.DeleteTexture(Handle);
        }
    }
}

