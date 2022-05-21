using System;
using System.Collections.Generic;
using System.Text;

namespace Saket.Particles
{
    public interface IModule
    {
        public void Initialize(SimulatorCPU sim);
    }

}
