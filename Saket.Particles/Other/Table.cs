using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace Saket.Particles
{
    // Other instruction sets doesn't have Avx2.gather equivalent :thinkingemoji:

    /// <summary>
    /// An array of values able to be sampled with SIMD
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Table<T> where T : unmanaged
    {
        public readonly T[] values;

        // Length of the 

        protected Vector256<float> vec256_len;
        protected Vector128<float> vec128_len;
        protected Vector64<float> vec64_len;


        public Table(T[] values)
        {
            this.values = values;

            int leni = values.Length - 1;
            float lens = values.Length - 1;


            vec256_len = Vector256.Create(lens);

            vec128_len = Vector128.Create(lens);

            vec64_len = Vector64.Create(lens);
        }

        public abstract Vector256<T> Sample(Vector256<float> vec_floatingIndex);
        public abstract Vector128<T> Sample(Vector128<float> vec_floatingIndex);
        public T Sample(float floatingIndex)
        {
            return values[(int)(floatingIndex * (values.Length - 1))];
        }
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
                    
                    // Evaluate curve with index
                    return Avx2.GatherVector256(ptr, vec_index, 4);
                }
            }
            else if (Avx.IsSupported)
            {
                fixed (float* ptr = values)
                {
                    // Multiply Lifeprogress with Length and convert to index (int32)
                    var vec_index = Avx.ConvertToVector256Int32(Avx.Multiply(vec256_len, vec_floatingIndex));

                    // Evaluate curve with index
                    return Vector256.Create(
                        values[vec_index.GetElement(0)], values[vec_index.GetElement(1)], values[vec_index.GetElement(2)], values[vec_index.GetElement(3)],
                        values[vec_index.GetElement(4)], values[vec_index.GetElement(5)], values[vec_index.GetElement(6)], values[vec_index.GetElement(7)]
                        );
                }
            }
            throw new System.NotImplementedException();
        }
        public override unsafe Vector128<float> Sample(Vector128<float> vec_floatingIndex)
        {
            if (Sse2.IsSupported)
            {
                var vec_index = Sse2.ConvertToVector128Int32(Sse2.Multiply(vec128_len, vec_floatingIndex));
                return Vector128.Create(values[vec_index.GetElement(0)], values[vec_index.GetElement(1)], values[vec_index.GetElement(2)], values[vec_index.GetElement(3)]);
            }
            else if (AdvSimd.IsSupported)
            {
                var vec_index = AdvSimd.ConvertToInt32RoundToZero(AdvSimd.Multiply(vec128_len, vec_floatingIndex));
                return Vector128.Create(values[vec_index.GetElement(0)], values[vec_index.GetElement(1)], values[vec_index.GetElement(2)], values[vec_index.GetElement(3)]);
            }
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
                    var vec_index = Avx2.ConvertToVector256Int32(Avx2.Multiply(vec256_len, vec_floatingIndex));
                    return Avx2.GatherVector256(ptr, vec_index, 4);
                }
            }
            else if (Avx.IsSupported)
            {
                fixed (uint* ptr = values)
                {
                    var vec_index = Avx.ConvertToVector256Int32(Avx.Multiply(vec256_len, vec_floatingIndex));
                    return Vector256.Create(
                         values[vec_index.GetElement(0)], values[vec_index.GetElement(1)], values[vec_index.GetElement(2)], values[vec_index.GetElement(3)],
                         values[vec_index.GetElement(4)], values[vec_index.GetElement(5)], values[vec_index.GetElement(6)], values[vec_index.GetElement(7)]
                         );
                }
            }
            throw new System.NotImplementedException();
        }

        public override unsafe Vector128<uint> Sample(Vector128<float> vec_floatingIndex)
        {
            if (Sse2.IsSupported)
            {
                var vec_index = Sse2.ConvertToVector128Int32(Sse2.Multiply(vec128_len, vec_floatingIndex));
                return Vector128.Create(values[vec_index.GetElement(0)], values[vec_index.GetElement(1)], values[vec_index.GetElement(2)], values[vec_index.GetElement(3)]);
            }
            else if (AdvSimd.IsSupported)
            {
                var vec_index = AdvSimd.ConvertToInt32RoundToZero(AdvSimd.Multiply(vec128_len, vec_floatingIndex));
                return Vector128.Create(values[vec_index.GetElement(0)], values[vec_index.GetElement(1)], values[vec_index.GetElement(2)], values[vec_index.GetElement(3)]);
            }
            throw new System.NotImplementedException();
        }
    }
}
