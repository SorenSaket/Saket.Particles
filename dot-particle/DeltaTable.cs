using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Runtime.Intrinsics ;
namespace Core.Particles
{
    // TODO: SSE doesn't have an avx.gather equivalent
    // 

    public unsafe class DeltaTable
    {
        public readonly float[] values;
        
        private Vector256<float> vec256_len;
        private Vector256<int> vec256_leni;

        private Vector128<float> vec128_len;
        private Vector128<int> vec128_leni;

        public DeltaTable(float[] values)
        {
            this.values = values;
            for (int i = 0; i < values.Length; i++)
            {
                values[i] += 1;
            }


            int leni = values.Length - 1;
            float lens = values.Length - 1;
            
            {
                if (Avx2.IsSupported)
                {
                    vec256_len = Avx2.BroadcastScalarToVector256(&lens);
                    vec256_leni = Avx2.BroadcastScalarToVector256(&leni);
                }
                else if (Sse2.IsSupported)
                {
                    vec128_len = Sse2.LoadScalarVector128(&lens);
                    vec128_leni = Sse2.LoadScalarVector128(&leni);
                }
            }
          
        }

        public unsafe Vector256<float> Sample(Vector256<float> vec_floatingIndex)
        {
            if (Avx2.IsSupported)
            {
                fixed (float* ptr = values)
                {
                    // Multiply Lifeprogress with Length and convert to index (int32)
                    var vec_index = Avx2.ConvertToVector256Int32(Avx2.Multiply(vec256_len, vec_floatingIndex));
                    // Enure that index remains within bounds.. TODO maybe clamp lifetime
                    // Else we will be gathering unvalid memory with Gather
                    // Maybe doesn't matter
                    vec_index = Avx2.Min(vec256_leni, vec_index);
                    // Evaluate curve with index
                    return Avx2.GatherVector256(ptr, vec_index, 4);
                }
            }else
                throw new System.NotImplementedException();
        }

        public unsafe Vector128<float> Sample(Vector128<float> vec_floatingIndex)
        {
            if (Sse41.IsSupported)
            {
                var vec_index = Sse41.ConvertToVector128Int32(Sse41.Multiply(vec128_len, vec_floatingIndex));
                vec_index = Sse41.Min(vec128_leni, vec_index);
                return Vector128.Create(values[vec_index.GetElement(0)], values[vec_index.GetElement(1)], values[vec_index.GetElement(2)], values[vec_index.GetElement(3)]);
            }
            throw new System.NotImplementedException();
        }

    }

    public unsafe class DeltaTableUint
    {
        public readonly uint[] values;

        private Vector256<float> vec256_len;
        private Vector256<int> vec256_leni;

        private Vector128<float> vec128_len;
        private Vector128<int> vec128_leni;

        public DeltaTableUint(uint[] values)
        {
            this.values = values;
            for (int i = 0; i < values.Length; i++)
            {
                values[i] += 1;
            }


            int leni = values.Length - 1;
            float lens = values.Length - 1;

            {
                if (Avx2.IsSupported)
                {
                    vec256_len = Avx2.BroadcastScalarToVector256(&lens);
                    vec256_leni = Avx2.BroadcastScalarToVector256(&leni);
                }
                else if (Sse2.IsSupported)
                {
                    vec128_len = Sse2.LoadScalarVector128(&lens);
                    vec128_leni = Sse2.LoadScalarVector128(&leni);
                }
            }

        }

        public unsafe Vector256<uint> Sample(Vector256<float> vec_floatingIndex)
        {
            if (Avx2.IsSupported)
            {
                fixed (uint* ptr = values)
                {
                    // Multiply Lifeprogress with Length and convert to index (int32)
                    var vec_index = Avx2.ConvertToVector256Int32(Avx2.Multiply(vec256_len, vec_floatingIndex));
                    // Enure that index remains within bounds.. TODO maybe clamp lifetime
                    // Else we will be gathering unvalid memory with Gather
                    // Maybe doesn't matter
                    vec_index = Avx2.Min(vec256_leni, vec_index);
                    // Evaluate curve with index
                    return Avx2.GatherVector256(ptr, vec_index, 4);
                }
            }
            else
                throw new System.NotImplementedException();
        }

        public unsafe Vector128<uint> Sample(Vector128<float> vec_floatingIndex)
        {
            if (Sse41.IsSupported)
            {
                var vec_index = Sse41.ConvertToVector128Int32(Sse41.Multiply(vec128_len, vec_floatingIndex));
                vec_index = Sse41.Min(vec128_leni, vec_index);
                return Vector128.Create(values[vec_index.GetElement(0)], values[vec_index.GetElement(1)], values[vec_index.GetElement(2)], values[vec_index.GetElement(3)]);
            }
            throw new System.NotImplementedException();
        }

    }
}
