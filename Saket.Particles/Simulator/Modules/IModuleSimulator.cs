using System;
using System.Collections.Generic;
using System.Text;

namespace Saket.Particles
{
    public interface IModuleSimulator
    {
        public void Update(float delta, int startIndex, int endIndex);
    }
}