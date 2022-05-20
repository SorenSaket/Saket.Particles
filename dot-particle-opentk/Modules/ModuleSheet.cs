using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Core.Particles
{
    public class ModuleSheet : IModule
    {
        /// <summary> Color </summary>
        public byte[] SpriteIndex { get; set; }

        public ModuleSheet(int texture2DArray)
        {

        }

        public void Initialize(SimulatorCPU sim)
        {
            SpriteIndex = new byte[sim.Count];
        }

    }
}
