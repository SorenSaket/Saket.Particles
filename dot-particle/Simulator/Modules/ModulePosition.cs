using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Particles
{
    public class ModulePosition : IModule
    {
        // --------- Positional State ---------

        /// <summary> X World Space coordinates </summary>
        public float[] PositionX { get; set; }
        /// <summary> Y World Space coordinates </summary>
        public float[] PositionY { get; set; }
        /// <summary> Y World Space coordinates </summary>
        public float[] PositionZ { get; set; }


        public void Initialize(SimulatorCPU sim)
        {
            PositionX = new float[sim.Count];
            PositionY = new float[sim.Count];
            PositionZ = new float[sim.Count];
        }
    }
}