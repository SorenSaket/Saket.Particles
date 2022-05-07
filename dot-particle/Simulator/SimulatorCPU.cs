using System;

using System.Threading;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Diagnostics;


namespace Core.Particles
{
	// TODO
	// SOA
	// Custom Rendering 
	// SIMD 

	//
	// Prewarming 
	// Bursts ☑
	// Simulation speed and scaling
	// 

	/// <summary>
	/// Not shuriken particle system (tm)
	/// WÌP
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	public class SimulatorCPU
	{
		public long Tick => barrier.CurrentPhaseNumber;
		public ParticleSystemSettings settings;

		public ParticlePublic Particles => particles;
		protected ParticlePublic particles;

		public int Count => particles.Length;

		protected bool paused;

		protected int currentCount = 0;

		protected int nextParticleIndex = 0;

		/// <summary> Barrier used to secure sync across particle system threads</summary>
		protected readonly Barrier barrier;
		/// <summary> Barrier used to secure sync across particle system threads</summary>
		protected readonly int threads;
		/// <summary>count/threads</summary>
		protected readonly int stride;
		/// <summary>   </summary>
		protected readonly System.Random random;
		
		protected readonly object spawnlock;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="maxSize">Recommended sizes of power of 2048</param>
		/// <param name="settings"></param>
		public SimulatorCPU(int count, ParticleSystemSettings settings, int seed = 0)
		{
			this.settings = settings;
			//this.threads = settings.MaxParticles / 512;

			// Allocate memory for particles
			//count = (int)Math.Ceiling((count / Environment.ProcessorCount) / 8f) * 8;
			//Debug.WriteLine(count);

			this.particles = new ParticlePublic(count);
			// Keep threads to a minimum
			// Calculate number of threads from pagesize, particlecount 
			// Ensure that (particlecount/threads) % 8 == 0 to
			this.threads = Environment.ProcessorCount;

			this.barrier = new Barrier(threads + 1);
		
			this.stride = particles.Length / (threads);

			this.random = new System.Random(seed);

			spawnlock = new object();
		}

		public void Play() { paused = false; StartSimulation(); }
		public void Pause() => paused = true;


		protected void StartSimulation()
		{
			// TODO vectorize
			for (int i = 0; i < particles.Lifetime.Length; i++)
			{
				// Kill all particles
				particles.Lifetime[i] = -1;
			}

			// Start up syncronization thread
			new Thread(
				() => {
					while (!paused)
					{
						barrier.SignalAndWait();
						Thread.Sleep(16);
					}
				})
			{ IsBackground = true }.Start();

			// Start up workers
			for (int i = 0; i < threads; i++)
			{
				new Thread(new ParameterizedThreadStart(DoUpdateParticle)) { IsBackground = true }.Start(i);
			}
		}

		protected void DoUpdateParticle(object data)
		{
			int thread = (int)data;

			int startIndex = thread * stride;

			int endIndex = ((thread + 1) * stride); // exclusive

			int range = endIndex - startIndex;

			while (!paused)
			{
				// Progress lifetime with delta time
				CPUMath.Add(particles.CurrentLifetime, 1f/60f, startIndex, endIndex);

				// Calculate lifetime progress
				// Divide currentlifetime with lifetime and store in lifeprogress. 
				CPUMath.DivStore(particles.LifeProgress, particles.CurrentLifetime, particles.Lifetime, startIndex, endIndex);

				// SIMD curve evaluation
				// SIMD Random Number Generator

				// Limit velocity over lifetime
				CPUMath.Mult(particles.VelocityX, settings.Drag.Table[0], startIndex, endIndex);
				CPUMath.Mult(particles.VelocityY, settings.Drag.Table[0], startIndex, endIndex);


				// -------- Progress State --------
				CPUMath.Add(particles.PositionX, particles.VelocityX, startIndex, endIndex);
				CPUMath.Add(particles.PositionY, particles.VelocityY, startIndex, endIndex);

				CPUMath.Add(particles.Rotation, particles.RotationalVelocity, startIndex, endIndex);


				barrier.SignalAndWait();
			}
		}

		public virtual void SpawnParticle(Particle particle)
		{
			lock (spawnlock)
			{
				particles.PositionX[nextParticleIndex] = particle.PositionX;
				particles.PositionY[nextParticleIndex] = particle.PositionY;
				particles.Rotation[nextParticleIndex] = particle.Rotation;
				particles.RotationalVelocity[nextParticleIndex] = particle.RotationalVelocity;
				particles.VelocityX[nextParticleIndex] = particle.VelocityX;
				particles.VelocityY[nextParticleIndex] = particle.VelocityY;
				particles.Lifetime[nextParticleIndex] = particle.Lifetime;
				particles.CurrentLifetime[nextParticleIndex] = particles.LifeProgress[nextParticleIndex] = 0;
				particles.ScaleX[nextParticleIndex] = particle.ScaleX;
				particles.ScaleY[nextParticleIndex] = particle.ScaleY;
				particles.Color[nextParticleIndex] = particle.Color;


				// ---- Advance Counters ----
				nextParticleIndex++;
				if (currentCount < particles.Length)
					currentCount++;
				if (nextParticleIndex >= particles.Length)
					nextParticleIndex = 0;
			}
			
		}
	}
}