using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Core.Particles
{

	public class Gradient<T> : IEvaluable<T> where T : ILerpable<T>
	{
		public GradientPoint<T>[] Points => colors;

		GradientPoint<T>[] colors;


		public Gradient(GradientPoint<T>[] colors)
		{
			this.colors = colors.OrderBy((x)=>x.pos).ToArray();
		}
		public Gradient(T[] _colors)
		{
			this.colors = new GradientPoint<T>[_colors.Length];
			for (int i = 0; i < _colors.Length; i++)
			{
				this.colors[i] = new GradientPoint<T>((1f / (_colors.Length - 1f)) * (i), _colors[i]);
			}
		}

		public T Evaluate(float t)
		{
			// Clamp t
			t = MathF.Min(1, MathF.Max(t, 0));

			GradientPoint<T> col1 = colors[FloorIndex(t)];
			GradientPoint<T> col2 = colors[CeilIndex(t)];
			//(col1.color) is an arbitrary instance of T to acess the non-static Lerp function from Ilerpable
			return (col1.color).Lerp(col1.color, col2.color, (col1.pos - t) / (col1.pos - col2.pos));
		}
		
		private int FloorIndex(float t)
		{
			for (int i = colors.Length-1; i >= 0; i--)
			{
				if (colors[i].pos <= t)
					return i;
			}
			return 0;
		}
		private int CeilIndex(float t)
		{
			for (int i = 0; i < colors.Length; i++)
			{
				if (colors[i].pos > t)
					return i;
			}
			return colors.Length-1;
		}

	
	}

	public struct GradientPoint<T>
	{
		public float pos;
		public T color;
		
		public GradientPoint(float pos, T color)
		{
			this.pos = pos;
			this.color = color;
		}
	}

}
