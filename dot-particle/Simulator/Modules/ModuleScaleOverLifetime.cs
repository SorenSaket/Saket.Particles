using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Core.Particles
{
    public class ModuleScaleOverLifetime : IModule, IModuleSimulator
    {
        public float[] deltaCurve;
        ModuleScale scale;
        ModuleLifetime lifetime;

        public ModuleScaleOverLifetime(float[] deltaCurve)
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
                float len = deltaCurve.Length;
                fixed (float* ptr = deltaCurve, ptrLifetime = lifetime.LifeProgress, ptrScaleX = scale.ScaleX, ptrScaleY = scale.ScaleY)
                {
                    // Broadcast the length of the deltacurve
                    var vec_len = Avx2.BroadcastScalarToVector256(&len);

                    for (int i = startIndex; i < endIndex; i+=8)
                    {
                        // Get lifeprogress
                        var vec_lifetime = Avx2.LoadVector256(&ptrLifetime[i]);
                        // Multiply Lifeprogress with Length and convert to index (int32)
                        var vec_index = Avx2.ConvertToVector256Int32(Avx2.Multiply(vec_len, vec_lifetime));
                        // Evaluate curve with index
                        var vec_m = Avx2.GatherVector256(ptr, vec_index, 1);
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
