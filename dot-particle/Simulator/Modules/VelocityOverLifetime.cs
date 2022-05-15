using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Core.Particles
{
    public class ModuleVelocityOverLifetime : IModule, IModuleSimulator
    {
        public float[] deltaCurve;
        ModuleVelocity velocity;
        ModuleLifetime lifetime;

        public ModuleVelocityOverLifetime(float[] deltaCurve)
        {
            this.deltaCurve = deltaCurve;
        }

        public void Initialize(SimulatorCPU sim)
        {
            lifetime = sim.GetModule<ModuleLifetime>();
            System.Diagnostics.Debug.Assert(lifetime != null);

            velocity = sim.GetModule<ModuleVelocity>();
            System.Diagnostics.Debug.Assert(velocity != null);
        }

        public unsafe void Update(float delta, int startIndex, int endIndex)
        {
            if (Avx2.IsSupported)
            {
                float len = deltaCurve.Length;
                fixed (float* ptr = deltaCurve, ptrLifetime = lifetime.LifeProgress, 
                    ptrVelocityX = velocity.VelocityX, ptrVelocityY = velocity.VelocityY,ptrVelocityZ = velocity.VelocityZ)
                {
                    // Broadcast the length of the deltacurve
                    var vec_len = Avx2.BroadcastScalarToVector256(&len);

                    for (int i = startIndex; i < endIndex; i += 8)
                    {
                        // Get lifeprogress
                        var vec_lifetime = Avx2.LoadVector256(&ptrLifetime[i]);
                        // Multiply Lifeprogress with Length and convert to index (int32)
                        var vec_index = Avx2.ConvertToVector256Int32(Avx2.Multiply(vec_len, vec_lifetime));
                        // Evaluate curve with index
                        var vec_m = Avx2.GatherVector256(ptr, vec_index, 1);

                        // Multiply Curve with  X
                        var vec_sx = Avx2.LoadVector256(&ptrVelocityX[i]);
                        Avx2.Store(&ptrVelocityX[i], Avx2.Multiply(vec_sx, vec_m));
                        // Multiply Curve with  Y
                        var vec_sy = Avx2.LoadVector256(&ptrVelocityY[i]);
                        Avx2.Store(&ptrVelocityY[i], Avx2.Multiply(vec_sy, vec_m));
                        // Multiply Curve with  z
                        var vec_sz = Avx2.LoadVector256(&ptrVelocityZ[i]);
                        Avx2.Store(&ptrVelocityZ[i], Avx2.Multiply(vec_sz, vec_m));
                    }
                }
            }
            else
                throw new System.NotImplementedException();
        }
    }
}
