using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Core.Particles
{
    public class ModuleScaleOverLifetime : IModule, IModuleSimulator
    {
        public DeltaTable deltaCurve;
        ModuleScale scale;
        ModuleLifetime lifetime;

        public ModuleScaleOverLifetime(DeltaTable deltaCurve)
        {
            this.deltaCurve = deltaCurve;
        }

        public void Initialize(SimulatorCPU sim)
        {
            lifetime = sim.GetModule<ModuleLifetime>();
            System.Diagnostics.Debug.Assert(lifetime != null);

            scale = sim.GetModule<ModuleScale>();
            System.Diagnostics.Debug.Assert(scale != null);
        }

        public unsafe void Update(float delta, int startIndex, int endIndex)
        {
            if (Avx2.IsSupported)
            {

                fixed (float* ptrLifetime = lifetime.LifeProgress, ptrScaleX = scale.ScaleX, ptrScaleY = scale.ScaleY)
                {
                    for (int i = startIndex; i < endIndex; i+=8)
                    {
                        // Get lifeprogress
                        var vec_lifetime = Avx2.LoadVector256(&ptrLifetime[i]);

                        // Sample Curve
                        var vec_m = deltaCurve.Sample(vec_lifetime);

                        // Multiply Curve with scale X
                        var vec_sx = Avx2.LoadVector256(&ptrScaleX[i]);
                        Avx2.Store(&ptrScaleX[i], Avx2.Multiply(vec_sx, vec_m));
                        // Multiply Curve with scale Y
                        var vec_sy = Avx2.LoadVector256(&ptrScaleY[i]);
                        Avx2.Store(&ptrScaleY[i], Avx2.Multiply(vec_sy, vec_m));
                    }
                }
            }
            else
                throw new System.NotImplementedException();
        }
    }
}
