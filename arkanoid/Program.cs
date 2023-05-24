using System;
using block;
using arkanoid;


class Program
{
    public static void Main()
    {
        bool saveExists = File.Exists("game.sav");

        while (true)
        {
            Console.ForegroundColor= ConsoleColor.White;
            Console.CursorVisible = false;
            Console.SetWindowSize(ArkanoidGame.Width, ArkanoidGame.Height);
            Console.SetBufferSize(ArkanoidGame.Width, ArkanoidGame.Height);
            Console.Title = "Arkanoid";
            Console.Clear();
            Console.WriteLine("1. Play new game");
            Console.WriteLine("2. Exit");

            if (saveExists)
            {
                Console.WriteLine("3. Continue from save");
            }

            string output = Console.ReadLine();
            if (output == "1")
            {
                Console.WriteLine("OK");

                ArkanoidGame game = new ArkanoidGame();
                game.StartNewGame();
                game.Run();
            }
            else if (output == "2")
            {
                break;
            }
            else if (output == "3" && saveExists)
            {
                ArkanoidGame game = new ArkanoidGame();
                game.LoadGame();
                game.Run();
            }
            else
            {
                Console.WriteLine("Wrong input");
                Thread.Sleep(250);
            }
        }
    }
}
