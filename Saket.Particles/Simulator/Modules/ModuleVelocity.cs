using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics.Arm;


namespace Saket.Particles
{
    public class ModuleVelocity : IModule, IModuleSimulator
    {
        // --------- Positional State ---------
        /// <summary> Velocity X </summary>
        public float[] VelocityX { get; set; }
        /// <summary> Velocity Y </summary>
        public float[] VelocityY { get; set; }
        /// <summary> Velocity Z </summary>
        public float[] VelocityZ { get; set; }


        private ModulePosition position;


        public void Initialize(SimulatorCPU sim)
        {
            position = sim.GetModule<ModulePosition>();
            System.Diagnostics.Debug.Assert(position != null);

            VelocityX = new float[sim.Count];
            VelocityY = new float[sim.Count];
            VelocityZ = new float[sim.Count];
        }

        public unsafe void Update(float delta, int startIndex, int endIndex)
        {
            CPUMath.Add(position.PositionX, VelocityX, startIndex, endIndex);
            CPUMath.Add(position.PositionY, VelocityY, startIndex, endIndex);
            CPUMath.Add(position.PositionZ, VelocityZ, startIndex, endIndex);
        }
    }
}
