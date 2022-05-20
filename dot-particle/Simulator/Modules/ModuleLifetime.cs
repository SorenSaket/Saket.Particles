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

            for (int i = 0; i < sim.Count; i++)
            {
                Lifetime[i] = 1f;
                CurrentLifetime[i] = 1f;
                LifeProgress[i] = 1f;
            }
        }

        public unsafe void Update(float delta, int startIndex, int endIndex)
        {
            // Why?
            delta = Math.Clamp(delta, 0f, 1f);
            if (Avx.IsSupported)
            {
                // Load delta
                var vectorDelta = Avx.BroadcastScalarToVector256(&delta);

                fixed (float* aPtr = Lifetime, bPtr = CurrentLifetime, cPtr = LifeProgress)
                {
                    for (int i = startIndex; i < endIndex; i += 8)
                    {
                        // Load Current life
                        var currentLifetime = Avx.LoadVector256(&bPtr[i]);
                        // Advance lifetime by delta
                        currentLifetime = Avx.Add(currentLifetime, vectorDelta);
                        // store current lifetime
                        Avx.Store(&bPtr[i], currentLifetime);

                        // divide currentlife by lifetime to get lifetimeprogress 0..inf (techinically no limit)
                        // Consider limiting lifetime
                        Avx.Store(&cPtr[i], Avx.Divide(currentLifetime, Avx.LoadVector256(&aPtr[i])));
                    }
                }
            }
            else
                throw new System.NotImplementedException();
            
        }
    }
}