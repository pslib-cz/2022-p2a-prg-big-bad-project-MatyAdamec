using block;
using bullet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ui_handler;
public class UIHandler
{
    public void DrawHud(int score, int lives, int width, TimeSpan GunReloadTime, DateTime lastBulletTime, GunType gunType)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(2, 0);
        Console.Write($"Score: {score}");
        Console.SetCursorPosition(width - 10, 0);
        Console.Write($"Lives: {lives}");
        double countdown = Math.Max(0, GunReloadTime.TotalSeconds - (DateTime.Now - lastBulletTime).TotalSeconds);

        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(width - 7 - gunType.ToString().Length, Console.WindowHeight - 2);
        Console.Write("  " + gunType + " " + countdown.ToString("F1"));
    }

    public void DrawBullets(List<Bullet> bullets)
    {
        foreach (Bullet bullet in bullets)
        {
            if (bullet.GunType == GunType.Glock)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (bullet.GunType == GunType.Sniper)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            else if (bullet.GunType == GunType.AK47)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
            }
            bullet.Draw();
        }
    }

    public void DrawBlocks(Block[,] map, int numBlocksY, int numBlocksX, int blockHeight, int blockWidth)
    {
        for (int y = 0; y < numBlocksY; y++)
        {
            for (int x = 0; x < numBlocksX; x++)
            {
                if (map[x, y] != null)
                {
                    Block block = map[x, y];
                    bool blockDestroyed = block.Destroyed;

                    Console.ForegroundColor = block.Color;

                    for (int i = 0; i < blockHeight; i++)
                    {
                        for (int j = 0; j < blockWidth; j++)
                        {
                            int drawX = block.X + j;
                            int drawY = block.Y + i;

                            Console.SetCursorPosition(drawX, drawY);
                            Console.Write("█");
                        }
                    }

                }
            }
        }
    }

    public void RemoveBlocks(Block[,] map, int numBlocksY, int numBlocksX, int blockHeight, int blockWidth)
    {
        for (int y = 0; y < numBlocksY; y++)
        {
            for (int x = 0; x < numBlocksX; x++)
            {
                if (map[x, y] != null)
                {
                    Block block = map[x, y];
                    bool blockDestroyed = block.Destroyed;

                    if (blockDestroyed && !block.IsRemoved)
                    {
                        for (int i = 0; i < blockHeight; i++)
                        {
                            for (int j = 0; j < blockWidth; j++)
                            {
                                int clearX = block.X + j;
                                int clearY = block.Y + i;

                                Console.SetCursorPosition(clearX, clearY);
                                Console.Write(" ");
                            }
                        }

                        block.IsRemoved = true;
                    }
                }
            }
        }
    }

    public void DrawPauseMenu()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(
            "                               |   _____                       __  __                   |\n" +
            "                               |  |  __ \\                     |  \\/  |                  |\n" +
            "                               |  | |__) |_ _ _   _ ___  ___  | \\  / | ___ _ __  _   _  |\n" +
            "                               |  |  ___/ _` | | | / __|/ _ \\ | |\\/| |/ _ \\ '_ \\| | | | |\n" +
            "                               |  | |  | (_| | |_| \\__ \\  __/ | |  | |  __/ | | | |_| | |\n" +
            "                               |  |_|   \\__,_|\\__,_|___/\\___| |_|  |_|\\___|_| |_|\\__,_| |\n" +
            "                               |________________________________________________________|\n" +
            "\n" +
            "\n" +
            "\n" +
            "\n" +
            "                                               ########################\n" +
            "                                               #                      #\n" +
            "                                               #     1. Continue      #\n" +
            "                                               #                      #\n" +
            "                                               ########################\n" +
            "\n" +
            "\n" +
            "\n" +
            "                                               ########################\n" +
            "                                               #                      #\n" +
            "                                               #      2. Reload       #\n" +
            "                                               #                      #\n" +
            "                                               ########################\n" +
            "\n" +
            "\n" +
            "\n" +
            "                                               ########################\n" +
            "                                               #                      #\n" +
            "                                               #     3. Save Game     #\n" +
            "                                               #                      #\n" +
            "                                               ########################\n" +
            "\n" +
            "\n" +
            "\n" +
            "                                               ########################\n" +
            "                                               #                      #\n" +
            "                                               #     4. Main Menu     #\n" +
            "                                               #                      #\n" +
            "                                               ########################");
    }

    public void Countdown()
    {
        Console.Clear();

        string[] countdown3 = new string[]
        {
    "  ____   ",
    " |___ \\  ",
    "   __) | ",
    "  |__ <  ",
    "  ___) | ",
    " |____/ ",
    "         "
        };
        string[] countdown2 = new string[]
        {
    "   ___   ",
    "  |__ \\  ",
    "     ) | ",
    "    / /  ",
    "   / /_  ",
    "  |____| ",
    "         "
        };
        string[] countdown1 = new string[]
        {
    "    __   ",
    "   /_ |  ",
    "    | |  ",
    "    | |  ",
    "    | |  ",
    "    |_|  ",
    "         "
        };

        for (int i = 3; i > 0; i--)
        {
            string[] countdownGo = new string[] { };
            if (i == 3)
            {
                countdownGo = countdown3;
                Console.ForegroundColor = ConsoleColor.Red;
            }
            if (i == 2)
            {
                countdownGo = countdown2;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }
            if (i == 1)
            {
                countdownGo = countdown1;
                Console.ForegroundColor = ConsoleColor.Green;
            }

            for (int x = 0; x < countdownGo.Length; x++)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 2 - 4 + x);
                Console.WriteLine(countdownGo[x]);
            }

            Thread.Sleep(250);
            Console.Clear();

        }
    }

    public void DrawPaddleAndBall(int paddleX, int paddleHeight, int paddleWidth, int ballX, int ballY, int ballSize, int height, int Width)
    {
        Console.ForegroundColor = ConsoleColor.White;

        for (int x = 0; x < paddleX; x++)
        {
            Console.SetCursorPosition(x, height - paddleHeight);
            Console.Write(" ");
        }

        for (int i = 0; i < paddleHeight; i++)
        {
            for (int j = 0; j < paddleWidth; j++)
            {
                int drawX = paddleX + j;
                int drawY = height - paddleHeight + i;

                Console.SetCursorPosition(drawX, drawY);
                Console.Write("█");
            }
        }

        for (int x = paddleX + paddleWidth; x < Width; x++)
        {
            Console.SetCursorPosition(x, height - paddleHeight);
            Console.Write(" ");
        }

        Console.ForegroundColor = ConsoleColor.Red;

        for (int i = 0; i < ballSize; i++)
        {
            for (int j = 0; j < ballSize; j++)
            {
                int drawX = ballX + j;
                int drawY = ballY + i;

                Console.SetCursorPosition(drawX, drawY);
                Console.Write("O");
            }
        }
    }

    public void BallFall(int drawX, int drawY)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        for (int i = 0; i < 8; i++)
        {
            Console.SetCursorPosition(drawX, drawY);
            Console.Write(" ");
            Thread.Sleep(100 - 8 * i);
            Console.SetCursorPosition(drawX, drawY);
            Console.Write("O");
            Thread.Sleep(100 - 8 * i);
        }
        Console.SetCursorPosition(drawX, drawY);
        Console.Write(" ");
    }

    public void GameFinish(bool win, int ballX, int ballY, int score)
    {
        string[] _resultOver = new string[] { };
        if (win)
        {
            _resultOver = new string[]
            {
            "                                                      ",
            "  __     __          __          _______ _   _   _   ",
            "  \\ \\   / /          \\ \\        / /_   _| \\ | | | |  ",
            "   \\ \\_/ /__  _   _   \\ \\  /\\  / /  | | |  \\| | | |  ",
            "    \\   / _ \\| | | |   \\ \\/  \\/ /   | | | . ` | | |  ",
            "     | | (_) | |_| |    \\  /\\  /   _| |_| |\\  | |_|  ",
            "     |_|\\___/ \\__,_|     \\/  \\/   |_____|_| \\_| (_)  ",
            "                                                      ",
            "                                                      "
            };
        }
        else
        {
            BallFall(ballX, ballY);

            _resultOver = new string[]
            {
                "                                                      ",
                "   _____                         ____                 ",
                "  / ____|                       / __ \\                ",
                " | |  __  __ _ _ __ ___   ___  | |  | |_   _____ _ __ ",
                " | | |_ |/ _` | '_ ` _ \\ / _ \\ | |  | \\ \\ / / _ \\ '__|",
                " | |__| | (_| | | | | | |  __/ | |__| |\\ V /  __/ |   ",
                "  \\_____|\\__,_|_| |_| |_|\\___|  \\____/  \\_/ \\___|_|   ",
                "                                                      ",
                "  score:" + score + "                                             ",
                "                                                      "
            };
        }

        Console.ForegroundColor = ConsoleColor.White;
        for (int i = 0; i < _resultOver.Length; i++)
        {
            Console.SetCursorPosition(Console.WindowWidth / 2 - 27, Console.WindowHeight / 2 - 4 + i);
            Console.WriteLine(_resultOver[i]);
            Thread.Sleep(100);
        }

        Thread.Sleep(1000);

        Console.Clear();
    }

    public void SaveMenu(int autosave, bool save)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("<< Back (B/b)");
        Console.ForegroundColor = ConsoleColor.White;
        if (save)
        {
            Console.Write(
                "                |   _                     _    _____                        |\n" +
                "                             |  | |                   | |  / ____|                       |\n" +
                "                             |  | |     ___   __ _  __| | | |  __  __ _ _ __ ___   ___   |\n" +
                "                             |  | |    / _ \\ / _` |/ _` | | | |_ |/ _` | '_ ` _ \\ / _ \\  |\n" +
                "                             |  | |___| (_) | (_| | (_| | | |__| | (_| | | | | | |  __/  |\n" +
                "                             |  |______\\___/ \\__,_|\\__,_|  \\_____|\\__,_|_| |_| |_|\\___|  |\n" +
                "                             |___________________________________________________________|\n\n\n\n\n"
            );
        }
        else
        {
            Console.Write(
                "                 |    _____                    _____                        |\n" +
                "                              |   / ____|                  / ____|                       |\n" +
                "                              |  | (___   __ ___   _____  | |  __  __ _ _ __ ___   ___   |\n" +
                "                              |   \\___ \\ / _` \\ \\ / / _ \\ | | |_ |/ _` | '_ ` _ \\ / _ \\  |\n" +
                "                              |   ____) | (_| |\\ V /  __/ | |__| | (_| | | | | | |  __/  |\n" +
                "                              |  |_____/ \\__,_| \\_/ \\___|  \\_____|\\__,_|_| |_| |_|\\___|  |\n" +
                "                              |__________________________________________________________|\n\n\n\n\n"
            );
        }

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
                            string isAutosave = "";
                            if (i == autosave - 1)
                            {
                                isAutosave = "                                           #     Auto Save / Fast Save     #\n";
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(
                                "                                           #################################\n" +
                                "                                           #                               #\n" +
                               $"                                           #             Save {i + 1}            #\n" +
                                "                                           #                               #\n" +
                               $"                                           #            Level: {level}           #\n" +
                               $"                                           #            Score: {score}           #\n" + isAutosave +
                                "                                           #                               #\n" +
                                "                                           #################################\n\n\n");
                        }
                    }
                }
            }
            else
            {
                string isAutosave = "";
                if (i == autosave - 1)
                {
                    isAutosave = "                                           #     Auto Save / Fast Save     #\n";
                }
                if (save)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.Write(
                "                                           #################################\n" +
                "                                           #                               #\n" +
               $"                                           #             Save {i + 1}            #\n" +
                "                                           #                               #\n" +
                "                                           #      File does not exist      #\n" + isAutosave +
                "                                           #                               #\n" +
                "                                           #################################\n\n\n");
            }

        }
    }

    public void MainMenu()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;
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
            "                                             #############################"
        );
    }

    public void End()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition( 21, 12 );
        Console.Write(
            "  _____                            _         _       _   _                 _ \n" +
            "                      / ____|                          | |       | |     | | (_)               | |\n" +
            "                     | |     ___  _ __   __ _ _ __ __ _| |_ _   _| | __ _| |_ _  ___  _ __  ___| |\n" +
            "                     | |    / _ \\| '_ \\ / _` | '__/ _` | __| | | | |/ _` | __| |/ _ \\| '_ \\/ __| |\n" +
            "                     | |___| (_) | | | | (_| | | | (_| | |_| |_| | | (_| | |_| | (_) | | | \\__ \\_|\n" +
            "                      \\_____\\___/|_| |_|\\__, |_|  \\__,_|\\__|\\__,_|_|\\__,_|\\__|_|\\___/|_| |_|___(_)\n" +
            "                                         __/ |                                                    \n" +
            "                                        |___/                                                     \n" +
            "\n                                      Thank you for playing my take on Arkanoid! :)");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(
            "\n\n\n\n                                        .;;;, .;;;,                   .;;;, .;;;,\r\n" +
            "                                   .;;;,;;;;;,;;;;;,.;;;,       .;;;.,;;;;;,;;;;;,;;;.\r\n" +
            "                                  ;;;;xXXxXXxXXxXXxXXx;;;. .,. .;;;xXXxXXxXXxXXxXX;;;;;\r\n" +
            "                              .,,.`xXX'             `xXXx,;;;;;,xXXx'            `XXx;;,,.\r\n" +
            "                             ;;;;xXX'                  `xXXx;xXXx'                 `XXx;;;;\r\n" +
            "                             `;;XX'                       `XXX'                      `XX;;'\r\n" +
            "                            ,;;,XX                         `X'                        XX,;;,\r\n" +
            "                            ;;;;XX,                                                  ,XX;;;;\r\n" +
            "                             ``.;XX,                                                ,XX;,''\r\n" +
            "                               ;;;;XX,                                            ,XX;;;;\r\n" +
            "                                ```.;XX,                                        ,XX;,'''\r\n" +
            "                                   ;;;;XX,                                    ,XX;;;;\r\n" +
            "                                    ```,;XX,                                ,XX;,'''\r\n" +
            "                                        ;;;;XX,                          ,XX;;;;\r\n" +
            "                                         ````,;XX,                    ,XX;, '''\r\n" +
            "                                             ;;;;;XX,              ,XX;;;;\r\n" +
            "                                              `````,;XX,        ,XX;,''''\r\n" +
            "                                                   ;;;;;XX,  ,XX;;;;;\r\n" +
            "                                                    `````;;XX;;'''''\r\n" +
            "                                                         `;;;;'\r\n");

        Console.ReadKey();
    }

    public void Replay()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(46, 24);
        Console.Write("Replay level again?... (Y/N)");
        Console.SetCursorPosition(60, 25);
    }

    public void Tutorial(bool notFirstTime)
    {
        if (!notFirstTime)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            string intro = "\n\n\n\n\n\n\n\n\n\n\n\n                                                        Welcome! \n                                                Welcome to my Arkanoid game! \n                  Your goal is to destroy all the blocks on the screen by hitting them with the ball.";
            for (int i = 0; i < intro.Length; i++)
            {
                Console.Write(intro[i]);
                if (intro[i] != ' ')
                {
                    Thread.Sleep(5);
                }
            }
            Console.SetCursorPosition(92, 47);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Thread.Sleep(1000);
            Console.Write("Press any key to continue...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();


            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(59, 45);
            Console.Write("O");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(57, 36);
            Console.Write(
                ";;;;;\r\n" +
                "                                                         ;;;;;\r\n" +
                "                                                         ;;;;;\r\n" +
                "                                                         ;;;;;\r\n" +
                "                                                         ;;;;;\r\n" +
                "                                                       ..;;;;;..\r\n" +
                "                                                        ':::::'\r\n" +
                "                                                          ':`\r\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(51, 18);
            Console.Write(
                "This is your Ball... \n                   Your goal is to destroy all the blocks on the screen by hitting them with the ball.");
            Console.SetCursorPosition(92, 47);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Press any key to continue...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();


            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(51, 47);
            Console.Write("██████████████████");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(57, 37);
            Console.Write(
                ";;;;;\r\n" +
                "                                                         ;;;;;\r\n" +
                "                                                         ;;;;;\r\n" +
                "                                                         ;;;;;\r\n" +
                "                                                         ;;;;;\r\n" +
                "                                                       ..;;;;;..\r\n" +
                "                                                        ':::::'\r\n" +
                "                                                          ':`\r\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(49, 18);
            Console.Write(
                "This is your Paddle... \n     You control the paddle at the bottom of the screen. You can move it left and right using the arrow keys (<- , ->). ");
            Console.SetCursorPosition(92, 47);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Press any key to continue...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();


            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" Score: X\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(
                "\n        .\r\n" +
                "      .:;:.\r\n" +
                "    .:;;;;;:.\r\n" +
                "      ;;;;;\r\n" +
                "      ;;;;;\r\n" +
                "      ;;;;;\r\n" +
                "      ;;;;;\r\n" +
                "      ;;;;;\r\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(32, 23);
            Console.Write(
                "You have score that increases when you destroy a block. \n                                      Your score will be reset after each level. ");
            Console.SetCursorPosition(92, 47);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Press any key to continue...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();


            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(110, 0);
            Console.Write(" Lives: X\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(
                "\n                                                                                                                   .\r\n" +
                "                                                                                                                 .:;:.\r\n" +
                "                                                                                                               .:;;;;;:.\r\n" +
                "                                                                                                                 ;;;;;\r\n" +
                "                                                                                                                 ;;;;;\r\n" +
                "                                                                                                                 ;;;;;\r\n" +
                "                                                                                                                 ;;;;;\r\n" +
                "                                                                                                                 ;;;;;\r\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(30, 23);
            Console.Write(
                "You have a lives indicator that decreeses when your ball falls\n                                      Your lives will be reset after each level. ");
            Console.SetCursorPosition(92, 47);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Press any key to continue...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();


            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(108, Console.WindowHeight - 2);
            Console.Write("GunType: X");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(0, 36);
            Console.Write(
                "                                                                                                                 ;;;;;\r\n" +
                "                                                                                                                 ;;;;;\r\n" +
                "                                                                                                                 ;;;;;\r\n" +
                "                                                                                                                 ;;;;;\r\n" +
                "                                                                                                                 ;;;;;\r\n" +
                "                                                                                                               ..;;;;;..\r\n" +
                "                                                                                                                ':::::'\r\n" +
                "                                                                                                                  ':`\r\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0, 19);
            Console.Write(
                "                       You can shoot bullets by pressing the space bar. \n" +
                "                       The bullets will destroy the blocks they hit. \n" +
                "                       You can change the type of the gun by pressing the number keys 1, 2 and 3. \n" +
                "                       The gun types are: \n" +
                "\t                       Glock - one bullet can destroy 1 block and has a reload time of 1 second. \n" +
                "\t                       Sniper - one bullet can destroy 2 blocks and has a reload time of 2 seconds. \n" +
                "\t                       AK47 - one bullet can destroy 1 block and has a reload time of 3 seconds. \n" +
                "");
            Console.SetCursorPosition(92, 47);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Press any key to continue...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();

            Console.Clear();
            Console.SetCursorPosition(45, 15);
            Console.Write("To sum it up:\n\n" +
                "                                             \t <- , -> : paddle movement\n" +
                "                                             \t space bar : gun shot\n" +
                "                                             \t S : Quick Save\n" +
                "                                             \t escape : Pause Menu\n" +
                "                                             \t num 1 : change the gun to Glock\n" +
                "                                             \t num 2 : change the gun to Sniper\n" +
                "                                             \t num 3 : change the gun to AK-47\n\n");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("                                             Just for you ... to test \n                                              \t C : level ++");
            Console.ReadKey();
        }
    }
}