using OpenTK.Graphics.OpenGL4;
using StbImageSharp;


namespace Sober.Rendering
{
    public class Texture :IDisposable
    {

        //pointer/ID to a texture stored on the GPU that outside code cannot overwrite it
        public int Handle { get; private set; }

        public Texture(string path)
        {
            //Loads image from disk
            using var stream = File.OpenRead(path);

            //Read the image file, decodes it, forces it into RGBA format
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            //Create OpenGL texture
            Handle = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, Handle);

            //Upload pixel data to GPU
            GL.TexImage2D(
                    TextureTarget.Texture2D, 0 , PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data
                );

            //Texture sampling parameters (UV)
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //Pre-scaled versions of the texture
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Unbind
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

