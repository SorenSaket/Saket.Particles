using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace Saket.Particles
{
    // TODO: SSE doesn't have an avx.gather equivalent
    // 
    /// <summary>
    /// An array of values able to be sampled with SIMD
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Table<T> where T : unmanaged
    {
        public readonly T[] values;

        protected Vector256<float> vec256_len;
        protected Vector256<int> vec256_leni;

        protected Vector128<float> vec128_len;
        protected Vector128<int> vec128_leni;

        public Table(T[] values)
        {
            this.values = values;

            int leni = values.Length - 1;
            float lens = values.Length - 1;
            unsafe
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

        public abstract Vector256<T> Sample(Vector256<float> vec_floatingIndex);
        public abstract Vector128<T> Sample(Vector128<float> vec_floatingIndex);
        public abstract T Sample(float floatingIndex);
    }

    /// <inheritdoc cref="Table{T}"/>
    public class TableFloat : Table<float>
    {
        public TableFloat(float[] values) : base(values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i] += 1;
            }
        }

        public override unsafe Vector256<float> Sample(Vector256<float> vec_floatingIndex)
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
            }
            throw new System.NotImplementedException();
        }
        public override unsafe Vector128<float> Sample(Vector128<float> vec_floatingIndex)
        {
            if (Sse41.IsSupported)
            {
                var vec_index = Sse41.ConvertToVector128Int32(Sse41.Multiply(vec128_len, vec_floatingIndex));
                vec_index = Sse41.Min(vec128_leni, vec_index);
                return Vector128.Create(values[vec_index.GetElement(0)], values[vec_index.GetElement(1)], values[vec_index.GetElement(2)], values[vec_index.GetElement(3)]);
            }
            throw new System.NotImplementedException();
        }

        public override float Sample(float floatingIndex) 
        {
            throw new System.NotImplementedException();
        }

    }

    /// <inheritdoc cref="Table{T}"/>
    public class TableUint : Table<uint>
    {
        public TableUint(uint[] values) : base(values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i] += 1;
            }
        }

        public override unsafe Vector256<uint> Sample(Vector256<float> vec_floatingIndex)
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

        public override unsafe Vector128<uint> Sample(Vector128<float> vec_floatingIndex)
        {
            if (Sse41.IsSupported)
            {
                var vec_index = Sse41.ConvertToVector128Int32(Sse41.Multiply(vec128_len, vec_floatingIndex));
                vec_index = Sse41.Min(vec128_leni, vec_index);
                return Vector128.Create(values[vec_index.GetElement(0)], values[vec_index.GetElement(1)], values[vec_index.GetElement(2)], values[vec_index.GetElement(3)]);
            }
            throw new System.NotImplementedException();
        }
        public override uint Sample(float floatingIndex)
        {
            throw new System.NotImplementedException();
        }
    }
}
