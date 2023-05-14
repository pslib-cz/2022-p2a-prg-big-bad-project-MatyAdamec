using block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid;
public class ArkanoidGame
{
    private const int Width = 120;
    private const int Height = 48;
    private const int BlockWidth = Width / 10;
    private const int BlockHeight = 2;
    private const int NumBlocksX = Width / BlockWidth;
    private const int NumBlocksY = 4;
    private const int PaddleWidth = 18;
    private const int PaddleHeight = 1;
    private const int BallSize = 1;
    private const int InitialLives = 3;


    private readonly Random _random = new Random();
    private readonly Block[,] _blocks = new Block[NumBlocksX, NumBlocksY];
    private readonly List<int> _keysPressed = new List<int>();
    private readonly int[] _ballDx = { -1, 1 };
    private readonly int[] _ballDy = { -1, 1 };

    private int _score = 0;
    private int _previousScore = 0;
    private int _previousLives = InitialLives;
    private int _lives = InitialLives;
    private int _ballX = Width / 2;
    private int _ballY = Height - PaddleHeight - BallSize - 1;
    private int _ballDxIndex = 0;
    private int _ballDyIndex = 0;
    private int _paddleX = Width / 2 - PaddleWidth / 2;
    private bool _gameOver = false;

    public ArkanoidGame()
    {

        Console.CursorVisible = false;
        Console.SetWindowSize(Width, Height);
        Console.SetBufferSize(Width, Height);
        Console.Title = "Arkanoid";

        Console.Clear();
        GenerateBlocks();
        DrawBlocks();
    }

    public void Run()
    {

        int ballTime = 0;
        while (!_gameOver)
        {

            if (ballTime % 3 == 0)
            {
                UpdateBall();
            }

            RemoveBlocks();
            HandleInput();
            DrawPaddle();
            DrawBall();
            DrawScore();
            DrawLives();

            Thread.Sleep(1);
            ballTime++;
        }
    }

