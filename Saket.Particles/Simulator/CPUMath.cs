using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics.Arm;

namespace Saket.Particles
{
	public unsafe static class CPUMath
	{
		
		public static void Add(float[] a, float[] b, int startIndex, int endIndex)
		{
            if (Avx.IsSupported)
            {
                fixed (float* ptrA = a, ptrB = b)
                {
                    for (int i = startIndex; i < endIndex; i += 8)
                    {
                        Avx.Store(&ptrA[i], Avx.Add(Avx.LoadVector256(&ptrA[i]), Avx.LoadVector256(&ptrB[i])));
                    }
                }
            }
            else if (Sse.IsSupported)
            {
                fixed (float* ptrA = a, ptrB = b)
                {
                    for (int i = startIndex; i < endIndex; i += 4)
                    {
                        Sse.Store(&ptrA[i], Sse.Add(Sse.LoadVector128(&ptrA[i]), Sse.LoadVector128(&ptrB[i])));
                    }
                }
            }
            else if (AdvSimd.IsSupported)
            {
                fixed (float* ptrA = a, ptrB = b)
                {
                    for (int i = startIndex; i < endIndex; i += 4)
                    {
                        AdvSimd.Store(&ptrA[i], AdvSimd.Add(AdvSimd.LoadVector128(&ptrA[i]), AdvSimd.LoadVector128(&ptrB[i])));
                    }
                }
            }
            else
            {
                for (int i = startIndex; i < endIndex; i += 1)
                {
                    a[i] += b[i];
                }
            }
        }




	}
}
