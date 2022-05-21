using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;
using System.Text;

namespace Saket.Particles
{
    public class ModuleEmitterTrail : IModule, IModuleSimulator
    {
        public delegate void TrailEmitterAction(int index);

        private readonly SimulatorCPU baseSimulator;

        public float[] LastPositionX { get; set; }
        public float[] LastPositionY { get; set; }
        public float[] LastPositionZ { get; set; }

        public float[] Distances { get; set; }

        private ModulePosition position;
        private EmitterAction action;
        private float distancePerSpawn;

        private Vector256<float> di;
        private Vector256<float> zero;
        public ModuleEmitterTrail(SimulatorCPU sim, EmitterAction action, float distancePerSpawn)
        {
            this.action = action;
            this.distancePerSpawn = distancePerSpawn;
            unsafe
            {
                di = Avx.BroadcastScalarToVector256(&distancePerSpawn);
                zero = new Vector256<float>();
            }
        }
        public void Initialize(SimulatorCPU sim)
        {
            position = sim.GetModule<ModulePosition>();
            System.Diagnostics.Debug.Assert(position != null);

            Distances = new float[sim.Count];
            LastPositionX = new float[sim.Count];
            LastPositionY = new float[sim.Count];
            LastPositionZ = new float[sim.Count];
        }
        public unsafe void Update(float delta, int startIndex, int endIndex)
        {
            if (Avx.IsSupported)
            {
                fixed (float* 
                    ptrPositionX = position.PositionX, 
                    ptrPositionY = position.PositionY, 
                    ptrPositionZ = position.PositionZ,

                    ptrLastPositionX = LastPositionX,
                    ptrLastPositionY = LastPositionY,
                    ptrLastPositionZ = LastPositionZ,

                    ptrDistances = Distances)
                {
                    for (int i = startIndex; i < endIndex; i += 8)
                    {
                        var vec_posX = Avx.LoadVector256(&ptrPositionX[i]);
                        var vec_posY = Avx.LoadVector256(&ptrPositionY[i]);
                        var vec_posZ = Avx.LoadVector256(&ptrPositionZ[i]);

                        var vec_lastX = Avx.LoadVector256(&ptrLastPositionX[i]);
                        var vec_lastY = Avx.LoadVector256(&ptrLastPositionY[i]);
                        var vec_lastZ = Avx.LoadVector256(&ptrLastPositionZ[i]);

                        var vec_dist = Avx.LoadVector256(&ptrDistances[i]);

                        vec_dist = Avx.Add(vec_dist, Avx.Subtract(vec_lastX, vec_posX));
                        vec_dist = Avx.Add(vec_dist, Avx.Subtract(vec_lastY, vec_posY));
                        vec_dist = Avx.Add(vec_dist, Avx.Subtract(vec_lastZ, vec_posZ));

                        for (int q = 0; q < 4; q++)
                        {
                            if(vec_dist.GetElement(q) > distancePerSpawn)
                            {
                                action.Invoke(i);
                            }
                        }

                        var mask = Avx.Compare(vec_dist, di, FloatComparisonMode.OrderedGreaterThanOrEqualSignaling);

                        //Avx.MaskStore(vec_dist, mask, zero);



                        Avx2.Store(&ptrDistances[i], vec_dist);


                    }
                }
            }
        }
    }
}