    private void GenerateBlocks()
    {
        for (int y = 0; y < NumBlocksY; y++)
        {
            for (int x = 0; x < NumBlocksX; x++)
            {
                ConsoleColor color;

                do
                {
                    color = (ConsoleColor)_random.Next(1, 16);
                } while (HasAdjacentBlockWithSameColor(x, y, color));

                _blocks[x, y] = new Block
                {
                    X = x * BlockWidth,
                    Y = y * BlockHeight,
                    Color = color,
                    Destroyed = false
                };
            }
        }
    }
    private bool HasAdjacentBlockWithSameColor(int x, int y, ConsoleColor color)
    {
        if (x > 0 && _blocks[x - 1, y] != null && _blocks[x - 1, y].Destroyed == false && _blocks[x - 1, y].Color == color)
        {
            return true;
        }
        if (y > 0 && _blocks[x, y - 1] != null && _blocks[x, y - 1].Destroyed == false && _blocks[x, y - 1].Color == color)
        {
            return true;
        }

        return false;
    }
    private void RemoveBlocks()
    {
        for (int y = 0; y < NumBlocksY; y++)
        {
            for (int x = 0; x < NumBlocksX; x++)
            {
                if (_blocks[x, y] != null)
                {
                    Block block = _blocks[x, y];
                    bool blockDestroyed = block.Destroyed;

                    if (blockDestroyed)
                    {
                        for (int i = 0; i < BlockHeight; i++)
                        {
                            for (int j = 0; j < BlockWidth; j++)
                            {
                                int clearX = block.X + j;
                                int clearY = block.Y + i;

                                Console.SetCursorPosition(clearX, clearY);
                                Console.Write(" ");
                            }
                        }
                    }
                }
            }
        }
    }
    private void DrawBlocks()
    {
        for (int y = 0; y < NumBlocksY; y++)
        {
            for (int x = 0; x < NumBlocksX; x++)
            {
                if (_blocks[x, y] != null)
                {
                    Block block = _blocks[x, y];
                    bool blockDestroyed = block.Destroyed;

                    Console.ForegroundColor = block.Color;

                    for (int i = 0; i < BlockHeight; i++)
                    {
                        for (int j = 0; j < BlockWidth; j++)
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

    private void DrawPaddle()
    {
        Console.ForegroundColor = ConsoleColor.White;

        // Draw dashes before the paddle
        for (int x = 0; x < _paddleX; x++)
        {
            Console.SetCursorPosition(x, Height - PaddleHeight);
            Console.Write(" ");
        }

        // Draw the paddle
        for (int i = 0; i < PaddleHeight; i++)
        {
            for (int j = 0; j < PaddleWidth; j++)
            {
                int drawX = _paddleX + j;
                int drawY = Height - PaddleHeight + i;

                Console.SetCursorPosition(drawX, drawY);
                Console.Write("█");
            }
        }

        // Draw dashes after the paddle
        for (int x = _paddleX + PaddleWidth; x < Width; x++)
        {
            Console.SetCursorPosition(x, Height - PaddleHeight);
            Console.Write(" ");
        }
    }

    private void DrawBall()
    {
        Console.ForegroundColor = ConsoleColor.Red;

        for (int i = 0; i < BallSize; i++)
        {
            for (int j = 0; j < BallSize; j++)
            {
                int drawX = _ballX + j;
                int drawY = _ballY + i;
                int prevX = _ballX - _ballDx[_ballDxIndex] + j;
                int prevY = _ballY - _ballDy[_ballDyIndex] + i;

                Console.SetCursorPosition(prevX, prevY);
                Console.Write(" ");

                Console.SetCursorPosition(drawX, drawY);
                Console.Write("O");
            }
        }
    }

    private void DrawScore()
    {
        if (_previousScore != _score || _score == 0)
        {
            Console.SetCursorPosition(2, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Score: {_score}");
        }

    }

    private void DrawLives()
    {
        if (_previousLives != _lives || _lives == InitialLives)
        {
            Console.SetCursorPosition(Width - 10, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Lives: {_lives}");
        }
    }

    private void HandleInput()
    {
        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.LeftArrow && _paddleX > 0)
            {
                _keysPressed.Add(-1);
                _paddleX--;
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow && _paddleX + PaddleWidth < Width)
            {
                _keysPressed.Add(1);
                _paddleX++;
            }
        }
    }

    private void UpdateBall()
    {

        int dx = _ballDx[_ballDxIndex];
        int dy = _ballDy[_ballDyIndex];



        for (int y = 0; y < NumBlocksY; y++)
        {
            for (int x = 0; x < NumBlocksX; x++)
            {
                if (_blocks[x, y] != null && _blocks[x, y].Destroyed == false)
                {
                    if (_ballX >= _blocks[x, y].X && _ballX <= _blocks[x, y].X + BlockWidth &&
                        _ballY + BallSize >= _blocks[x, y].Y && _ballY <= _blocks[x, y].Y + BlockHeight)
                    {
                        _blocks[x, y].Destroyed = true;
                        _score++;

                        if (_score == NumBlocksX * NumBlocksY)
                        {
                            _gameOver = true;
                            Console.SetCursorPosition(Width / 2 - "You Win!".Length / 2, Height / 2);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("You Win!");
                            return;
                        }
                        if (CheckCollision(_blocks[x, y].X, _blocks[x, y].Y, BlockWidth, BlockHeight, ref dx, ref dy))
                        {
                            _ballDx[_ballDxIndex] = dx;
                            _ballDy[_ballDyIndex] = dy;
                        }
                    }
                }
            }
        }

        if (_ballX >= _paddleX && _ballX <= _paddleX + PaddleWidth &&
            _ballY + BallSize >= Height - PaddleHeight && _ballY < Height - PaddleHeight)
        {
            if (CheckCollision(_paddleX, Height - PaddleHeight, PaddleWidth, PaddleHeight, ref dx, ref dy))
            {
                _ballDx[_ballDxIndex] = dx;
                _ballDy[_ballDyIndex] = dy;
            }
        }



        if (_ballY + BallSize + dy >= Height)
        {
            _lives--;
            if (_lives == 0)
            {
                _gameOver = true;
                string[] _resultOver = new string[]
                {
                    "                                                      ",
                    "   _____                         ____                 ",
                    "  / ____|                       / __ \\                ",
                    " | |  __  __ _ _ __ ___   ___  | |  | |_   _____ _ __ ",
                    " | | |_ |/ _` | '_ ` _ \\ / _ \\ | |  | \\ \\ / / _ \\ '__|",
                    " | |__| | (_| | | | | | |  __/ | |__| |\\ V /  __/ |   ",
                    "  \\_____|\\__,_|_| |_| |_|\\___|  \\____/  \\_/ \\___|_|   ",
                    "                                                      ",
                    "  score:" + _score + "                                             ",
                    "                                                      "
                };


                for (int i = 0; i < _resultOver.Length; i++)
                {
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 27, Console.WindowHeight / 2 - 4 + i);
                    Console.WriteLine(_resultOver[i]);
                    Thread.Sleep(100);
                }
                Thread.Sleep(1000);
            }

            int drawX = _ballX;
            int drawY = _ballY;
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
            _ballX = _paddleX + PaddleWidth / 2;
            _ballY = Height - PaddleHeight - BallSize - 1;



        }


        if (_ballY == 0 && dy < 0)
        {
            dy = -dy; // reverse Y-axis velocity
            _ballDy[_ballDyIndex] = dy;
        }

        if (_ballX + dx < 0 || _ballX + BallSize + dx > Width)
        {
            _ballDxIndex = (_ballDxIndex + 1) % _ballDx.Length;
            dx = _ballDx[_ballDxIndex];


        }

        _ballX += dx;
        _ballY += dy;
    }

    private bool CheckCollision(int x, int y, int width, int height, ref int dx, ref int dy)
    {
        int ballCenterX = _ballX + BallSize / 2;
        int ballCenterY = _ballY + BallSize / 2;
        int blockCenterX = x + width / 2;
        int blockCenterY = y + height / 2;

        int deltaX = Math.Abs(ballCenterX - blockCenterX);
        int deltaY = Math.Abs(ballCenterY - blockCenterY);

        if (deltaX > (BallSize + width) / 2 || deltaY > (BallSize + height) / 2)
        {
            return false;
        }

        if (deltaX <= width / 2 || deltaY <= height / 2)
        {
            dy = -dy;
            return true;
        }

        int cornerDeltaX = deltaX - width / 2;
        int cornerDeltaY = deltaY - height / 2;
        int cornerDeltaXSquared = cornerDeltaX * cornerDeltaX;
        int cornerDeltaYSquared = cornerDeltaY * cornerDeltaY;
        int cornerDistanceSquared = cornerDeltaXSquared + cornerDeltaYSquared;
        if (cornerDistanceSquared <= (BallSize / 2) * (BallSize / 2))
        {
            dx = -dx;
            dy = -dy;
            return true;
        }

        return false;
    }
}