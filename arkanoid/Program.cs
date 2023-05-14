class Block
{
    public int X { get; set; }
    public int Y { get; set; }
    public ConsoleColor Color { get; set; }
}
class Program
{
    static int paddleX;
    static int ballX, ballY;
    static int ballDX = 1, ballDY = -1;
    static Block[] blocks = new Block[36];

    static void Main()
    {
        Console.CursorVisible = false;
        Console.WindowWidth = 120;
        Console.WindowHeight = 44;

        paddleX = Console.WindowWidth / 2 - 6;
        ballX = Console.WindowWidth / 2;
        ballY = Console.WindowHeight - 2;

        Random rand = new Random();

        // Initialize the blocks with random colors
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i] = new Block()
            {
                X = (i % 12) * 10 + 1,
                Y = (i / 12) * 2 + 1,
                Color = (ConsoleColor)rand.Next(1, 16)
            };
        }

        int time = 0;

        while (true)
        {
            Console.Clear();
            DrawPaddle();
            DrawBlocks();
            DrawBall();

            if (time % 2 == 0)
            {

                ballX += ballDX;
                ballY += ballDY;


                if (ballX == 0 || ballX == Console.WindowWidth - 1)
                {
                    ballDX *= -1;
                }

                if (ballY == 0)
                {
                    ballDY *= -1;
                }
                if (ballY == Console.WindowHeight - 2 && ballX >= paddleX - 4 && ballX <= paddleX + 7)
                {
                    ballDY *= -1;
                }
            }


            // Check for collision with the blocks
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] != null && ballY == blocks[i].Y && ballX >= blocks[i].X && ballX < blocks[i].X + 10)
                {
                    ballDY *= -1;
                    blocks[i] = null; // Remove the block
                }
            }

            // Check for game over
            if (ballY == Console.WindowHeight - 1)
            {
                Console.WriteLine("Game Over!");
                Console.ReadLine();
                break;
            }

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.LeftArrow && paddleX > 0)
                    paddleX--;
                if (key.Key == ConsoleKey.RightArrow && paddleX < Console.WindowWidth - 13)
                    paddleX++;
            }

            System.Threading.Thread.Sleep(1);
            time++;
        }
    }

    static void DrawPaddle()
    {
        Console.SetCursorPosition(paddleX, Console.WindowHeight - 1);
        Console.Write("============");
    }

    static void DrawBall()
    {
        Console.SetCursorPosition(ballX, ballY);
        Console.Write("O");
    }

    static void DrawBlocks()
    {
        foreach (Block block in blocks)
        {
            if (block != null)
            {
                Console.SetCursorPosition(block.X - 1, block.Y);
                Console.ForegroundColor = block.Color;
                Console.Write("##########");
                Console.ResetColor();
            }
        }
    }

}

