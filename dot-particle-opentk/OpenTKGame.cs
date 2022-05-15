using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Particles.Rendering;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Mathematics;

namespace Core.Particles
{
	internal class OpenTKGame : GameWindow
    {

        private uint scencilIndex = 0;
        private RendererOpenTK2D renderer;
        SimulatorCPU _particleSystem;
        Emitter emitter;

        Matrix4 view;
        Matrix4 projection;

        public OpenTKGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
		{
            

            /*
            GL.Enable(EnableCap.Texture3DExt);
            int texObject = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2DArray, texObject);
            using (var image = new Bitmap("./Assets/Texture/Smoke"))
            {
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
               
                var data = image.LockBits(
                  new Rectangle(0, 0, image.Width, image.Height),
                  ImageLockMode.ReadOnly,
                  System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                
                GL.TexImage3D(TextureTarget.Texture2DArray,
                 0,
                 PixelInternalFormat.Rgba,
                 image.Width/6,
                 image.Height/6,
                 6*6,
                 0,
                 OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                 PixelType.UnsignedByte,
                 data.Scan0);
            }
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
            */

            //Image<Rgba32> image = Image.Load<Rgba32>();



            _particleSystem = new SimulatorCPU(8000, new IModule[]
            {
                new ModuleLifetime(),
                new ModulePosition(),
                new ModuleRotation(),
                new ModuleVelocity(),
                new ModuleColor(),
            });

            var emitterSettings = new EmitterSettings()
            {
                RateOverTime = 800,
            };

            emitter = new MyCustomEmitter(emitterSettings, _particleSystem);
            emitter.Position = new System.Numerics.Vector3(0f, 0.0f, 0f);


            renderer = new RendererOpenTK2D(_particleSystem);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            _particleSystem.Play();
            emitter.Start();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.ClearStencil(0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit | ClearBufferMask.DepthBufferBit);
      

            renderer.Draw(ref view, ref projection);

            SwapBuffers();
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Size.X / Size.Y, 0.1f, 100.0f);
        }


        private float totalTime = 0;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            
            emitter.Update((float)e.Time);

            totalTime += (float)e.Time;

            view = Matrix4.LookAt(new Vector3(MathF.Sin(totalTime) * 3, 3, MathF.Cos(totalTime) * 3), Vector3.UnitX, Vector3.UnitY);
        }
        
    }
}
