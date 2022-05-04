using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;

namespace Core.Particles
{
	public class XNAGame : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private ParticleSystem _particleSystem;
		private RendererXNA renderer;
		private Emitter emitter;
		public XNAGame()
		{
			_graphics = new GraphicsDeviceManager(this);
	
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			
			
		}

		protected override void Initialize()
		{
			_graphics.PreferredBackBufferWidth = 1920;
			_graphics.PreferredBackBufferHeight = 1080;
			_graphics.ApplyChanges();
			_graphics.SynchronizeWithVerticalRetrace = false;
			

			// TODO: Add your initialization logic here


			var settings = new ParticleSystemSettings() {
				MaxParticles = 1000000,
				StartSpeed = 0f,
				StartSize = 64f,
				RenderSettings = new SheetRenderSettings(new Texture2D(GraphicsDevice,64,64))};


			_particleSystem = new ParticleSystem(settings);

			var emitterSettings = new EmitterSettings()
			{
				RateOverTime = 10000,
				Shape = new Shapes.Rectangle(){ Size= new System.Numerics.Vector2( 1920, 1080) }
			};
			emitter = new Emitter(emitterSettings, _particleSystem);
			emitter.Position = new System.Numerics.Vector2(-1920 / 2, -1080/2);

			renderer = new RendererXNA(GraphicsDevice, _particleSystem);

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_particleSystem.Play();
			emitter.Start();
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			emitter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			renderer.Render();
			base.Draw(gameTime);
		}
	}
}
