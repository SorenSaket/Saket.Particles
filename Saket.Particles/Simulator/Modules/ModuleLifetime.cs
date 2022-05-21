using System;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics;

namespace Saket.Particles
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
       
        
        protected Vector256<float> vec256_one;
        protected Vector128<float> vec128_one;
        protected Vector64<float> vec64_one;

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

          
            vec256_one = Vector256.Create(1f);
            vec128_one = Vector128.Create(1f);
            vec64_one = Vector64.Create(1f);
            
        }

        public unsafe void Update(float delta, int startIndex, int endIndex)
        {
            // Clamp the delta
            //delta = Math.Clamp(delta, 0f, 1f);
            
            if (Avx.IsSupported) // 256
            {
                // Load delta
                var vectorDelta = Avx.BroadcastScalarToVector256(&delta);

                fixed (float* ptrLifetime = Lifetime, ptrProgress = LifeProgress)
                {
                    for (int i = startIndex; i < endIndex; i += 8) 
                    {
                        // 
                        var vec_factor = Avx.Divide(vectorDelta, Avx.LoadVector256(&ptrLifetime[i]));
                        // Progress Lifetime
                        var vec_lifetime = Avx.Add(Avx.LoadVector256(&ptrProgress[i]), vec_factor);
                        // Clamp to one
                        vec_lifetime = Avx.Min(vec_lifetime, vec256_one);
                        //
                        Avx.Store(&ptrProgress[i], vec_lifetime);
                    }
                }
            }
            else if (Sse.IsSupported) // 128
            {
                // Load delta
                var vectorDelta = Sse.LoadScalarVector128(&delta);

                fixed (float* ptrLifetime = Lifetime, ptrProgress = LifeProgress)
                {
                    for (int i = startIndex; i < endIndex; i += 4)
                    {
                        // 
                        var vec_factor = Sse.Divide(vectorDelta, Sse.LoadVector128(&ptrLifetime[i]));
                        // Progress Lifetime
                        var vec_lifetime = Sse.Add(Sse.LoadVector128(&ptrProgress[i]), vec_factor);
                        // Clamp to one
                        vec_lifetime = Sse.Min(vec_lifetime, vec128_one);
                        //
                        Sse.Store(&ptrProgress[i], vec_lifetime);
                    }
                }
            }
            else if (AdvSimd.Arm64.IsSupported) // 128
            {
                var vectorDelta = AdvSimd.DuplicateToVector128(delta);

                fixed (float* ptrLifetime = Lifetime, ptrProgress = LifeProgress)
                {
                    for (int i = startIndex; i < endIndex; i += 4)
                    {
                        var vec_factor = AdvSimd.Arm64.Divide(vectorDelta, AdvSimd.LoadVector128(&ptrLifetime[i]));
                        var vec_lifetime = AdvSimd.Add(AdvSimd.LoadVector128(&ptrProgress[i]), vec_factor);
                        vec_lifetime = AdvSimd.Min(vec_lifetime, vec128_one);
                        AdvSimd.Store(&ptrProgress[i], vec_lifetime);
                    }
                }
            }
            else if (AdvSimd.IsSupported) // 64
            {
                var vectorDelta = AdvSimd.DuplicateToVector64(delta);

                fixed (float* ptrLifetime = Lifetime, ptrProgress = LifeProgress)
                {
                    for (int i = startIndex; i < endIndex; i += 2)
                    {
                        var vec_factor = AdvSimd.DivideScalar(vectorDelta, AdvSimd.LoadVector64(&ptrLifetime[i]));
                        var vec_lifetime = AdvSimd.Add(AdvSimd.LoadVector64(&ptrProgress[i]), vec_factor);
                        vec_lifetime = AdvSimd.Min(vec_lifetime, vec64_one);
                        AdvSimd.Store(&ptrProgress[i], vec_lifetime);
                    }
                }
            }
            else // 32
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    LifeProgress[i] += delta / Lifetime[i];
                }
            }
        }
    }
}