using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;

namespace Core.Particles
{
    [SimpleJob(RunStrategy.Monitoring, targetCount: 1)]
    public class ParticleBenchmark
    {
		SimulatorCPU simulatorCPU;
        Emitter emitter;
        public ParticleBenchmark()
        {
            var settings = new ParticleSystemSettings()
            {
                Drag = 1f,
            };

            this.simulatorCPU = new SimulatorCPU(800000, settings);
            
            var emitterSettings = new EmitterSettings()
            {
                RateOverTime = 800000,
                Shape = new Shapes.Rectangle() { Size = new System.Numerics.Vector2(2f, 2f) }
            };
            emitter = new Emitter(emitterSettings, simulatorCPU);

        }

        [Benchmark]
        public void test()
        {
            this.simulatorCPU.Play();
            while (simulatorCPU.Tick < 1000)
            {

            }
        }
    }


	public static class Program
	{
		[STAThread]
		static void Main()
		{
            BenchmarkRunner.Run<ParticleBenchmark>();
		}
	}
}
