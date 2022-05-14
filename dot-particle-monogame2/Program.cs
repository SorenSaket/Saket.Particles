using System;

namespace Core.Particles
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new XNAGame())
                game.Run();
        }
    }
}
