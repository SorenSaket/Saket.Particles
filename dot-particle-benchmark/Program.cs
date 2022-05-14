using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;

namespace Core.Particles
{


    [StructLayout(LayoutKind.Sequential)]
    public struct SOALifetime
    {
        public float[] Lifetime;
        public float[] CurrentLifetime;
        public float[] LifeProgress;

        public SOALifetime(int count)
        {
            Lifetime = new float[count];
            CurrentLifetime = new float[count];
            LifeProgress = new float[count];
        }
    }

    [SimpleJob(RunStrategy.Monitoring, targetCount: 8)]
    public class SOAvAOSvAOSOA
    {
        int count = 100000000;
        private IntPtr data;

        public SOAvAOSvAOSOA()
        {
            Random random = new Random();

            int fc = count * 3;
            data = Marshal.AllocHGlobal(sizeof(float) * count * 3);
            unsafe
            {
                for (int i = 0; i < count; i++)
                {
                    ((float*)data)[i] = (float)random.NextDouble();
                }
            }

        }

        [Benchmark]
        public unsafe void AllocHGlobalSOA()
        {
            float* aPtr = ((float*)data);
            float* bPtr = ((float*)data+count);

            for (int i = 0; i < count; i += 8)
            {
                var av = Avx.LoadVector256(&aPtr[i]);
                var ab = Avx.LoadVector256(&bPtr[i]);
                Avx.Store(&aPtr[i], Avx.Add(av, ab));
                Avx.Store(&aPtr[i], Avx.Add(av, ab));
                Avx.Store(&aPtr[i], Avx.Subtract(av, ab));
                Avx.Store(&aPtr[i], Avx.Add(av, ab));
            }
        }
        [Benchmark]
        public unsafe void AllocHGlobalAOSOA()
        {
            float* aPtr = ((float*)data);

            for (int i = 0; i < count; i += 24)
            {
                var av = Avx.LoadVector256(&aPtr[i]);
                var ab = Avx.LoadVector256(&aPtr[i+8]);
                Avx.Store(&aPtr[i], Avx.Add(av, ab));
                Avx.Store(&aPtr[i], Avx.Add(av, ab));
                Avx.Store(&aPtr[i], Avx.Subtract(av, ab));
                Avx.Store(&aPtr[i], Avx.Add(av, ab));
            }
        }



    }


    [SimpleJob(RunStrategy.Monitoring, targetCount: 100)]
    public class Ma
    {
        private const int count = 1000000;
        const int hybridCount = count / 8;
        const int unsafeCount = count * 3;

        private float[] Lifetime;
        private float[] CurrentLifetime;
        private float[] LifeProgress;

        private SOALifetime[] hybrid;

        private IntPtr hybridData;


        public Ma()
        {
            Lifetime = new float[count];
            CurrentLifetime = new float[count];
            LifeProgress = new float[count];

           
            hybrid = new SOALifetime[hybridCount];
            for (int i = 0; i < hybridCount; i++)
            {
                hybrid[i] = new SOALifetime(8);
            }
            
            hybridData = Marshal.AllocHGlobal(Marshal.SizeOf<float>() * 3 * count);
        }

        [Benchmark(Baseline = true)]
        public void StepWise()
        {
            // Multiple calculatations for more accurate result
            for (int q = 0; q < 100; q++)
            {
                // Progress lifetime with delta time
                CPUMath.Add(CurrentLifetime, 1f / 60f, 0, count);

                // Calculate lifetime progress
                // Divide currentlifetime with lifetime and store in lifeprogress. 
                CPUMath.DivStore(LifeProgress, CurrentLifetime, Lifetime, 0, count);
            }
        }

