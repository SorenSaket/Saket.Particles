namespace Saket.Particles
{
    public class ModuleRotationalVelocity : IModule,  IModuleSimulator
    {
        /// <summary> Rotational Velocity in Radias Per Second</summary>
        public float[] RotationalVelocity { get; set; }

        private ModuleRotation rotation;

        public void Initialize(SimulatorCPU sim)
        {
            rotation = sim.GetModule<ModuleRotation>();
            System.Diagnostics.Debug.Assert(rotation != null);

            RotationalVelocity = new float[sim.Count];
        }

        public unsafe void Update(float delta, int startIndex, int endIndex)
        {
            CPUMath.Add(rotation.Rotation, RotationalVelocity, startIndex, endIndex);
        }
    }
}
