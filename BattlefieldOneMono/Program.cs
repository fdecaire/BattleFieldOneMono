using System;
using Autofac;

namespace BattlefieldOneMono
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
			IocContainer.Setup();

	        using (var game = new GameMain(IocContainer.Container.Resolve<IGameBoard>()))
	        {
		        game.Run();
	        }
		}
    }
#endif
}
