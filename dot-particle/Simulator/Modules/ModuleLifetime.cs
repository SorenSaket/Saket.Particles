using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Core.Particles
{
    public class ModuleLifetime : IModule, IModuleSimulator
    {   
        // --------- Lifetime ---------

        /// <summary> The absolute max lifetime in seconds </summary>
        public float[] Lifetime { get; set; }
        /// <summary> The absolute current lifetime in seconds </summary>
        public float[] CurrentLifetime { get; set; }
        /// <summary> Relative 0..1 lifetime progress. Saved to prevent recalculation </summary>
        public float[] LifeProgress { get; set; }

        private SimulatorCPU sim;

        public void Initialize(SimulatorCPU sim)
        {
            this.sim = sim;
            Lifetime        = new float[sim.Count];
            CurrentLifetime = new float[sim.Count];
            LifeProgress    = new float[sim.Count];
        }

        public unsafe void Update(float delta, int startIndex, int endIndex)
        {
            if (Avx.IsSupported)
            {
                // Load Constant
                var vectorDelta = Avx.BroadcastScalarToVector256(&delta);

                fixed (float* aPtr = Lifetime, bPtr = CurrentLifetime, cPtr = LifeProgress)
                {
                    for (int i = startIndex; i < endIndex; i += 8)
                    {
                        // Load Current life
                        var currentLifetime = Avx.LoadVector256(&bPtr[i]);
                        currentLifetime = Avx.Add(currentLifetime, vectorDelta);

                        Avx.Store(&bPtr[i], currentLifetime);
                        var totalLifeTime = Avx.LoadVector256(&aPtr[i]);
                        Avx.Store(&cPtr[i], Avx.Divide(currentLifetime, totalLifeTime));
                    }
                }
            }
            else
                throw new System.NotImplementedException();
            
        }
    }
}