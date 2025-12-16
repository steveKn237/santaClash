using System;

namespace SantaClash
{
    /// <summary>
    /// Point d'entrée principal de l'application
    /// </summary>
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}
