using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Particles
{
    public class Emitter
	{   
		/// <summary> </summary>
		public Vector3 Position { get; set; }

		/// <summary> </summary>
		public bool Paused { get; set; } = true;
		
		/// <summary> </summary>
		public EmitterSettings settings;

		/// <summary> </summary>
		private float duration;

		/// <summary> Spawn particles over time </summary>
		private float spawnTimer;

		/// <summary> Spawn particles over distance </summary>
		private float distanceTraveled;

		/// <summary> Spawn particles over distance </summary>
		private Vector3 lastPosition;

		protected Action emitterAction;
		// -------- Lifetime --------
		public Emitter(EmitterSettings settings, Action emitterAction)
		{
			this.settings = settings;
			this.emitterAction = emitterAction;
		}
		public void Update(float delta)
		{
			if (Paused)
				return;

			float lastDuration = duration;
			duration += delta;

			// Deactivate object if done
			if (duration > settings.Duration && settings.Duration > 0)
			{
				Paused = true;
			}

			// Bursts
			if (settings.Brusts != null) 
			{ 
				// For earch burst
				for (int i = 0; i < settings.Brusts.Length; i++)
				{
					// If passed burst Time
					if (settings.Brusts[i].Time < duration && lastDuration <= settings.Brusts[i].Time)
					{
						// TODO probability, cycles & interval

						int count = settings.Brusts[i].Count;

						for (int y = 0; y < count; y++)
						{
							SpawnParticle();
						}
					}
				}
			}

			SpawnParticlesOverTime(delta);
			SpawnParticlesOverDistance();
		}

		// -------- Public --------
		public void Start()
		{
			Paused = false;
		}
		public void Restart()
		{
			duration = 0;
			spawnTimer = 0;
		}
		public void Stop()
		{

		}

		// -------- Private --------
		private void SpawnParticlesOverTime(float delta)
		{
			if (settings.RateOverTime <= 0)
				return;

			spawnTimer -= delta;

			//  
			if (spawnTimer <= 0)
			{
				// Needed to spawn more than one particle per frame

				// The time requied to spawn a single particle
				float timeBetweenSpawns = 1f / settings.RateOverTime;
				// if 
				int particlesToSpawn = (int) Math.Max( Math.Abs( spawnTimer / timeBetweenSpawns),1);


				for (int i = 0; i < particlesToSpawn; i++)
				{
					SpawnParticle();
				}
				spawnTimer = timeBetweenSpawns;
			}
		}
		private void SpawnParticlesOverDistance()
		{
			if (settings.RateOverDistance <= 0)
				return;

			distanceTraveled += Vector3.Distance(Position, lastPosition);
			lastPosition = Position;
			if (distanceTraveled >= settings.RateOverDistance)
			{
				SpawnParticle();
				distanceTraveled = 0;
			}
		}
		protected virtual void SpawnParticle()
        {
			emitterAction.Invoke();
        }
	}
}