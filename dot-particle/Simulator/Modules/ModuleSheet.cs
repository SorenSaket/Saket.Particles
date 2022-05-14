using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Core.Particles
{
    public class ModuleSheet : IModuleSimulator
    {
        /// <summary> Color </summary>
        public byte[] Count { get; set; }

        private SimulatorCPU simulator;
        private ModuleLifetime lifetime;

        public ModuleSheet(int maxCount)
        {
        }

        public void Initialize(SimulatorCPU sim)
        {
            Count = new byte[sim.Count];
            lifetime = sim.GetModule<ModuleLifetime>();
        }

        public unsafe void Update(float delta, int startIndex, int endIndex)
        {/*
            if (Avx.IsSupported)
            {

                fixed(float* cPtr = lifetime.LifeProgress)
                {
                    fixed (byte* aPtr = Count)
                    {
                        for (int i = startIndex; i < endIndex; i += 8)
                        {
                            
                        }
                    }
                }
               
            }
            else
                throw new System.NotImplementedException();*/
        }
    }
}
