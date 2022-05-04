using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Core.Particles
{
	internal static unsafe class CPUMath
	{
		public static void Add(float[] a, float[] b, int start, int end)
		{
			if (Avx.IsSupported)
			{
				AvxAdd(a, b, start, end);
			}
			else if (Sse.IsSupported)
			{
				SseAdd(a, b, start, end);
			}
			else
			{
				throw new System.NotImplementedException();
			}
		}


		private static unsafe void AvxAdd(float[] a, float[] b, int start, int end)
		{
			fixed (float* aPtr = &a[0], bPtr = &b[0])
			{
				for (int i = start; i < end; i += 8)
				{
					var av = Avx.LoadVector256(&aPtr[i]);
					var ab = Avx.LoadVector256(&bPtr[i]);
					Avx.Store(&aPtr[i], Avx.Add(av, ab));
				}
			}
		}

		private static unsafe void SseAdd(float[] a, float[] b, int start, int end)
		{
			fixed (float* aPtr = &a[0], bPtr = &b[0])
			{
				for (int i = start; i < end; i += 4)
				{
					var av = Sse.LoadVector128(&aPtr[i]);
					var ab = Sse.LoadVector128(&bPtr[i]);
					Sse.Store(&aPtr[i], Sse.Add(av, ab));
				}
			}
		}


		private static unsafe void AvxSub(float[] a, float b, int start, int end)
		{
			fixed (float* aPtr = &a[0])
			{
				for (int i = start; i < end; i += 8)
				{
					var av = Avx.LoadVector256(&aPtr[i]);
					var ab = Avx.BroadcastScalarToVector256(&b);
					Avx.Store(&aPtr[i], Avx.Subtract(av, ab));
				}
			}
		}

	}
}
