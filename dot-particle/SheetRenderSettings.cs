using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
	/// <summary>
	/// 
	/// </summary>
	public class SheetRenderSettings
	{
		/// <summary> The texture of the particle </summary>
		public SheetAnimationType AnimationType => animType;
		public Rectangle[] Frames => frames;
		public Vector2 Origin => origin;

		public int Width { get; private set; }
		public int Height { get; private set; }

		private readonly SheetAnimationType animType;
		private readonly Rectangle[] frames;
		private readonly Vector2 origin;

		public SheetRenderSettings(int width, int height, SheetAnimationType animType = SheetAnimationType.None, int columns = 1, int rows = 1, int startElement = 0, int elements = -1)
		{
			this.animType = animType;

			// If no element count is already specified uses the whole texture
			if (elements <= 0)
				elements = columns * rows;

			// The frame count
			int frameCount = 1;
			// If animating the framecount should be equal to the selected frames.
			if (animType == SheetAnimationType.Anim)
				frameCount = elements - startElement;

			// The size of a single frame
			Width = width / columns;
			Height = height / rows;

			this.frames = new Rectangle[frameCount]; // initialize the frames with the computed framecount

			// TODO reimplement
			//if (animType == SheetAnimationType.Random)
			//	startElement = Randoms.Range(startElement, startElement + elements);

			// Compute each Rectangle/Frame
			for (int i = 0; i < frames.Length; i++)
				frames[i] = new Rectangle(((startElement + i) % rows) * Width, (((startElement + i) / rows)) * Height, Width, Height);

			// Compute origin as center 
			// Todo make customizable
			this.origin = new Vector2(Width, Height) / 2f;
		}

	}

	public enum SheetAnimationType
	{
		None,
		Single,
		Random,
		Anim
	}
}
