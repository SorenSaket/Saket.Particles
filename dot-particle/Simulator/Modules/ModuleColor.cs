using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Particles
{
    public class ModuleColor : IModule
    {
        /// <summary> Color </summary>
        /// R, G, B, A
        public uint[] Color { get; set; }

        public void Initialize(SimulatorCPU sim)
        {
            Color = new uint[sim.Count];

            for (int i = 0; i < Color.Length; i++)
            {
                Color[i] = uint.MaxValue;
            }
        }
    }
}
