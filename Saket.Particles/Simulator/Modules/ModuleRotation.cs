namespace Saket.Particles
{
    public class ModuleRotation : IModule
    {
        /// <summary> Screen Space rotation in radians</summary>
        public float[] Rotation { get; set; }
       
        public void Initialize(SimulatorCPU sim)
        {
            Rotation = new float[sim.Count];
        }
    }
}
