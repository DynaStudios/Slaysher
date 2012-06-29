using System;

namespace Slaysher
{
#if WINDOWS || XBOX

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            using (Engine game = new Engine())
            {
#if DEBUG
                game.Username = "test";
                game.Password = "test";
#else
                if (args.Length != 2)
                {
                    return;
                }
                else
                {
                    game.Username = args[0];
                    game.Password = args[1];
                }
#endif
                game.Run();
            }
        }
    }

#endif
}