using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Particles.Rendering
{
    public abstract class Renderer
    {

        private ParticleSystem system;

		protected Renderer(ParticleSystem system)
		{
			this.system = system;
		}
	}
}
