using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Saket.Particles
{
    public class ModuleSheet : IModule
    {
        /// <summary> Color </summary>
        public byte[] SpriteIndex { get; set; }

        public void Initialize(SimulatorCPU sim)
        {
            SpriteIndex = new byte[sim.Count];
        }

    }
}
