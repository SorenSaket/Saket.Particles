using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Core
{
	public static class EvalUtils
    {
		public static T[] Quantize<T>(this IEvaluable<T> evaluable, int samples)
        {
			var result = new T[samples];

            for (int i = 0; i < result.Length; i++)
            {
				result[i] = evaluable.Evaluate(((float)i)/ ((float)samples));
			}

			return result;
        }

	}

	public interface IEvaluable<T>
    {
		public T Evaluate(float t);
    }


	public class Gradient : IEquatable<Gradient>, IEvaluable<Color>
	{
		public GradientPoint[] Points => colors;

		GradientPoint[] colors;


		public Gradient(GradientPoint[] colors)
		{
			this.colors = colors.OrderBy((x)=>x.pos).ToArray();
		}
		public Gradient(Color[] _colors)
		{
			this.colors = new GradientPoint[_colors.Length];
			for (int i = 0; i < _colors.Length; i++)
			{
				this.colors[i] = new GradientPoint((1f / (_colors.Length - 1f)) * (i), _colors[i]);
			}
		}

		public Color Evaluate(float t)
		{
			 t %= 1;

			GradientPoint col1 = colors[FloorIndex(t)];
			GradientPoint col2 = colors[CeilIndex(t)];
			return Color.Lerp(col1.color, col2.color, (col1.pos - t) / (col1.pos - col2.pos));
		}

		public Color EvaluateClamped(float t)
		{
			t = MathHelper.Clamp(t, 0, 1);

			GradientPoint col1 = colors[FloorIndex(t)];
			GradientPoint col2 = colors[CeilIndex(t)];
			return Color.Lerp(col1.color, col2.color, (col1.pos - t) / (col1.pos - col2.pos));
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

		public bool Equals([AllowNull] Gradient other)
		{
			return other == this;
		}


	}

	public struct GradientPoint
	{
		public float pos;
		public Color color;
		

		public GradientPoint(float pos, Color color)
		{
			this.pos = pos;
			this.color = color;
			
		}
	}

}
