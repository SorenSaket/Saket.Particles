using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core.Particles
{
    public class Emitter
	{   
		/// <summary> </summary>
		public Vector2 Position { get; set; }

		/// <summary> </summary>
		public bool Paused { get; set; } = true;
		
		/// <summary> </summary>
		public EmitterSettings settings;

		public SimulatorCPU system;
		
		/// <summary> </summary>
		private float duration;

		/// <summary> Spawn particles over time </summary>
		private float spawnTimer;

		/// <summary> Spawn particles over distance </summary>
		private float distanceTraveled;

		/// <summary> Spawn particles over distance </summary>
		private Vector2 lastPosition;

		public Emitter(EmitterSettings settings, SimulatorCPU system)
		{
			this.settings = settings;
			this.system = system;
		}

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

						int count = settings.Brusts[i].Count.GetRandomValue;

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

			distanceTraveled += Vector2.Distance(Position, lastPosition);
			lastPosition = Position;
			if (distanceTraveled >= settings.RateOverDistance)
			{
				SpawnParticle();
				distanceTraveled = 0;
			}
		}

		private void SpawnParticle()
		{
			Vector2 localPos = settings.Shape.RandomPointWithin();
			Vector2 pos = Position + localPos;
			
			float rot = (MathF.Atan2(Position.Y-localPos.Y, Position.X - localPos.X)* settings.SpherizeDirection) + 
						(Randoms.Rotation() * settings.RandomizeRotation);


			system.SpawnParticle(
				pos.X,
				pos.Y,
				rot,
				MathF.Cos(rot) * 0.01f,
				MathF.Sin(rot) * 0.01f
				);
		}
	}
}