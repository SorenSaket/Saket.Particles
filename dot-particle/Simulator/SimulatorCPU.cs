using System;

using System.Threading;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Diagnostics;
using System.Linq;

namespace Core.Particles
{
	// TODO
	// Prewarming 
	// Simulation speed and scaling

	/// <summary>
	/// Not shuriken particle system (tm)
	/// </summary>
	public class SimulatorCPU
	{
		private const int largestStride = 8;
		private const int particlesPerThreadMin = 2048;

		private const float delta = 1f/60f;
		public readonly int Count;

		public bool IsRunning => !stopped;
		public bool Stopped => stopped;


		public long Tick => barrier.CurrentPhaseNumber;
		public IModule[] Modules => modules;
		
		
		
		private IModule[] modules;
		private IModuleSimulator[] simulatorModules;


		protected bool stopped = true;

		protected int currentCount = 0;

		protected int nextParticleIndex = 0;

		/// <summary> Barrier used to secure sync across particle system threads</summary>
		protected readonly Barrier barrier;
		/// <summary> the number of threads to use </summary>
		protected readonly int threads;
		/// <summary>count/threads</summary>
		protected readonly int stride;
		/// <summary> Random Number Generator  </summary>
		protected readonly System.Random random;
		/// <summary> Lock used for spawning </summary>
		protected readonly object spawnlock;
		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="maxSize">Recommended sizes of power of 2048</param>
		/// <param name="modules"></param>
		public SimulatorCPU(int count, IModule[] modules, int seed = 0)
		{
			this.threads = Math.Clamp(count / particlesPerThreadMin,1, Environment.ProcessorCount);
			this.Count = (int)(Math.Ceiling((count / this.threads) / 8f) * 8 * this.threads);
			Debug.WriteLine("Starting particle system with " + this.Count + " particles, across " + this.threads + " threads.");

			this.modules = modules;
			this.simulatorModules = modules.OfType<IModuleSimulator>().ToArray();


			this.threads = Environment.ProcessorCount;

			this.barrier = new Barrier(threads + 1);
		
			this.stride = count / (threads);

			this.random = new System.Random(seed);

			spawnlock = new object();

            for (int i = 0; i < modules.Length; i++)
            {
				modules[i].Initialize(this);
			}
		}

		/// <summary> 
		/// Starts up thread 
		/// </summary>
		public void Play() { 
			if (!stopped) 
				return;
			stopped = false; 
			StartSimulation(); 
		}

		/// <summary> 
		/// Stops the simulation. All theads will exit. The simulator does not use CPU when stopped.
		/// </summary>
		public void Stop() { 
			stopped = true; 
		}


		/// <summary>
		/// Returns the module or null
		/// </summary>
		public T GetModule<T>() where T : IModule
		{
			for (int i = 0; i < modules.Length; i++)
			{
				if (modules[i] is T r)
					return r;
			}
			return default(T);
		}

		/// <summary>
		/// Returns the index of the next particle in the ring buffer
		/// </summary>
		public virtual int GetNextParticle()
		{
			// lock is nessesary to avoid race condition from multiple threads trying to spawn.
			lock (spawnlock)
			{
				int r = nextParticleIndex;
				// ---- Advance Counters ----
				nextParticleIndex++;
				if (currentCount < Count)
					currentCount++;

				if (nextParticleIndex >= Count)
                {
					nextParticleIndex = 0;
				}
				//
				return r;
			}
			
		}
		
		/// <summary> 
		/// Starts up threads
		/// </summary>
		protected void StartSimulation()
		{
			// Start up syncronization thread
			new Thread(
				() => {
					while (!stopped)
					{
						barrier.SignalAndWait();
						Thread.Sleep(16);
					}
				})
			{ IsBackground = true }.Start();

			// Start up workers
			// TODO start up workers incrementally when needed
			for (int i = 0; i < threads; i++)
			{
				new Thread(new ParameterizedThreadStart(DoUpdateParticle)) { IsBackground = true }.Start(i);
			}
		}
		
		/// <summary>
		/// The internal loop each thread performs each iteration
		/// </summary>
		/// <param name="data">A boxed int representing the thread ID</param>
		protected void DoUpdateParticle(object data)
		{
			// Thead ID
			int thread = (int)data;
			// Starting index 
			int startIndex = thread * stride;
			// Last index is exclusive
			int endIndex = ((thread + 1) * stride); 
			
			while (!stopped)
			{
                for (int i = 0; i < simulatorModules.Length; i++)
                {
					simulatorModules[i].Update(delta, startIndex, endIndex);
				}
				barrier.SignalAndWait();
			}
		}



	}
}