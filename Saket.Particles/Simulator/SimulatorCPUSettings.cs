using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Particles
{
    public class SimulatorCPUSettings
    {
		/// <summary>
		/// Whether to use incremental startup
		/// </summary>
		public bool IncreamentalStartup { get; set; } = true;
		/// <summary>
		/// The larges common stride in bytes. Since we're targeting 256-bit wide SIMD the largest stride is 8 (256/32 = 8). In case of 512 width change to 16.
		/// </summary>
		public int LargestStride  { get; set; } = 8;
		/// <summary>
		/// Currently arbitrary numer designating the least amount of particles required to start another thread.
		/// </summary>
		public int ParticlesPerThreadMin { get; set; } = 2048;
		/// <summary>
		/// TODO make syncronization more robust. Currently no modules rely on the delta and the delta is const.
		/// </summary>
		public float TargetFrameRate  { get; set; } = 30;
	}
}
