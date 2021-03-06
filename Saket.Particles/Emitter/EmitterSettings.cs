using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Particles
{
	public class EmitterSettings
	{
		public float Duration { get; set; } = -1f;
		
		public float RateOverTime { get; set; } = 0f;
		/// <summary>
		/// pixels to move per spawn
		/// </summary>
		public float RateOverDistance { get; set; } = 0;

		public Burst[] Brusts { get; set; } = null;
	}

	public class Burst
	{
		public Burst(int count, float time = 0, int cycles = 1, float interval = 0, float probability = 1)
		{
			Time = time;
			Count = count;
			Cycles = cycles;
			Interval = interval;
			Probability = probability;
		}

		/// <summary> The time at which the burst should start </summary>
		public float Time { get; set; } = 0;
		/// <summary>The amount of particles to spawn </summary>
		public int Count { get; set; } = 30;
		/// <summary> On many times to spawn the burst  </summary>
		public int Cycles { get; set; } = 1;
		/// <summary> Time interval between spawns </summary>
		public float Interval { get; set; } = 1;
		/// <summary> Likelyhood of the burst spawning  </summary>
		public float Probability { get; set; } = 1;
	}

}
