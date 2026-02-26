using System;
using OpenTK.Graphics.OpenGL4;

namespace Sober.Rendering.PostProcessing
{
    public sealed class FrameBuffer : IDisposable
    {

        public int Fbo { get; private set; }
        public int ColorTexture { get; private set; }
        public int DepthRbo { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public FrameBuffer(int width, int height)
        {
            Resize(width, height);
        }


        public void Resize(int w, int h)
        {
            Width = w;
            Height = h;

            if (Fbo != 0)
            {
                GL.DeleteFramebuffer(Fbo);
            }
            if (ColorTexture != 0)
            {
                GL.DeleteTexture(ColorTexture);
            }
            if (DepthRbo != 0)
            {
                GL.DeleteRenderbuffer(DepthRbo);
            }

            var frameBuffer = GL.GenFramebuffer();
            Fbo = frameBuffer;
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);

            // create color texture
            var colorTexture = GL.GenTexture();
            ColorTexture = colorTexture;
            GL.BindTexture(TextureTarget.Texture2D, colorTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            //attach color texture to framebuffer
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, colorTexture, 0);

            DepthRbo = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthRbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer,RenderbufferStorage.Depth24Stencil8,Width, Height);

            //attach depth renderbuffer to framebuffer
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,FramebufferAttachment.DepthStencilAttachment,RenderbufferTarget.Renderbuffer,DepthRbo);

            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Framebuffer is not complete: " + status);
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        }
        

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Fbo);
            GL.Viewport(0, 0, Width, Height);
        }

        public void Unbind(int windowW, int windowH)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, windowW, windowH);
        }

        public void Dispose()
        {
            if (ColorTexture != 0) { GL.DeleteTexture(ColorTexture); }
            if (DepthRbo != 0) { GL.DeleteRenderbuffer(DepthRbo); }
            if (Fbo != 0) { GL.DeleteFramebuffer(Fbo); }
            ColorTexture = 0;
            DepthRbo = 0;
            Fbo = 0;
        }
    }
}
