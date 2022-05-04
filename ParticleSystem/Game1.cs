using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Core.Particles;

	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private ParticleSystem _particleSystem;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = 1280;
			_graphics.PreferredBackBufferHeight = 720;

			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			// TODO: Remove in production
			_graphics.SynchronizeWithVerticalRetrace = false;
			IsFixedTimeStep = false;

			Window.AllowUserResizing = true;
			Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		protected override void Update(GameTime gameTime)
		{

		}

		protected override void Draw(GameTime gameTime)
		{
			
		}

		// Contant window aspect ratio
		float AspectRatio = 16f/9f;
		Point OldWindowSize;

		void Window_ClientSizeChanged(object sender, EventArgs e)
		{
			// https://stackoverflow.com/questions/8396677/uniformly-resizing-a-window-in-xna
		 
			// Remove this event handler, so we don't call it when we change the window size in here
			Window.ClientSizeChanged -= new EventHandler<EventArgs>(Window_ClientSizeChanged);

			if (Window.ClientBounds.Width != OldWindowSize.X)
			{ // We're changing the width
			  // Set the new backbuffer size
				_graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
				_graphics.PreferredBackBufferHeight = (int)(Window.ClientBounds.Width / AspectRatio);
			}
			else if (Window.ClientBounds.Height != OldWindowSize.Y)
			{ // we're changing the height
			  // Set the new backbuffer size
				_graphics.PreferredBackBufferWidth = (int)(Window.ClientBounds.Height * AspectRatio);
				_graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
			}

			_graphics.ApplyChanges();

			// Update the old window size with what it is currently
			OldWindowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);

			// add this event handler back
			Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
		}
	}
