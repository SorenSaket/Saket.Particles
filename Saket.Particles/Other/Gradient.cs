using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Saket.Particles
{

	public abstract class Gradient<T> : IEvaluable<T>
	{
		public GradientPoint<T>[] Points => values;

		GradientPoint<T>[] values;


		public Gradient(GradientPoint<T>[] values)
		{
			this.values = values.OrderBy((x)=>x.pos).ToArray();
		}
		public Gradient(T[] values)
		{
			this.values = new GradientPoint<T>[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				this.values[i] = new GradientPoint<T>((1f / (values.Length - 1f)) * (i), values[i]);
			}
		}

		public T Evaluate(float t)
		{
			// Clamp t
			t = MathF.Min(1, MathF.Max(t, 0));

			GradientPoint<T> col1 = values[FloorIndex(t)];
			GradientPoint<T> col2 = values[CeilIndex(t)];
			//(col1.color) is an arbitrary instance of T to acess the non-static Lerp function from Ilerpable
			return Lerp(col1.color, col2.color, (col1.pos - t) / (col1.pos - col2.pos));
		}
		/// <summary>
		/// Returns index of Value that has the nearest smaller position
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		private int FloorIndex(float t)
		{
			for (int i = values.Length-1; i >= 0; i--)
			{
				if (values[i].pos <= t)
					return i;
			}
			return 0;
		}
		/// <summary>
		/// Returns index of Value that has the nearest higher position
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		private int CeilIndex(float t)
		{
			for (int i = 0; i < values.Length; i++)
			{
				if (values[i].pos > t)
					return i;
			}
			return values.Length-1;
		}

		public abstract T Lerp(T a, T b, float t);
	}

    public class ColorGradient : Gradient<uint>
    {
        public ColorGradient(GradientPoint<uint>[] values) : base(values)
        {
        }

        public override uint Lerp(uint a, uint b, float t)
        {
			byte Ared	= (byte)a;
			byte Agreen	= (byte)(a >> 8);
			byte Ablue	= (byte)(a >> 16);
			byte Aalpha	= (byte)(a >> 24);

			byte Bred	= (byte)b;
			byte Bgreen = (byte)(b >> 8);
			byte Bblue	= (byte)(b >> 16);
			byte Balpha = (byte)(b >> 24);

			return 
				(uint)(
				((byte)(Ared	+ (Bred - Ared)		* t) << 24) +
				((byte)(Agreen	+ (Bgreen - Agreen) * t) << 16) +
				((byte)(Ablue	+ (Bblue - Ablue)	* t) << 8) +
				((byte)(Aalpha	+ (Balpha - Aalpha) * t) << 0));
				
		}
    }

    public class FloatingGradient : Gradient<float>
    {
        public FloatingGradient(GradientPoint<float>[] values) : base(values)
        {
        }

        public override float Lerp(float a, float b, float t)
        {
			return (a + (b - a) * t);
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
