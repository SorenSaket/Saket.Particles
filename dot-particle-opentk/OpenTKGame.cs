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
        private RendererOpenTK2D renderer;
        SimulatorCPU _particleSystem;
        Emitter emitter;
        public OpenTKGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
		{
           //Image<Rgba32> image = Image.Load<Rgba32>("./Assets/Texture/Smoke");



            _particleSystem = new SimulatorCPU(256, new IModule[]
            {
                new ModuleLifetime(),
                new ModulePosition(),
                new ModuleRotation(),
                new ModuleVelocity(),
                new ModuleColor()
            });

            var emitterSettings = new EmitterSettings()
            {
                RateOverTime = 16,
            };

            emitter = new MyCustomEmitter(emitterSettings, _particleSystem);
            emitter.Position = new System.Numerics.Vector3(0.0f, 0.0f, 0f);


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
