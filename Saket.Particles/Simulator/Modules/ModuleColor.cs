namespace Saket.Particles
{
    public class ModuleColor : IModule
    {
        /// <summary> Color </summary>
        /// R, G, B, A
        public uint[] Color { get; set; }

        public void Initialize(SimulatorCPU sim)
        {
            Color = new uint[sim.Count];
        }
    }
}