using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics.Arm;

namespace Saket.Particles
{
    public class ModuleScaleOverLifetime : IModule, IModuleSimulator
    {
        public TableFloat deltaCurve;
        ModuleScale scale;
        ModuleLifetime lifetime;

        public ModuleScaleOverLifetime(TableFloat deltaCurve)
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
            if (Avx.IsSupported)
            {
                fixed (float* ptrLifetime = lifetime.LifeProgress, ptrScaleX = scale.ScaleX, ptrScaleY = scale.ScaleY)
                {
                    for (int i = startIndex; i < endIndex; i+=8)
                    {
                        // Get lifeprogress
                        var vec_lifetime = Avx.LoadVector256(&ptrLifetime[i]);

                        // Sample Curve
                        var vec_m = deltaCurve.Sample(vec_lifetime);

                        Avx.Store(&ptrScaleX[i], vec_m);
                        Avx.Store(&ptrScaleY[i], vec_m);
                    }
                }
            }
            else if (Sse.IsSupported)
            {
                fixed (float* ptrLifetime = lifetime.LifeProgress, ptrScaleX = scale.ScaleX, ptrScaleY = scale.ScaleY)
                {
                    for (int i = startIndex; i < endIndex; i += 4)
                    {
                        var vec_lifetime = Sse.LoadVector128(&ptrLifetime[i]);
                        var vec_m = deltaCurve.Sample(vec_lifetime);
                        Sse.Store(&ptrScaleX[i], vec_m);
                        Sse.Store(&ptrScaleY[i], vec_m);
                    }
                }
            }
            else if (AdvSimd.IsSupported)
            {
                fixed (float* ptrLifetime = lifetime.LifeProgress, ptrScaleX = scale.ScaleX, ptrScaleY = scale.ScaleY)
                {
                    for (int i = startIndex; i < endIndex; i += 4)
                    {
                        var vec_lifetime = AdvSimd.LoadVector128(&ptrLifetime[i]);
                        var vec_m = deltaCurve.Sample(vec_lifetime);
                        AdvSimd.Store(&ptrScaleX[i], vec_m);
                        AdvSimd.Store(&ptrScaleY[i], vec_m);
                    }
                }
            }
            else
            {
                for (int i = startIndex; i < endIndex; i += 1)
                {
                    float factor = deltaCurve.Sample(lifetime.LifeProgress[i]);
                    scale.ScaleX[i] = factor;
                    scale.ScaleY[i] = factor;
                }
            }
        }
    }
}
