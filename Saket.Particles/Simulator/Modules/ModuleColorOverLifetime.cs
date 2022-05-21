
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics.Arm;

namespace Saket.Particles
{
    public class ModuleColorOverLifetime : IModule, IModuleSimulator
    {
        public TableUint deltaCurve;
        ModuleColor color;
        ModuleLifetime lifetime;

        public ModuleColorOverLifetime(TableUint deltaCurve)
        {
            this.deltaCurve = deltaCurve;
        }

        public void Initialize(SimulatorCPU sim)
        {
            lifetime = sim.GetModule<ModuleLifetime>();
            System.Diagnostics.Debug.Assert(lifetime != null);

            color = sim.GetModule<ModuleColor>();
            System.Diagnostics.Debug.Assert(color != null);
        }

        public unsafe void Update(float delta, int startIndex, int endIndex)
        {
            if (Avx.IsSupported)
            {
                fixed (float* ptrLifetime = lifetime.LifeProgress)
                {
                    fixed (uint* ptrColor = color.Color)
                    {
                        for (int i = startIndex; i < endIndex; i += 8)
                        {
                            // Get lifeprogress
                            var vec_lifetime = Avx.LoadVector256(&ptrLifetime[i]);
                            var vec_color = Avx.LoadVector256(&ptrColor[i]);

                            // Sample Curve
                            var vec_target = deltaCurve.Sample(vec_lifetime);

                            Avx.Store(&ptrColor[i], vec_target);
                        }
                    }
                }
            }
            else if (Sse2.IsSupported)
            {
                fixed (float* ptrLifetime = lifetime.LifeProgress)
                {
                    fixed (uint* ptrColor = color.Color)
                    {
                        for (int i = startIndex; i < endIndex; i += 4)
                        {
                            // Get lifeprogress
                            var vec_lifetime = Sse2.LoadVector128(&ptrLifetime[i]);
                            var vec_color = Sse2.LoadVector128(&ptrColor[i]);

                            // Sample Curve
                            var vec_target = deltaCurve.Sample(vec_lifetime);

                            Sse2.Store(&ptrColor[i], vec_target);
                        }
                    }
                }
            }
            else if (AdvSimd.IsSupported)
            {
                fixed (float* ptrLifetime = lifetime.LifeProgress)
                {
                    fixed (uint* ptrColor = color.Color)
                    {
                        for (int i = startIndex; i < endIndex; i += 4)
                        {
                            // Get lifeprogress
                            var vec_lifetime = AdvSimd.LoadVector128(&ptrLifetime[i]);
                            var vec_color = AdvSimd.LoadVector128(&ptrColor[i]);

                            // Sample Curve
                            var vec_target = deltaCurve.Sample(vec_lifetime);

                            AdvSimd.Store(&ptrColor[i], vec_target);
                        }
                    }
                }
            }
            else
            {
                for (int i = startIndex; i < endIndex; i += 1)
                {
                    color.Color[i] = deltaCurve.Sample(lifetime.LifeProgress[i]);
                }
            }
        }
    }
}
