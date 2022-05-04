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

namespace Core.Particles
{
	internal class OpenTKGame : GameWindow
    {

        private uint scencilIndex = 0;
        private RendererOpenTK renderer;
        ParticleSystem _particleSystem;
        Emitter emitter;
        public OpenTKGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
		{
            var settings = new ParticleSystemSettings()
            {
                MaxParticles = 1000,
                StartSpeed = 0f,
                StartSize = 64f
            };


            _particleSystem = new ParticleSystem(settings);


            var emitterSettings = new EmitterSettings()
            {
                RateOverTime = 100,
                Shape = new Shapes.Rectangle() { Size = new System.Numerics.Vector2(2f, 2f) }
            };
            emitter = new Emitter(emitterSettings, _particleSystem);
            emitter.Position = new System.Numerics.Vector2(-1f, -1f);
            _particleSystem.SpawnParticle(0.1f, 0.1f,0,0.1f,0.1f);

            renderer = new RendererOpenTK(_particleSystem);
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
            Debug.WriteLine(1f / e.Time);

        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            
            emitter.Update((float)e.Time);
        }
        
    }
}
