using System;

using System.Threading;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Diagnostics;
using System.Linq;

namespace Saket.Particles
{
	// TODO
	// Prewarming 
	// Simulation speed and scaling

	/// <summary>
	/// Not shuriken particle system (tm)
	/// </summary>
	public class SimulatorCPU
	{
		public SimulatorCPUSettings Settings { get; private set; }
		public float targetDelta;

		/// <summary>
		/// The Maximum number of particles of the system
		/// </summary>
		public readonly int Count;
		/// <summary>
		/// Whether the simulator is currently running
		/// </summary>
		public bool IsRunning => !stopped;

		/// <summary>
		/// 
		/// </summary>
		public long Tick => barrier.CurrentPhaseNumber;

		/// <summary>
		/// All modules
		/// </summary>
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
		protected Stopwatch stopwatch;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="maxSize">Recommended sizes of power of 2048</param>
		/// <param name="modules"></param>
		public SimulatorCPU(int count, IModule[] modules, int seed = 0, SimulatorCPUSettings settings = null)
		{
			this.Settings = settings;
			if (this.Settings == null)
				this.Settings = new SimulatorCPUSettings();
				
			this.threads = Math.Clamp(count / Settings.ParticlesPerThreadMin, 1, Environment.ProcessorCount);
			this.Count = (int)(Math.Ceiling((count / this.threads) / ((float)Settings.LargestStride)) * Settings.LargestStride * this.threads);
			Debug.WriteLine("Starting particle system with " + this.Count + " particles, across " + this.threads + " threads.");

			this.modules = modules;
			this.simulatorModules = modules.OfType<IModuleSimulator>().ToArray();
			this.targetDelta = (1f / Settings.TargetFrameRate);
			

			this.barrier = new Barrier(1);
		
			this.stride = this.Count / (this.threads);

			this.random = new System.Random(seed);

			this.spawnlock = new object();
			this.stopwatch = new Stopwatch();

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
                {
					currentCount++;
					if(Settings.IncreamentalStartup && (currentCount -1) % (stride) == 0)
                    {
						int index = (int)(currentCount / (stride - 1));
						Debug.WriteLine("Starting up thread: " + index);
						
						new Thread(new ParameterizedThreadStart(DoUpdateParticle)) { IsBackground = true }.Start(index);
					}
				}

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
					
					int threadCount = threads;
					// Start up workers
					if (Settings.IncreamentalStartup)
						threadCount = (int)MathF.Ceiling((float)currentCount / (float)stride);
					for (int i = 0; i < threadCount; i++)
					{
						new Thread(new ParameterizedThreadStart(DoUpdateParticle)) { IsBackground = true }.Start(i);
					}

					while (!stopped)
					{
						stopwatch.Reset();
						stopwatch.Start();
						barrier.SignalAndWait();
						stopwatch.Stop();
						Thread.Sleep((int)(targetDelta * 1000 - stopwatch.ElapsedMilliseconds));
					}

				})
			{ IsBackground = true }.Start();
		}
		
		/// <summary>
		/// The internal loop each thread performs each iteration
		/// </summary>
		/// <param name="data">A boxed int representing the thread ID</param>
		protected void DoUpdateParticle(object data)
		{
			barrier.AddParticipant();
			// Thead ID
			int thread = (int)data;
			// Starting index 
			int startIndex = thread * stride;
			// Ending index is exclusive
			int endIndex = ((thread + 1) * stride); 
			
			while (!stopped)
			{
                for (int i = 0; i < simulatorModules.Length; i++)
                {
					simulatorModules[i].Update(targetDelta, startIndex, endIndex);
				}
				barrier.SignalAndWait();
			}
			barrier.RemoveParticipant();
		}
	}
}