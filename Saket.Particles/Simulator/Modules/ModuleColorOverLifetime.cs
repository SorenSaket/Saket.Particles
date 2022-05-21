using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

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
            if (Avx2.IsSupported)
            {
                fixed (float* ptrLifetime = lifetime.LifeProgress)
                {
                    fixed (uint* ptrColor = color.Color)
                    {
                        for (int i = startIndex; i < endIndex; i += 8)
                        {
                            // Get lifeprogress
                            var vec_lifetime = Avx2.LoadVector256(&ptrLifetime[i]);
                            var vec_color = Avx2.LoadVector256(&ptrColor[i]);

                            // Sample Curve
                            var vec_target = deltaCurve.Sample(vec_lifetime);

                            Avx2.Store(&ptrColor[i], vec_target);

                        }
                    }
                }
            }
            else
                throw new System.NotImplementedException();
        }
    }
}
