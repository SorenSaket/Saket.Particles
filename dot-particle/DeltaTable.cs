using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Runtime.Intrinsics ;
namespace Core.Particles
{
    public unsafe class DeltaTable
    {
        public readonly float[] values;
        
        
        private Vector256<float> vec_len;
        private Vector256<int> vec_leni;

        public DeltaTable(float[] values)
        {
            this.values = values;
            for (int i = 0; i < values.Length; i++)
            {
                values[i] += 1;
            }


            int leni = values.Length - 1;
            float lens = values.Length - 1;

            if (!Avx2.IsSupported)
                throw new System.NotImplementedException();

            vec_len = Avx2.BroadcastScalarToVector256(&lens);
            vec_leni = Avx2.BroadcastScalarToVector256(&leni);
        }

        public unsafe Vector256<float> Sample(Vector256<float> vec_floatingIndex)
        {
            fixed (float* ptr = values)
            {
                // Multiply Lifeprogress with Length and convert to index (int32)
                var vec_index = Avx2.ConvertToVector256Int32(Avx2.Multiply(vec_len, vec_floatingIndex));
                // Enure that index remains within bounds.. TODO maybe clamp lifetime
                // Else we will be gathering unvalid memory with Gather
                // Maybe doesn't matter
                vec_index = Avx2.Min(vec_leni, vec_index);
                // Evaluate curve with index
                return Avx2.GatherVector256(ptr, vec_index, 4);
            }
        }
    }
}
