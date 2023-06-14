using System;
using block;
using arkanoid;
using System.Media;
using ui_handler;

class Program
{
    public static void Main()
    {

        bool saveExists = File.Exists("save_0.sav") || File.Exists("save_1.sav") || File.Exists("save_2.sav");
        UIHandler _UIHandler = new UIHandler();
        Console.SetWindowSize(ArkanoidGame.Width, ArkanoidGame.Height);
        Console.SetBufferSize(ArkanoidGame.Width, ArkanoidGame.Height);
        Console.Title = "Arkanoid";
        Console.CursorVisible = false;
        _UIHandler.Tutorial(saveExists);

        while (true)
        {
            _UIHandler.MainMenu();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(49, 42);
            Console.Write("Your choice (1-3): ");

            string input = Console.ReadLine();
            if (input == "1")
            {
                ArkanoidGame game = new ArkanoidGame();
                game.StartNewGame();
                game.Run();
            }
            else if (input == "2")
            {
                while (true)
                {
                    Console.ForegroundColor= ConsoleColor.White;
                    _UIHandler.SaveMenu(1, true);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.SetCursorPosition(42, 42);
                    Console.Write("Choose a SAVE to load the game (1-3): ");
                    string saveIndexStr = Console.ReadLine();
                    if (int.TryParse(saveIndexStr, out int saveIndex) && saveIndex >= 1 && saveIndex <= 3)
                    {
                        string saveFileName = $"save_{saveIndex - 1}.sav";
                        if (File.Exists(saveFileName))
                        {

                            ArkanoidGame game = new ArkanoidGame();
                            game.LoadGame(saveFileName);
                            game.Run();
                            break;
                        }
                        else
                        {
                            Console.ForegroundColor= ConsoleColor.Red;
                            Console.Write("\n                                                Save file does not exist.");
                            Thread.Sleep(250);
                        }
                    }
                    else if (saveIndexStr == "B" || saveIndexStr == "b")
                    {
                        break;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("\n                                                Invalid save index.");
                        Thread.Sleep(250);
                    }
                }
            }
            else if (input == "3")
            {
                break;
            }
        }
    }
}
