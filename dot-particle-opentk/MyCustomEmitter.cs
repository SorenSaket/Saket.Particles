using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Particles
{
    internal class MyCustomEmitter : Emitter
	{
		private ModuleLifetime simulatorModuleLifetime;
		private ModulePosition simulatorModule2D;
		private ModuleColor simulatorModuleColor;


		public MyCustomEmitter(EmitterSettings settings, SimulatorCPU system) : base(settings, system)
        {
			simulatorModuleLifetime = system.GetModule<ModuleLifetime>();
			simulatorModule2D = system.GetModule<ModulePosition>();
			simulatorModuleColor = system.GetModule<ModuleColor>();
		}

        // Particle Settings
        public RandomDynamicFloat StartLifetime { get; set; } = 1f;
		public RandomDynamicFloat StartSpeed { get; set; } = 1f;
		public RandomDynamicFloat StartRotationalSpeed { get; set; } = 0;
		public RandomDynamicFloat StartSize { get; set; } = 1f;
		public RandomDynamicFloat StartRotation { get; set; } = 0f;
		public uint StartColor { get; set; } = Color.Green.PackedValue;

		public Gradient gradient = Gradient.Rainbow;

        protected override void SpawnParticle()
        {
			int particle = system.GetNextParticle();
			
			
			float rot = Randoms.Rotation();


			simulatorModule2D.PositionX[particle] = Position.X ;
			simulatorModule2D.PositionY[particle] = Position.Y;
			//simulatorModule2D.VelocityX[particle] = MathF.Cos(rot)*0.006f;
			//simulatorModule2D.VelocityY[particle] = MathF.Sin(rot) * 0.006f;

			//simulatorModule2D.Rotation[particle] = Randoms.Rotation();
			//simulatorModule2D.RotationalVelocity[particle] = 0.0004f;



			if(simulatorModuleColor != null)
            {
				simulatorModuleColor.Color[particle] = gradient.Evaluate(Randoms.Range01()).PackedValue;
			}

			if(simulatorModuleLifetime != null)
            {
				simulatorModuleLifetime.Lifetime[particle] = 1f;
				simulatorModuleLifetime.CurrentLifetime[particle] = 0f;
				simulatorModuleLifetime.LifeProgress[particle] = 0f;
			}

		}
	}
}
