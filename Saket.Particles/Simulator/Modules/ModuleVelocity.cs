using System;
using System.Collections.Generic;
using System.Text;

namespace Saket.Particles
{
    public class ModuleVelocity : IModule, IModuleSimulator
    {
        // --------- Positional State ---------
        /// <summary> Velocity X </summary>
        public float[] VelocityX { get; set; }
        /// <summary> Velocity Y </summary>
        public float[] VelocityY { get; set; }
        /// <summary> Velocity Z </summary>
        public float[] VelocityZ { get; set; }


        private ModulePosition _position;


        public void Initialize(SimulatorCPU sim)
        {
            _position = sim.GetModule<ModulePosition>();
            System.Diagnostics.Debug.Assert(_position != null);

            VelocityX = new float[sim.Count];
            VelocityY = new float[sim.Count];
            VelocityZ = new float[sim.Count];
        }

        public unsafe void Update(float delta, int startIndex, int endIndex)
        {
            // Movement
            CPUMath.Add(_position.PositionX, VelocityX, startIndex, endIndex);
            CPUMath.Add(_position.PositionY, VelocityY, startIndex, endIndex);
            CPUMath.Add(_position.PositionZ, VelocityZ, startIndex, endIndex);
        }
    }
}
