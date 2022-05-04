using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Particles.Shapes
{
	public abstract class Shape
	{
		protected Vector2 position;

		/// <summary>
		/// Axis Aligned Bounding Box
		/// </summary>
		public abstract Vector2 AABB ();
		/// <summary>
		/// The center of the shape
		/// </summary>
		public Vector2 Position { get => position; set => position = value; }

		public float X => position.X;
		public float Y => position.Y;

		


		/// <summary>
		/// Checks if point is within the shape
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public abstract bool Contains(Vector2 point);

		public abstract Vector2 RandomPointWithin();
	}
}
