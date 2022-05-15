using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using System;

namespace Core.Particles
{
	public class XNAGame : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private SimulatorCPU _particleSystem;
		private RendererXNA2D renderer;
		private Emitter2D emitter;

		private Matrix view;
		private Matrix projection;

		public XNAGame()
		{
			_graphics = new GraphicsDeviceManager(this);
			_graphics.GraphicsProfile = GraphicsProfile.HiDef;
			_graphics.ApplyChanges();

			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			
		}

		protected override void Initialize()
		{
			

			_graphics.PreferredBackBufferWidth = 1920;
			_graphics.PreferredBackBufferHeight = 1080;
			_graphics.ApplyChanges();
			// Disable v sync for testing 
			_graphics.SynchronizeWithVerticalRetrace = false;
			projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height, 0.1f, 100f);
			view = Matrix.CreateLookAt(new Vector3(0, 1, -3), Vector3.Zero, Vector3.UnitY);

			// Create Particle System
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
				RateOverTime = 1000,
			};

			emitter = new Emitter2D(emitterSettings, _particleSystem);
			emitter.Position = new System.Numerics.Vector3(0f, 0f, 0f);

			renderer = new RendererXNA2D(GraphicsDevice, Content.Load<Effect>("shader_particle"), _particleSystem);

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
			renderer.Update();
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CadetBlue);

			renderer.Draw(ref view, ref projection);



			base.Draw(gameTime);
		}
	}
}
