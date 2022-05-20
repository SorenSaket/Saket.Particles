using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;
using System.Text;

namespace Core.Particles
{
    public class ModuleSheetAnimatorRandom : IModule, IModuleSimulator
    {
        public delegate void TrailEmitterAction(int index);

        private readonly SimulatorCPU baseSimulator;

        public float[] Timer { get; set; }

        private ModulePosition position;
        private EmitterAction action;
        private float timerBetweenSpawns;

        private Vector256<float> di;
        private Vector256<float> zero;

        private ModuleSheet sheet;

        public ModuleSheetAnimatorRandom(EmitterAction action, float timeBetweenSpawns)
        {
            this.action = action;
            this.timerBetweenSpawns = timeBetweenSpawns;
            unsafe
            {
                di = Avx.BroadcastScalarToVector256(&timeBetweenSpawns);
                zero = new Vector256<float>();
            }
        }

        public void Initialize(SimulatorCPU sim)
        {
            Timer = new float[sim.Count];
            sheet = sim.GetModule<ModuleSheet>();
        }
        public unsafe void Update(float delta, int startIndex, int endIndex)
        {
            if (Avx.IsSupported)
            {
                var vectorDelta = Avx.BroadcastScalarToVector256(&delta);

                fixed (float* ptrTimer = Timer)
                {
                    for (int i = startIndex; i < endIndex; i += 8)
                    {
                        // Advance Timer
                        var vec_timer = Avx.LoadVector256(&ptrTimer[i]);
                        vec_timer = Avx.Add(vec_timer, vectorDelta);
                        Avx.Store(&ptrTimer[i], vec_timer);

                        // Reset timer
                        var mask = Avx.Compare(vec_timer, di, FloatComparisonMode.OrderedGreaterThanOrEqualSignaling);
                        Avx.MaskStore(&ptrTimer[i], mask, zero);
                    }
                }
            }
        }
    }
}
