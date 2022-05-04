

if (Avx.IsSupported)
    {
        AvxIntrinsics.AddScalarU(scalar, dst);
    }
    else if (Sse.IsSupported)
    {
        SseIntrinsics.AddScalarU(scalar, dst);
    }
"If AVX is supported, it is preferred, otherwise SSE is used if available, otherwise the software fallback path.
At runtime, the JIT will actually generate code for only one of these three blocks, as appropriate for the platform it finds itself on."
https://devblogs.microsoft.com/dotnet/using-net-hardware-intrinsics-api-to-accelerate-machine-learning-scenarios/