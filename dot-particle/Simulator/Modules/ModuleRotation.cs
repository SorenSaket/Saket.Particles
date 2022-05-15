using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Particles
{
    public class ModuleRotation : IModule, IModuleSimulator
    {
        /// <summary>  World space rotaiton in radians </summary>
        public float[] Rotation { get; set; }
        /// <summary> Rotational Velocity in Radias Per Second</summary>
        public float[] RotationalVelocity { get; set; }

        public void Initialize(SimulatorCPU sim)
        {
            Rotation = new float[sim.Count];
            RotationalVelocity = new float[sim.Count];
        }

        public unsafe void Update(float delta, int startIndex, int endIndex)
        {
            // Rotation
            CPUMath.Add(Rotation, RotationalVelocity, startIndex, endIndex);
        }
    }
}
