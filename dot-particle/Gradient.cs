using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Core
{
	public class Gradient : IEquatable<Gradient>
	{
		public GradientPoint[] Points => colors;

		GradientPoint[] colors;


		public Gradient()
		{
			colors = new GradientPoint[] {
					new GradientPoint(Color.White, 0),
					new GradientPoint(Color.White, 1),
				};
		}
		public Gradient(GradientPoint[] colors)
		{
			this.colors = colors.OrderBy((x)=>x.pos).ToArray();
		}
		public Gradient(Color[] _colors)
		{
			this.colors = new GradientPoint[_colors.Length];
			for (int i = 0; i < _colors.Length; i++)
			{
				this.colors[i] = new GradientPoint(_colors[i], (1f / (_colors.Length-1f)) * (i));
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

		static Gradient()
		{
			White = new Gradient(new GradientPoint[] {
					new GradientPoint(Color.White, 0),
					new GradientPoint(Color.White, 1),
				});
			Rainbow = new Gradient(new GradientPoint[] {
				new GradientPoint(Color.Red, 0),
				new GradientPoint(Color.Yellow, (1f/5f)*1f),
				new GradientPoint(Color.Green,(1f/5f)*2f),
				new GradientPoint(Color.Cyan, (1f/5f)*3f),
				new GradientPoint(Color.Blue, (1f/5f)*4f),
				new GradientPoint(Color.Magenta, (1f/5f)*5f),
			});
			BlackToWhite = new Gradient(new GradientPoint[] {
				new GradientPoint(Color.Black, 0),
				new GradientPoint(Color.White, 1),
			});
			Fire = new Gradient(new GradientPoint[] {
				new GradientPoint(Color.White, 0),
				new GradientPoint(Color.Yellow, 0.1f),
				new GradientPoint(Color.Red,0.5f),
				new GradientPoint(Color.Black,1f) });
		}

		public static Gradient White { get; private set; }

		public static Gradient Rainbow { get; private set; }

		public static Gradient BlackToWhite { get; private set; }

		public static Gradient Fire { get; private set; }

	}

	public struct GradientPoint
	{
		public Color color;
		public float pos;

		public GradientPoint(Color color, float pos)
		{
			this.color = color;
			this.pos = pos;
		}
	}

}
