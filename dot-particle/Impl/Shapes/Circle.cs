
using System;
using System.Numerics;

namespace Core.Particles.Shapes
{
	public class Circle : IShape
	{
		public float Radius {get; set;}

		public Vector2 RandomPointWithin()
		{
			float r = Radius * MathF.Sqrt(Randoms.Range01());
			float theta = Randoms.Rotation();
			return new 
				Vector2(r * MathF.Cos(theta),
						r * MathF.Sin(theta));
		}
	}
}
