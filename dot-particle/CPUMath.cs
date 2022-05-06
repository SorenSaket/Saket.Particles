using System.Runtime.Intrinsics.X86;

namespace Core.Particles
{
	internal static class CPUMath
	{
		public static void Mult(float[] a, float b, int start, int end)
		{
			if (Avx.IsSupported)
			{
				AvxMult(a, b, start, end);
			}
			else
			{
				throw new System.NotImplementedException();
			}
		}
		private static unsafe void AvxMult(float[] a, float b, int start, int end)
		{
			var ab = Avx.BroadcastScalarToVector256(&b);

			fixed (float* aPtr = &a[0])
			{
				for (int i = start; i < end; i += 8)
				{
					var av = Avx.LoadVector256(&aPtr[i]);
					Avx.Store(&aPtr[i], Avx.Multiply(av, ab));
				}
			}
		}


		public static void DivStore(float[] store, float[] a, float[] b, int start, int end)
		{
			if (Avx.IsSupported)
			{
				AvxDivStore(store, a, b, start, end);
			}
			else
			{
				throw new System.NotImplementedException();
			}
		}
		private static unsafe void AvxDivStore(float[] store, float[] a, float[] b, int start, int end)
		{
			fixed (float* storePtr = &store[0], aPtr = &a[0], bPtr = &b[0])
			{
				for (int i = start; i < end; i += 8)
				{
					var av = Avx.LoadVector256(&aPtr[i]);
					var ab = Avx.LoadVector256(&bPtr[i]);
					Avx.Store(&storePtr[i], Avx.Divide(av, ab));
				}
			}
		}


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

		public static void Add(float[] a, float b, int start, int end)
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
		private static unsafe void AvxAdd(float[] a, float b, int start, int end)
		{
			var ab = Avx.BroadcastScalarToVector256(&b);
			
			fixed (float* aPtr = &a[0])
			{
				for (int i = start; i < end; i += 8)
				{
					var av = Avx.LoadVector256(&aPtr[i]);
					Avx.Store(&aPtr[i], Avx.Add(av, ab));
				}
			}
		}
		private static unsafe void SseAdd(float[] a, float b, int start, int end)
		{
			var ab = Sse.LoadScalarVector128(&b);

			fixed (float* aPtr = &a[0])
			{
				for (int i = start; i < end; i += 4)
				{
					var av = Sse.LoadVector128(&aPtr[i]);
					Sse.Store(&aPtr[i], Sse.Add(av, ab));
				}
			}
		}

		public static void Sub(float[] a, float b, int start, int end)
		{
			if (Avx.IsSupported)
			{
				AvxSub(a, b, start, end);
			}
			else
			{
				throw new System.NotImplementedException();
			}
		}
		private static unsafe void AvxSub(float[] a, float b, int start, int end)
		{
			var ab = Avx.BroadcastScalarToVector256(&b);

			fixed (float* aPtr = &a[0])
			{
				for (int i = start; i < end; i += 8)
				{
					var av = Avx.LoadVector256(&aPtr[i]);
					Avx.Store(&aPtr[i], Avx.Subtract(av, ab));
				}
			}
		}

	}
}
