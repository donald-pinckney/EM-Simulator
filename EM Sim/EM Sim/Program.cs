using System;

namespace EM_Sim
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (EMSim game = new EMSim())
            {
                game.Run();
            }
        }
    }
#endif
}

