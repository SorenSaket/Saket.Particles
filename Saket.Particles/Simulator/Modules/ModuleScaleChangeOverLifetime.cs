using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics.Arm;

namespace Saket.Particles
{
    public class ModuleScaleChangeOverLifetime : IModule, IModuleSimulator
    {
        public TableFloat deltaCurve;
        ModuleScale scale;
        ModuleLifetime lifetime;

        public ModuleScaleChangeOverLifetime(TableFloat deltaCurve)
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

                        // Multiply Curve with scale X
                        var vec_sx = Avx.LoadVector256(&ptrScaleX[i]);
                        Avx.Store(&ptrScaleX[i], Avx.Multiply(vec_sx, vec_m));
                        // Multiply Curve with scale Y
                        var vec_sy = Avx.LoadVector256(&ptrScaleY[i]);
                        Avx.Store(&ptrScaleY[i], Avx.Multiply(vec_sy, vec_m));
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
                        var vec_sx = Sse.LoadVector128(&ptrScaleX[i]);
                        Sse.Store(&ptrScaleX[i], Sse.Multiply(vec_sx, vec_m));
                        var vec_sy = Sse.LoadVector128(&ptrScaleY[i]);
                        Sse.Store(&ptrScaleY[i], Sse.Multiply(vec_sy, vec_m));
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
                        var vec_sx = AdvSimd.LoadVector128(&ptrScaleX[i]);
                        AdvSimd.Store(&ptrScaleX[i], AdvSimd.Multiply(vec_sx, vec_m));
                        var vec_sy = AdvSimd.LoadVector128(&ptrScaleY[i]);
                        AdvSimd.Store(&ptrScaleY[i], AdvSimd.Multiply(vec_sy, vec_m));
                    }
                }
            }
            else
            {
                for (int i = startIndex; i < endIndex; i += 1)
                {
                    float factor = deltaCurve.Sample(lifetime.LifeProgress[i]);
                    scale.ScaleX[i] *= factor;
                    scale.ScaleY[i] *= factor;
                }
            }
        }
    }
}
