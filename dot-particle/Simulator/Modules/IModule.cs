using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Particles
{
    public interface IModule
    {
        public void Initialize(SimulatorCPU sim);
    }

}
