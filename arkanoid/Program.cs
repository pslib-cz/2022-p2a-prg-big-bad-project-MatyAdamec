using System;
using block;
using arkanoid;


class Program
{
    public static void Main()
    {
        bool saveExists = File.Exists("save_0.sav") || File.Exists("save_1.sav") || File.Exists("save_2.sav");

        while (true)
        {
            Console.SetWindowSize(ArkanoidGame.Width, ArkanoidGame.Height);
            Console.SetBufferSize(ArkanoidGame.Width, ArkanoidGame.Height);
            Console.Title = "Arkanoid";            
            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorVisible = false;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.Write(
    "                            |      _____         __                         .__    .___   |\n" +
    "                            |     /  _  \\_______|  | _______    ____   ____ |__| __| _/   |\n" +
    "                            |    /  /_\\  \\_  __ \\  |/ /\\__  \\  /    \\ /  _ \\|  |/ __ |    |\n" +
    "                            |   /    |    \\  | \\/    <  / __ \\|   |  (  <_> )  / /_/ |    |\n" +
    "                            |   \\____|__  /__|  |__|_ \\(____  /___|  /\\____/|__\\____ |    |\n" +
    "                            |           \\/           \\/     \\/     \\/               \\/    |\n" +
    "                            |_____________________________________________________________|\n" +
    "\n" +
    "\n" +
    "\n" +
    "\n" +
    "                                             #############################\n" +
    "                                             #                           #\n" +
    "                                             #                           #\n" +
    "                                             #     1. Play new game      #\n" +
    "                                             #                           #\n" +
    "                                             #                           #\n" +
    "                                             #############################\n" +
    "\n" +
    "\n" +
    "\n" +
    "                                             #############################\n" +
    "                                             #                           #\n" +
    "                                             #                           #\n" +
    "                                             #   2. Continue from save   #\n" +
    "                                             #                           #\n" +
    "                                             #                           #\n" +
    "                                             #############################\n" +
    "\n" +
    "\n" +
    "\n" +
    "                                             #############################\n" +
    "                                             #                           #\n" +
    "                                             #                           #\n" +
    "                                             #       3. Exit game        #\n" +
    "                                             #                           #\n" +
    "                                             #                           #\n" +
    "                                             #############################");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(49, 42);
            Console.Write("Your choice (1-4): ");

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
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("<< Back (B/b)");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(
                        "                |   _                     _    _____                        |\n" +
                        "                             |  | |                   | |  / ____|                       |\n" +
                        "                             |  | |     ___   __ _  __| | | |  __  __ _ _ __ ___   ___   |\n" +
                        "                             |  | |    / _ \\ / _` |/ _` | | | |_ |/ _` | '_ ` _ \\ / _ \\  |\n" +
                        "                             |  | |___| (_) | (_| | (_| | | |__| | (_| | | | | | |  __/  |\n" +
                        "                             |  |______\\___/ \\__,_|\\__,_|  \\_____|\\__,_|_| |_| |_|\\___|  |\n" +
                        "                             |___________________________________________________________|\n\n\n\n\n"
                        );


                    for (int i = 0; i <= 2; i++)
                    {
                        string saveFileName = $"save_{i}.sav";
                        if (File.Exists(saveFileName))
                        {
                            using (StreamReader reader = new StreamReader(saveFileName))
                            {
                                for (int j = 0; j < 5; j++)
                                {
                                    reader.ReadLine();
                                }

                                string scoreLine = reader.ReadLine();
                                int score;
                                if (int.TryParse(scoreLine, out score))
                                {
                                    string levelLine = reader.ReadLine();
                                    int level;
                                    if (int.TryParse(levelLine, out level))
                                    {
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Console.Write(
                                            "                                           #################################\n" +
                                            "                                           #                               #\n" +
                                           $"                                           #             Save {i + 1}            #\n" +
                                            "                                           #                               #\n" +
                                           $"                                           #            Level: {level}           #\n" +
                                           $"                                           #            Score: {score}           #\n" +
                                            "                                           #                               #\n" +
                                            "                                           #################################\n\n\n");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(
                            "                                           #################################\n" +
                            "                                           #                               #\n" +
                           $"                                           #             Save {i + 1}            #\n" +
                            "                                           #                               #\n" +
                            "                                           #      File does not exist      #\n" +
                            "                                           #                               #\n" +
                            "                                           #################################\n\n\n");
                        }
                    }
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
            else if (input == "3" && saveExists)
            {
                break;
            }
        }
    }
}