        [Benchmark]
        public unsafe void Single()
        {
            for (int q = 0; q < 100; q++)
            {
                float stp = 1f / 60f;

                fixed (float* aPtr = Lifetime, bPtr = CurrentLifetime, cPtr = LifeProgress)
                {
                    // Load Constatant
                    var stepVector = Avx.BroadcastScalarToVector256(&stp);

                    for (int i = 0; i < count; i += 8)
                    {
                        // Load Current life
                        var currentLifetime = Avx.LoadVector256(&bPtr[i]);
                        currentLifetime = Avx.Add(currentLifetime, stepVector);

                        Avx.Store(&bPtr[i], currentLifetime);
                        var totalLifeTime = Avx.LoadVector256(&aPtr[i]);
                        Avx.Store(&cPtr[i], Avx.Divide(currentLifetime, totalLifeTime));
                    }
                }
            }
        }

        [Benchmark]
        public unsafe void SingleHybrid()
        {
            for (int q = 0; q < 100; q++)
            {
                float stp = 1f / 60f;
                // Load Constatant
                var stepVector = Avx.BroadcastScalarToVector256(&stp);
                for (int i = 0; i < hybridCount; i++)
                {
                    fixed (float* aPtr = hybrid[i].Lifetime, bPtr = hybrid[i].CurrentLifetime, cPtr = hybrid[i].LifeProgress)
                    {
                        // Load Current life
                        var currentLifetime = Avx.LoadVector256(&bPtr[0]);
                        currentLifetime = Avx.Add(currentLifetime, stepVector);

                        Avx.Store(&bPtr[0], currentLifetime);
                        var totalLifeTime = Avx.LoadVector256(&aPtr[0]);
                        Avx.Store(&cPtr[0], Avx.Divide(currentLifetime, totalLifeTime));
                        
                    }
                }
            }
        }

        [Benchmark]
        public unsafe void SingleHybridUnsafe()
        {
            
            for (int q = 0; q < 100; q++)
            { 
                float stp = 1f / 60f;
                // Load Constatant
                var stepVector = Avx.BroadcastScalarToVector256(&stp);

                float* s = (float*)hybridData;
                Vector256<float> currentLifetime;
                Vector256<float> totalLifeTime;
                for (int i = 0; i < 3000000; i += 24)
                {
                    // Load Current life
                    currentLifetime = Avx.LoadVector256(&s[i+8]);
                    currentLifetime = Avx.Add(currentLifetime, stepVector);
                    totalLifeTime = Avx.LoadVector256(&s[i]);
                    Avx.Store(&s[i+16], Avx.Divide(currentLifetime, totalLifeTime));
                    Avx.Store(&s[i + 8], currentLifetime);
                }
            }
        }
    }





    [StructLayout(LayoutKind.Sequential)]
    public struct SOA
    {
        public float x;
        public float y;
        public float z;
    }

    public class Iteration
    {
        int count = 100000000;
        public SOA[] dataArray;
        public Memory<SOA> dataMem;
        public IntPtr dataPtr;

        public Iteration()
        {
            Random random = new Random();

            dataArray = new SOA[count];
            dataMem = new Memory<SOA>(dataArray);
            dataPtr = Marshal.AllocHGlobal(Marshal.SizeOf<SOA>() * count);

        }

        [Benchmark]
        public  void Array()
        {
            for (int i = 0; i < dataArray.Length; i++)
            {
                dataArray[i].x = 1f;
            }
        }

        [Benchmark]
        public unsafe void ArrayFixed()
        {
            fixed (SOA* aPtr = dataArray)
            {
                for (int i = 0; i < dataArray.Length; i++)
                {
                    aPtr[i].x = 1f;
                }
            }
            
        }

        [Benchmark]
        public unsafe void Memory()
        {
            for (int i = 0; i < dataMem.Length; i++)
            {
                dataMem.Span[i].x = 1f;
            }
        }

        [Benchmark]
        public unsafe void Ptr()
        {
            SOA* ptr = (SOA*)dataPtr;
            for (int i = 0; i < count; i++)
            {
                ptr[i].x = 1f;
            }
        }

    }
    
    public static class Program
	{
		[STAThread]
		static void Main()
		{
            BenchmarkRunner.Run<Ma>();
            Console.ReadKey();
		}
	}
}
