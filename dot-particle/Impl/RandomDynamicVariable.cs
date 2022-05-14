using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Random Dynamic is just some random keywords

namespace Core.Particles
{
	public class LookUpCurve<T>
	{
		public T[] Table { get; set; }
		
		public T Sample(double t)
		{
			return Table[(int)(t * Table.Length)];
		}


		public static implicit operator LookUpCurve<T>(T T) => new LookUpCurve<T>() { Table = new T[] {T}};

	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="C"></typeparam>
	public abstract class RandomDynamicVariable<T, C>
	{
		protected T min, max;
		protected C curveMin, curveMax;

		public bool HasValue => (!min.Equals(default(T)) || !max.Equals(default(T))) || (curveMin != null || curveMax != null);

		public RandomDynamicVariable(T min, T max, C curveMin, C curveMax)
		{
			this.min = min;
			this.max = max;
			this.curveMin = curveMin;
			this.curveMax = curveMax;
		}

		/// <summary>
		/// When set <see cref="GetRandomValue"></see> returns value
		/// </summary>
		/// <param name="value"></param>
		public void Constant(T value)
		{
			this.min = this.max = value;
			this.curveMin = this.curveMax = default(C);
		}

		/// <summary>
		/// When set <see cref="GetRandomValue"></see> returns a random value between min and max
		/// </summary>
		/// <param name="value"></param>
		public void RandomBetweenTwoConstants(T min, T max)
		{
			this.min = min;
			this.max = max;
			this.curveMin = this.curveMax = default(C);
		}

		/// <summary>
		/// When set <see cref="GetRandomValue"></see> evalutes the curve at a random position
		/// </summary>
		/// <param name="value"></param>
		public void Curve(C curve)
		{
			this.curveMin = this.curveMax = curve;
		}

		/// <summary>
		/// When set <see cref="GetRandomValue"></see> evalutes the min and max at a random position and returns a random number between them.
		/// </summary>
		/// <param name="value"></param>
		public void RandomBetweenTwoCurves(C min, C max)
		{
			curveMin = min;
			curveMax = max;
		}

		/// <summary>
		/// Returns the random value
		/// </summary>
		public T GetRandomValue => Evaluate(Randoms.Range01());

		/// <summary>
		/// Returns the random value at specific t
		/// </summary>
		public abstract T Evaluate(float t);
	}

	public class RandomDynamicColor : RandomDynamicVariable<Color, Gradient>
	{
		public RandomDynamicColor(Color min, Color max, Gradient curveMin, Gradient curveMax) : base(min, max, curveMin, curveMax)
		{
		}

		public override Color Evaluate(float t)
		{
			if (curveMin != null)
			{
				if (curveMax == curveMin)
					return curveMin.Evaluate(t);
				else
					return Color.Lerp(curveMin.Evaluate(t), curveMax.Evaluate(t), Randoms.Range01());
			}
			else
			{
				if (min == max)
					return min;
				else
					return Color.Lerp(min, max, t);
			}
		}
		// Color to RandomDynamicColor
		public static implicit operator RandomDynamicColor(Color f) => new RandomDynamicColor(f, f, null, null);
		// Gradient to RandomDynamicColor
		public static implicit operator RandomDynamicColor(Gradient c) => new RandomDynamicColor(Color.White, Color.White, c, c);
		// RandomDynamicColor to Color
		public static implicit operator Color(RandomDynamicColor x) => x.GetRandomValue;
	}

	public class RandomDynamicInt : RandomDynamicVariable<int, Curve>
	{
		public RandomDynamicInt(int min, int max, Curve curveMin, Curve curveMax) : base(min, max, curveMin, curveMax)
		{
		}

		public override int Evaluate(float t)
		{
			if (curveMin != null)
			{
				if (curveMax == curveMin)
					return (int)curveMin.Evaluate(t);
				else
				{
					return (int)Randoms.Range(curveMin.Evaluate(t), curveMax.Evaluate(t));
				}
			}
			else
			{
				if (min == max)
					return min;
				else
					return (int)MathHelper.Lerp(min, max, t);
			}
		}

		public static implicit operator RandomDynamicInt(int i) => new RandomDynamicInt(i, i, null, null);
		public static implicit operator int(RandomDynamicInt x) => x.GetRandomValue;
	}

	public class RandomDynamicFloat : RandomDynamicVariable<float, Curve>
	{
		public RandomDynamicFloat(float min, float max, Curve curveMin, Curve curveMax) : base(min, max, curveMin, curveMax)
		{
		}
		public override float Evaluate(float t)
		{
			if (curveMin != null)
			{
				if (curveMax == curveMin)
					return curveMin.Evaluate(t);
				else
				{
					return Randoms.Range(curveMin.Evaluate(t), curveMax.Evaluate(t));
				}
			}
			else
			{
				if (min == max)
					return min;
				else
					return MathHelper.Lerp(min, max, t);
			}
		}

		// float to RandomDynamicFloat
		public static implicit operator RandomDynamicFloat(float f) => new RandomDynamicFloat(f, f, null, null);
		// curve to RandomDynamicFloat
		public static implicit operator RandomDynamicFloat(Curve c) => new RandomDynamicFloat(0f, 0f, c, c);

		// min max float to RandomDynamicFloat
		public static implicit operator RandomDynamicFloat(Tuple<float, float> x) => new RandomDynamicFloat(x.Item1, x.Item2, null, null);
		public static implicit operator RandomDynamicFloat(Vector2 x) => new RandomDynamicFloat(x.X, x.Y, null, null);

		// RandomDynamicFloat to color
		public static implicit operator float(RandomDynamicFloat x) => x.GetRandomValue;
	}
}
