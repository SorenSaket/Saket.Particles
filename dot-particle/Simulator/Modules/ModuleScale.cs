using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Particles
{
    public class ModuleScale : IModule
    {
        public float[] ScaleX { get; set; }
        public float[] ScaleY { get; set; }

        public void Initialize(SimulatorCPU sim)
        {
            ScaleX = new float[sim.Count];
            ScaleY = new float[sim.Count];
        }
    }
}