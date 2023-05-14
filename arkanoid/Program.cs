using System;
using block;
using Arkanoid;

namespace arkanoid;
class Program
{
    public static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("play: 1 \nexit: 2");
            string output = Console.ReadLine(); 
            if (output == "1")
            {
                Console.WriteLine("ok");

                ArkanoidGame game = new ArkanoidGame();
                game.Run();

            }else if (output == "2")
            {
                break;
            }
            else
            {
                Console.WriteLine("wrong input");
            }
        }


    }
}

/*
 TODO:
    home screen
    paddle modes
    levels
    win screen

 DONE:
    draw paddle, lives, score, ball
 */