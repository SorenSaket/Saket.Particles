
using System.Numerics;

namespace Core.Particles.Shapes
{
	public class Rectangle : IShape
	{
		public Vector2 Size { get; set; }

		public Vector2 RandomPointWithin()
		{
			return new Vector2(Size.X*Randoms.Range01(), Size.Y * Randoms.Range01());
		}
	}
}
