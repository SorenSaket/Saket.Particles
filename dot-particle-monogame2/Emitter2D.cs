using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Particles
{
    internal class Emitter2D : Emitter
	{
		private ModulePosition simulatorModule2D;

        public Emitter2D(EmitterSettings settings, SimulatorCPU system) : base(settings, system)
        {
			simulatorModule2D = system.GetModule<ModulePosition>();
        }

        // Particle Settings
        public RandomDynamicFloat StartLifetime { get; set; } = 1f;
		public RandomDynamicFloat StartSpeed { get; set; } = 1f;
		public RandomDynamicFloat StartRotationalSpeed { get; set; } = 0;
		public RandomDynamicFloat StartSize { get; set; } = 1f;
		public RandomDynamicFloat StartRotation { get; set; } = 0f;
		public uint StartColor { get; set; } = uint.MaxValue;

        protected override void SpawnParticle()
        {
			int particle = system.GetNextParticle();


			simulatorModule2D.PositionX[particle] = Position.X;
			simulatorModule2D.PositionY[particle] = Position.Y;
		}
	}
}
