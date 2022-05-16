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
		private ModuleVelocity simulatorVelocity;

		private ModuleScale simulatorScale;
		private ModuleRotation simulatorRotation;
		public MyCustomEmitter(EmitterSettings settings, SimulatorCPU system) : base(settings, system)
        {
			simulatorModuleLifetime = system.GetModule<ModuleLifetime>();
			simulatorModule2D = system.GetModule<ModulePosition>();
			simulatorModuleColor = system.GetModule<ModuleColor>();
			simulatorVelocity = system.GetModule<ModuleVelocity>();

			simulatorRotation = system.GetModule<ModuleRotation>();
			simulatorScale = system.GetModule<ModuleScale>();
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


			simulatorModule2D.PositionX[particle] = Position.X;
			simulatorModule2D.PositionY[particle] = Position.Y;
			simulatorModule2D.PositionZ[particle] = Position.Z;

			simulatorVelocity.VelocityX[particle] = MathF.Sin(rot) * 0.006f;
			simulatorVelocity.VelocityY[particle] = 0.006f;
			simulatorVelocity.VelocityZ[particle] = MathF.Cos(rot) * 0.006f;

			simulatorScale.ScaleX[particle] = 0.4f;
			simulatorScale.ScaleY[particle] = 6f;

			simulatorRotation.Rotation[particle] = rot;
			simulatorRotation.RotationalVelocity[particle] = 0f;



			if (simulatorModuleColor != null)
            {
				simulatorModuleColor.Color[particle] = gradient.Evaluate(Randoms.Range01()).PackedValue;
			}

			if(simulatorModuleLifetime != null)
            {
				simulatorModuleLifetime.Lifetime[particle] = 3f;
				simulatorModuleLifetime.CurrentLifetime[particle] = 0f;
				simulatorModuleLifetime.LifeProgress[particle] = 0f;
			}

		}
	}
}
