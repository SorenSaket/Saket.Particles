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
		public ParticleSystemSettings settings;

		public ParticleSOA particles;

		public int Count => particles.Length;

		private bool paused;

		private int currentCount = 0;

		private int nextParticleIndex = 0;

		/// <summary> Barrier used to secure sync across particle system threads</summary>
		private readonly Barrier barrier;
		/// <summary> Barrier used to secure sync across particle system threads</summary>
		private readonly int threads;
		/// <summary>count/threads</summary>
		private readonly int stride;
		/// <summary>   </summary>
		private readonly System.Random random;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="maxSize">Recommended sizes of power of 2048.</param>
		/// <param name="settings"></param>
		public SimulatorCPU(int count, ParticleSystemSettings settings, int seed = 0)
		{
			this.settings = settings;
			//this.threads = settings.MaxParticles / 512;

			// Allocate memory for particles
			this.particles = new ParticleSOA(count);
			// Keep threads to a minimum
			// Calculate number of threads from pagesize, particlecount 
			// Ensure that (particlecount/threads) % 8 == 0
			this.threads = 1;//Environment.ProcessorCount;

			this.barrier = new Barrier(threads + 1);
	
			this.stride = particles.Length / (threads);

			this.random = new System.Random(seed);
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
			while (!paused)
			{
				int thread = (int)data;

				int startIndex = thread * stride;

				int endIndex = ((thread + 1) * stride); // exclusive

				int range = endIndex - startIndex;


				// Progress lifetime with delta time
				CPUMath.Add(particles.CurrentLifetime, 1f/60f, startIndex, endIndex);

				// Calculate lifetime progress
				CPUMath.DivStore(particles.LifeProgress, particles.CurrentLifetime, particles.Lifetime, startIndex, endIndex);

				// SIMD curve evaluation
				// SIMD Random Number Generator

				// Limit velocity over lifetime
				CPUMath.Mult(particles.VelocityX, settings.Drag.Table[0], startIndex, endIndex);
				CPUMath.Mult(particles.VelocityY, settings.Drag.Table[0], startIndex, endIndex);



				// -------- Progress State --------

				CPUMath.Add(particles.PositionX, particles.VelocityX, startIndex, endIndex);
				CPUMath.Add(particles.PositionY, particles.VelocityY, startIndex, endIndex);

				barrier.SignalAndWait();
			}
		}

		public virtual void SpawnParticle(float positionX, float positionY, float rotation = 0, float velocityX = 0, float velocityY = 0)
		{
			// ---- Externally set values ---- 

			// Set position
			particles.PositionX[nextParticleIndex] = positionX;
			particles.PositionY[nextParticleIndex] = positionY;

			// Set Rotation
			particles.Rotation[nextParticleIndex] = rotation;

			// Set Velocity
			particles.VelocityX[nextParticleIndex] = velocityX;
			particles.VelocityY[nextParticleIndex] = velocityY;


			// ---- Randomly Sampled Values ----

			// Set lifetime
			particles.Lifetime[nextParticleIndex] = particles.CurrentLifetime[nextParticleIndex] = settings.StartLifetime.Sample(random.NextDouble());

			// Set Scale
			particles.ScaleX[nextParticleIndex] = settings.StartSize.Sample(random.NextDouble());
			particles.ScaleY[nextParticleIndex] = settings.StartSize.Sample(random.NextDouble());

			// Set color
			particles.Color[nextParticleIndex] = settings.StartColor.Sample(random.NextDouble());

			//particles[nextParticleIndex].RotationalVelocity = settings.StartRotationalSpeed;

			// ---- Advance Counters ----
			nextParticleIndex++;
			if (currentCount < particles.Length)
				currentCount++;
			if (nextParticleIndex >= particles.Length)
				nextParticleIndex = 0;
		}
	}
}