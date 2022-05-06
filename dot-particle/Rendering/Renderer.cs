using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Particles.Rendering
{
    public abstract class Renderer
    {

        private SimulatorCPU system;

		protected Renderer(SimulatorCPU system)
		{
			this.system = system;
		}
	}
}
