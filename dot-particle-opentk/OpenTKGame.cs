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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Core.Particles
{
	internal class OpenTKGame : GameWindow
    {

        private uint scencilIndex = 0;
        private RendererOpenTK renderer;
        SimulatorCPU _particleSystem;
        Emitter emitter;
        public OpenTKGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
		{
            Image<Rgba32> image = Image.Load<Rgba32>("./Assets/Texture/Smoke");

            var settings = new ParticleSystemSettings()
            {
                Drag = 1f,
                RenderSettings = new SheetRenderSettings(image.Width, image.Height, SheetAnimationType.Anim, 6, 6)
            };


            _particleSystem = new SimulatorCPU(800,settings);


            var emitterSettings = new EmitterSettings()
            {
                
                RateOverTime = 8,
                Shape = new Shapes.Rectangle() { Size = new System.Numerics.Vector2(2f, 2f) }
            };
            emitter = new Emitter(emitterSettings, _particleSystem);
            emitter.Position = new System.Numerics.Vector2(-1f, -1f);

            renderer = new RendererOpenTK(_particleSystem, image);
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

            renderer.Draw();

            SwapBuffers();

        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            
            emitter.Update((float)e.Time);
        }
        
    }
}
