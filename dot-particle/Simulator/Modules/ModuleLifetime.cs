using System;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics.Arm;

namespace Core.Particles
{
    /// <summary>
    /// Lifetime Module
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class ModuleLifetime : IModule, IModuleSimulator
    {   
        /// <summary> The absolute max lifetime in seconds </summary>
        public float[] Lifetime { get; set; }
        /// <summary> Relative 0..1 lifetime progress </summary>
        public float[] LifeProgress { get; set; }

        public void Initialize(SimulatorCPU sim)
        {
            Lifetime        = new float[sim.Count];
            LifeProgress    = new float[sim.Count];

            // Kill all particles by default
            //
            for (int i = 0; i < sim.Count; i++)
            {
                Lifetime[i] = 1f;
                LifeProgress[i] = 1f;
            }
        }

        public unsafe void Update(float delta, int startIndex, int endIndex)
        {
            // Clamp the delta
            //delta = Math.Clamp(delta, 0f, 1f);
            
            if (Avx.IsSupported)
            {
                // Load delta
                var vectorDelta = Avx.BroadcastScalarToVector256(&delta);

                fixed (float* ptrLifetime = Lifetime, ptrProgress = LifeProgress)
                {
                    for (int i = startIndex; i < endIndex; i += 8) 
                    {
                        // Advance lifetime by delta
                        Avx.Store(&ptrProgress[i], Avx.Add(Avx.LoadVector256(&ptrProgress[i]), Avx.Divide(vectorDelta, Avx.LoadVector256(&ptrLifetime[i]))));
                    }
                }
            }
            if (AdvSimd.Arm64.IsSupported)
            {
                var vectorDelta = AdvSimd.DuplicateToVector128(delta);

                fixed (float* ptrLifetime = Lifetime, ptrProgress = LifeProgress)
                {
                    for (int i = startIndex; i < endIndex; i += 4)
                    {
                        AdvSimd.Store(&ptrProgress[i], AdvSimd.Add(AdvSimd.LoadVector128(&ptrProgress[i]), AdvSimd.Arm64.Divide(vectorDelta, AdvSimd.LoadVector128(&ptrLifetime[i]))));
                    }
                }
            }
            else
            {
                
                for (int i = startIndex; i < endIndex; i++)
                {
                    LifeProgress[i] += delta / Lifetime[i];
                }
            }
        }
    }
}