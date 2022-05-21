using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Saket.Particles
{
    public class ModuleVelocityOverLifetime : IModule, IModuleSimulator
    {
        public TableFloat deltaCurve;
        ModuleVelocity velocity;
        ModuleLifetime lifetime;

        public ModuleVelocityOverLifetime(TableFloat deltaCurve)
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
                fixed (float*  ptrLifetime = lifetime.LifeProgress, 
                    ptrVelocityX = velocity.VelocityX, ptrVelocityY = velocity.VelocityY,ptrVelocityZ = velocity.VelocityZ)
                {
                    for (int i = startIndex; i < endIndex; i += 8)
                    {
                        // Get lifeprogress
                        var vec_lifetime = Avx2.LoadVector256(&ptrLifetime[i]);
                        
                        // Sample Curve
                        var vec_m = deltaCurve.Sample(vec_lifetime);

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
