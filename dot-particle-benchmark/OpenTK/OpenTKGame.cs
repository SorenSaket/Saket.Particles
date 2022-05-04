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
                MaxParticles = 500000,
                StartSpeed = 0f,
                StartSize = 64f
            };


            _particleSystem = new ParticleSystem(settings);


            var emitterSettings = new EmitterSettings()
            {
                RateOverTime = 10000,
                Shape = new Shapes.Rectangle() { Size = new System.Numerics.Vector2(1f, 1f) }
            };
            emitter = new Emitter(emitterSettings, _particleSystem);
            emitter.Position = new System.Numerics.Vector2(-0.5f, -0.5f);
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
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            Debug.WriteLine(_particleSystem.particles.PositionX[0]);

            emitter.Update((float)e.Time);
        }
        
    }
}
