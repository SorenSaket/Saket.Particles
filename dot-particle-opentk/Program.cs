using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;

namespace Core.Particles
{
	public static class Program
	{
		[STAThread]
		static void Main()
		{
			Debug.WriteLine("Processor count: " + Environment.ProcessorCount);
			Debug.WriteLine("AVX: " + Avx2.IsSupported);
			Debug.WriteLine("float count simd " + System.Numerics.Vector<float>.Count);



			var nativeWindowSettings = new NativeWindowSettings()
			{
				Size = new Vector2i(1920, 1080),
				Title = "Particle System"
			};

			var gameWindowSettings = GameWindowSettings.Default;

			using (var window = new OpenTKGame(gameWindowSettings, nativeWindowSettings))
			{
				window.Run();
			}

			//using (var game = new XNAGame())
			//	game.Run();
		}


	}
}
