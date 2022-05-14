using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Particles
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


        /// <summary>  World space rotaiton in radians </summary>
        public float[] Rotation { get; set; }
        /// <summary> Rotational Velocity in Radias Per Second</summary>
        public float[] RotationalVelocity { get; set; }

        private ModulePosition _position;


        public void Initialize(SimulatorCPU sim)
        {
            _position = sim.GetModule<ModulePosition>();
            System.Diagnostics.Debug.Assert(_position != null);

            VelocityX = new float[sim.Count];
            VelocityY = new float[sim.Count];
            VelocityZ = new float[sim.Count];


            Rotation = new float[sim.Count];
            RotationalVelocity = new float[sim.Count];
        }

        public unsafe void Update(float delta, int startIndex, int endIndex)
        {
            // Movement
            CPUMath.Add(_position.PositionX, VelocityX, startIndex, endIndex);
            CPUMath.Add(_position.PositionY, VelocityY, startIndex, endIndex);

            // Rotation
            CPUMath.Add(Rotation, RotationalVelocity, startIndex, endIndex);
        }
    }
}
