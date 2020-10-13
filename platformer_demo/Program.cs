using System;

namespace platformer_demo
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new SimplePlatformer())
                game.Run();
        }
    }
}
