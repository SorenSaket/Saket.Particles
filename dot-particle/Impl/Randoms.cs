using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Particles
{
	public class Randoms
	{
		private static System.Random rnd = new System.Random();


		public static int Range(int inclMin, int exclMax)
		{
			return rnd.Next(inclMin, exclMax);
		}
		public static float Range(float min, float max)
		{
			return (float)(rnd.NextDouble() * (max - min) + min);
		}
		public static float Range01()
		{
			return (float)rnd.NextDouble();
		}
		public static float Rotation()
		{
			return (float)(rnd.NextDouble() * 2f * MathF.PI);
		}
	}
}
