using block;
using bullet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace arkanoid;
public class ArkanoidGame
{
    public const int Width = 120;
    public const int Height = 48;
    private const int NumBlocksX = 10;
    private const int BlockWidth = Width / NumBlocksX;
    private const int BlockHeight = 2;
    private const int NumBlocksY = 16;
    private const int PaddleWidth = 18;
    private const int PaddleHeight = 1;
    private const int BallSize = 1;
    private const int InitialLives = 1;

    /*
     Welcome!
     Welcome to my Arkanoid game!
     Your goal is to destroy all the blocks on the screen by hitting them with the ball.
        You have score that increases when you destroy a block. Your score will be reset after each level.
     You control the paddle at the bottom of the screen. You can move it left and right using the arrow keys (<- , ->). 
        In case you miss the ball and the ball leaves playing field, you lose a life.
     
     You can also shoot bullets by pressing the space bar. 
     The bullets will destroy the blocks they hit. 
     You can change the type of the gun by pressing the number keys 1, 2 and 3. 
     The gun types are:
        Glock - one bullet can destroy 1 block and has a reload time of 1 second.
        Sniper - one bullet can destroy 2 blocks and has a reload time of 2 seconds.
        AK47 - one bullet can destroy 1 block and has a reload time of 3 seconds.
     */

    private readonly Random _random = new Random();
    private readonly Block[,] _blocks = new Block[NumBlocksX, NumBlocksY];
    private readonly List<int> _keysPressed = new List<int>();
    private bool _ballDirToRight = true;
    private bool _ballDirToTop = true;

    private int _score = 0;
    private int _lives = InitialLives;
    private int _ballX = Width / 2;
    private int _ballY = Height - PaddleHeight - BallSize - 1;
    private int _paddleX = Width / 2 - PaddleWidth / 2;
    private bool _gameOver = false;

    private const int NumLevels = 3;
    private int _currentLevel = 1;
    private int _autosave = 1;

    private bool _paused = false;

    private DateTime _lastBulletTime;
    private bool _canShootBullet = true;
    private List<Bullet> _bullets = new List<Bullet>();

    private GunType _currentGunType = GunType.Glock;
    private int _currentGunMaxBlocks = 1;
    private TimeSpan _currentGunReloadTime = TimeSpan.FromSeconds(1);


    public ArkanoidGame()
    {

        Console.CursorVisible = false;
        Console.SetWindowSize(Width, Height);
        Console.SetBufferSize(Width, Height);

        Console.Clear();
        StartNewLevel();
    }

    public void Run()
    {

        int ballTime = 0;
        int bulletTime = 0;
        while (!_gameOver)
        {
            if (!_paused)
            {
                if (ballTime % 3 == 0)
                {
                    UpdateBall(_blocks);
                }
                if (bulletTime % 5 == 0)
                {
                    UpdateBullets();
                }

                DrawBullets();
                DrawHud();
                RemoveBlocks(_blocks);
                HandleInput();
                DrawPaddleAndBall();

                if (IsLevelCompleted())
                {

                    GameFinish(true);
                    _currentLevel++;
                    if (_currentLevel >= NumLevels-1)
                    {
                        _gameOver = true;
                        break;
                    }

                    StartNewLevel();
                }


                Thread.Sleep(1);
                ballTime++;
                bulletTime++;
            }
        }
    }

    public void StartNewGame()
    {
        GenerateBlocks(_blocks, GetBlockPatternForLevel(1));
    }
    private void StartNewLevel()
    {
        _currentGunType = GunType.Glock;
        _lives = InitialLives;
        _score = 0;
        Countdown();
        GenerateBlocks(_blocks, GetBlockPatternForLevel(_currentLevel));
        DrawBlocks(_blocks);
        _paddleX = Width / 2 - PaddleWidth / 2;
        InitializeBall();
    }

    private int[,] GetBlockPatternForLevel(int level)
    {
        switch (level)
        {
            case 1:
                return new int[,]
                {
                    { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
                    { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
                    { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
                    { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
                    { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                };
            case 2:
                return new int[,]
                {
                    { 1, 0, 0, 0, 1, 1, 0, 0, 0, 1 },
                    { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1 },
                    { 0, 0, 0, 1, 0, 0, 1, 0, 0, 0 },
                    { 0, 0, 1, 0, 1, 1, 0, 1, 0, 0 },
                    { 0, 0, 1, 1, 0, 0, 1, 1, 0, 0 },
                    { 0, 1, 0, 1, 0, 0, 1, 0, 1, 0 },
                    { 0, 1, 0, 1, 0, 0, 1, 0, 1, 0 },
                    { 0, 1, 0, 1, 0, 0, 1, 0, 1, 0 },
                    { 0, 1, 0, 1, 0, 0, 1, 0, 1, 0 },
                    { 0, 0, 1, 1, 0, 0, 1, 1, 0, 0 },
                    { 0, 0, 1, 0, 1, 1, 0, 1, 0, 0 },
                    { 0, 0, 0, 1, 0, 0, 1, 0, 0, 0 },
                    { 0, 0, 0, 1, 0, 0, 1, 0, 0, 0 },
                    { 0, 0, 0, 0, 1, 1, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 1, 1, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                };
            case 3:
                return new int[,]
                {
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                };
            default:
                return new int[NumBlocksY, NumBlocksX];
        }
    }
    private void GenerateBlocks(Block[,] map, int[,] blockPattern)
    {
        for (int y = 0; y < NumBlocksY; y++)
        {
            for (int x = 0; x < NumBlocksX; x++)
            {
                ConsoleColor color;

                do
                {
                    color = (ConsoleColor)_random.Next(1, 16);
                } while (HasAdjacentBlockWithSameColor(x, y, color, map));

                bool isBlockPresent = blockPattern[y, x] == 1;
                map[x, y] = new Block
                {
                    X = x * BlockWidth,
                    Y = y * BlockHeight,
                    Color = isBlockPresent ? color : ConsoleColor.Black,
                    Destroyed = !isBlockPresent
                };
            }
        }
    }


    private bool HasAdjacentBlockWithSameColor(int x, int y, ConsoleColor color, Block[,] map)
    {
        if (x > 0 && map[x - 1, y] != null && map[x - 1, y].Color == color)
            return true;
        if (x < NumBlocksX - 1 && map[x + 1, y] != null && map[x + 1, y].Color == color)
            return true;
        if (y > 0 && map[x, y - 1] != null && map[x, y - 1].Color == color)
            return true;
        if (y < NumBlocksY - 1 && map[x, y + 1] != null && map[x, y + 1].Color == color)
            return true;
        return false;
    }
    private void RemoveBlocks(Block[,] map)
    {
        for (int y = 0; y < NumBlocksY; y++)
        {
            for (int x = 0; x < NumBlocksX; x++)
            {
                if (map[x, y] != null)
                {
                    Block block = map[x, y];
                    bool blockDestroyed = block.Destroyed;

                    if (blockDestroyed && !block.IsRemoved)
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

                        block.IsRemoved = true;
                    }
                }
            }
        }
    }
    private void DrawBlocks(Block[,] map)
    {
        for (int y = 0; y < NumBlocksY; y++)
        {
            for (int x = 0; x < NumBlocksX; x++)
            {
                if (map[x, y] != null)
                {
                    Block block = map[x, y];
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
    private void DrawPaddleAndBall()
    {
        Console.ForegroundColor = ConsoleColor.White;

        for (int x = 0; x < _paddleX; x++)
        {
            Console.SetCursorPosition(x, Height - PaddleHeight);
            Console.Write(" ");
        }

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

        for (int x = _paddleX + PaddleWidth; x < Width; x++)
        {
            Console.SetCursorPosition(x, Height - PaddleHeight);
            Console.Write(" ");
        }

        Console.ForegroundColor = ConsoleColor.Red;

        for (int i = 0; i < BallSize; i++)
        {
            for (int j = 0; j < BallSize; j++)
            {
                int drawX = _ballX + j;
                int drawY = _ballY + i;

                Console.SetCursorPosition(drawX, drawY);
                Console.Write("O");
            }
        }
    }
    private void DrawHud()
    {        
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(2, 0);
        Console.Write($"Score: {_score}");
        Console.SetCursorPosition(Width - 10, 0);
        Console.Write($"Lives: {_lives}");
    }
    private void InitializeBall()
    {
        _ballX = _paddleX + PaddleWidth / 2;
        _ballY = Height - PaddleHeight - BallSize - 1;
        _ballDirToTop = true;
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
            else if (keyInfo.Key == ConsoleKey.S)
            {
                SaveGame(_autosave);
            }
            else if (keyInfo.Key == ConsoleKey.C)
            {
                _currentLevel++;
                GenerateBlocks(_blocks, GetBlockPatternForLevel(_currentLevel));
                DrawBlocks(_blocks);
                _paddleX = Width / 2 - PaddleWidth / 2;
                InitializeBall();
            }
            else if (keyInfo.Key == ConsoleKey.Escape)
            {
                if (!_paused)
                {
                    _paused = true;
                    ShowPauseMenu();
                }
            }

            else if (keyInfo.Key == ConsoleKey.Spacebar)
            {
                ShootBullet();
            }
            else if (keyInfo.Key == ConsoleKey.NumPad2)
            {
                SwitchGunType(GunType.Sniper);
            }
            else if (keyInfo.Key == ConsoleKey.NumPad1)
            {
                SwitchGunType(GunType.Glock);
            }
            else if (keyInfo.Key == ConsoleKey.NumPad3)
            {
                SwitchGunType(GunType.AK47);
            }
        }
    }
    private void UpdateBall(Block[,] map)
    {
        Console.SetCursorPosition(_ballX, _ballY);
        Console.Write(" ");

        int newBallX = _ballX + (_ballDirToRight ? 1 : -1);
        int newBallY = _ballY + (_ballDirToTop ? -1 : 1);

        if (newBallY == Height - PaddleHeight - BallSize && newBallX >= _paddleX && newBallX < _paddleX + PaddleWidth)
        {
            _ballDirToTop = true;
        }

        if (newBallY >= 0 && newBallY < NumBlocksY * BlockHeight)
        {
            Block block = map[newBallX / BlockWidth, newBallY / BlockHeight];
            if (block != null && !block.Destroyed)
            {
                block.Destroyed = true;
                _score++;


                if (newBallX % BlockWidth == 0 || newBallX % BlockWidth == BlockWidth - 1)
                {
                    _ballDirToRight = !_ballDirToRight;
                }
                else
                {
                    _ballDirToTop = !_ballDirToTop;
                }
            }
        }

        if (newBallX == 0 || newBallX == Width - 1)
        {
            _ballDirToRight = !_ballDirToRight;
        }
        if (newBallY == 0)
        {
            _ballDirToTop = !_ballDirToTop;
        }

        if (newBallY >= Height-1)
        {
            _lives--;
            if (_lives == 0)
            {
                GameFinish(false);
                
                Console.ForegroundColor= ConsoleColor.Red;
                Console.SetCursorPosition(46, 24);
                Console.Write("Replay level again?... (Y/N)");
                Console.SetCursorPosition(60, 25);
                string output = Console.ReadLine();
                if (output.Trim() == "Y" || output.Trim() == "y")
                {
                    StartNewLevel();
                }
                else
                {
                    _gameOver = true;
                }
                
            }
            else
            {
                BallFall(_ballX, _ballY);
            }
            InitializeBall();
        }
        else
        {
            _ballX = newBallX;
            _ballY = newBallY;
        }





    }

    private void GameFinish(bool win)
    {
        RemoveBlocks(_blocks);
        DrawHud();
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
            BallFall(_ballX, _ballY);

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
                "  score:" + _score + "                                             ",
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
    private void BallFall(int drawX, int drawY)
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
    private void Countdown()
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

            Thread.Sleep(100);
            Console.Clear();

        }
    }
    private bool IsLevelCompleted()
    {
        for (int y = 0; y < NumBlocksY; y++)
        {
            for (int x = 0; x < NumBlocksX; x++)
            {
                if (_blocks[x, y] != null && !_blocks[x, y].Destroyed)
                {
                    return false;
                }
            }
        }

        return true;
    }


    public void SaveGame(int save)
    {
        SaveData saveData = new SaveData
        {
            LastBulletTime = _lastBulletTime,
            CanShootBullet = _canShootBullet,
            Bullets = _bullets,
            CurrentGunType = _currentGunType,
            CurrentGunMaxBlocks = _currentGunMaxBlocks,
            CurrentGunReloadTime = _currentGunReloadTime,

            Score = _score,
            CurrentLevel = _currentLevel,
            Lives = _lives,
            BallX = _ballX,
            BallY = _ballY,
            BallDirToRight = _ballDirToRight,
            BallDirToTop = _ballDirToTop,
            PaddleX = _paddleX,
            Blocks = _blocks
        };
        string savepath = "";
        switch (save)
        {
            case 0:
                savepath = "save_0.sav";
                break;
            case 1:
                savepath = "save_1.sav";
                break;
            case 2:
                savepath = "save_2.sav";
                break;
            default:
                break;
        }
        using (StreamWriter writer = new StreamWriter(savepath))
        {
            writer.WriteLine(saveData.LastBulletTime);
            writer.WriteLine(saveData.CanShootBullet);
            writer.WriteLine(saveData.CurrentGunType);
            writer.WriteLine(saveData.CurrentGunMaxBlocks);
            writer.WriteLine(saveData.CurrentGunReloadTime);

            writer.WriteLine(saveData.Score);
            writer.WriteLine(saveData.CurrentLevel);
            writer.WriteLine(saveData.Lives);
            writer.WriteLine(saveData.BallX);
            writer.WriteLine(saveData.BallY);
            writer.WriteLine(saveData.BallDirToRight);
            writer.WriteLine(saveData.BallDirToTop);
            writer.WriteLine(saveData.PaddleX);

            foreach (Bullet bullet in saveData.Bullets)
            {
                writer.Write($"{bullet.X},{bullet.Y},{bullet.GunType},{bullet.MaxBlocks}|");
            }
            writer.WriteLine();
            for (int y = 0; y < NumBlocksY; y++)
            {
                for (int x = 0; x < NumBlocksX; x++)
                {
                    Block block = saveData.Blocks[x, y];
                    writer.WriteLine(block != null ? $"{block.X},{block.Y},{(int)block.Color},{block.Destroyed}" : string.Empty);
                }
            }
        }
    }
    public void LoadGame(string loadpath)
    {
        using (StreamReader reader = new StreamReader(loadpath))
        {

            _lastBulletTime = DateTime.Parse(reader.ReadLine());
            _canShootBullet = bool.Parse(reader.ReadLine());
            _currentGunType = (GunType)Enum.Parse(typeof(GunType), reader.ReadLine());
            _currentGunMaxBlocks = int.Parse(reader.ReadLine());
            _currentGunReloadTime = TimeSpan.Parse(reader.ReadLine());

            _score = int.Parse(reader.ReadLine());
            _currentLevel= int.Parse(reader.ReadLine());
            _lives = int.Parse(reader.ReadLine());
            _ballX = int.Parse(reader.ReadLine());
            _ballY = int.Parse(reader.ReadLine());
            _ballDirToRight = bool.Parse(reader.ReadLine());
            _ballDirToTop = bool.Parse(reader.ReadLine());
            _paddleX = int.Parse(reader.ReadLine());

            _bullets = new List<Bullet>();
            foreach (string bulletString in reader.ReadLine().Split('|'))
            {
                if (bulletString != string.Empty)
                {
                    string[] bulletParams = bulletString.Split(',');
                    int x = int.Parse(bulletParams[0]);
                    int y = int.Parse(bulletParams[1]);
                    GunType gunType = (GunType)Enum.Parse(typeof(GunType), bulletParams[2]);
                    int maxBlocks = int.Parse(bulletParams[3]);
                    Bullet bullet = new Bullet(x, y, gunType, maxBlocks);
                    _bullets.Add(bullet);
                }
            }
            for (int y = 0; y < NumBlocksY; y++)
            {
                for (int x = 0; x < NumBlocksX; x++)
                {
                    string[] blockData = reader.ReadLine().Split(',');
                    if (blockData.Length == 4)
                    {
                        int blockX = int.Parse(blockData[0]);
                        int blockY = int.Parse(blockData[1]);
                        ConsoleColor blockColor = (ConsoleColor)int.Parse(blockData[2]);
                        bool blockDestroyed = bool.Parse(blockData[3]);

                        _blocks[x, y] = new Block
                        {
                            X = blockX,
                            Y = blockY,
                            Color = blockColor,
                            Destroyed = blockDestroyed
                        };
                    }
                }
            }
        }
        DrawBlocks(_blocks);
    }
    private struct SaveData
    {
        public DateTime LastBulletTime;
        public bool CanShootBullet;
        public List<Bullet> Bullets;
        public GunType CurrentGunType;
        public int CurrentGunMaxBlocks;
        public TimeSpan CurrentGunReloadTime;

        public int Score;
        public int CurrentLevel;
        public int Lives;
        public int BallX;
        public int BallY;
        public bool BallDirToRight;
        public bool BallDirToTop;
        public int PaddleX;
        public Block[,] Blocks;
    }

    private void ShowPauseMenu()
    {

        bool validChoice = false;
        while (!validChoice)
        {
            Console.Clear();
            Console.ForegroundColor= ConsoleColor.White;
            Console.SetCursorPosition(0, 0);
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
            Console.ForegroundColor= ConsoleColor.Green;
            Console.SetCursorPosition(49, 42);
            Console.Write("Your choice (1-4): ");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.Clear();
                    DrawBlocks(_blocks);
                    validChoice = true;
                    break;

                case "2":
                    StartNewLevel();
                    validChoice = true;
                    break;

                case "3":
                    while (true)
                    {
                        Console.Clear();
                        Console.ForegroundColor= ConsoleColor.Red;
                        Console.Write("<< Back (B/b)");
                        Console.ForegroundColor= ConsoleColor.White;
                        Console.Write(
                            "                 |    _____                    _____                        |\n" +
                            "                              |   / ____|                  / ____|                       |\n" +
                            "                              |  | (___   __ ___   _____  | |  __  __ _ _ __ ___   ___   |\n" +
                            "                              |   \\___ \\ / _` \\ \\ / / _ \\ | | |_ |/ _` | '_ ` _ \\ / _ \\  |\n" +
                            "                              |   ____) | (_| |\\ V /  __/ | |__| | (_| | | | | | |  __/  |\n" +
                            "                              |  |_____/ \\__,_| \\_/ \\___|  \\_____|\\__,_|_| |_| |_|\\___|  |\n" +
                            "                              |__________________________________________________________|\n\n\n\n\n"
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
                                            string isAutosave = "";
                                            if (i == _autosave-1)
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
                                               $"                                           #            Score: {score}           #\n"+ isAutosave +
                                                "                                           #                               #\n" +
                                                "                                           #################################\n\n\n");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                string isAutosave = "";
                                if (i == _autosave-1)
                                {
                                    isAutosave = "                                           #     Auto Save / Fast Save     #\n";
                                }
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(
                                "                                           #################################\n" +
                                "                                           #                               #\n" +
                               $"                                           #             Save {i+1}            #\n" +
                                "                                           #                               #\n" +
                                "                                           #      File does not exist      #\n" + isAutosave +
                                "                                           #                               #\n" +
                                "                                           #################################\n\n\n");
                            }
                            
                        }
        
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.SetCursorPosition(45, 42);
                        Console.Write("Choose spot to save (1-3): ");
                        string saveIndexStr = Console.ReadLine();
                        if (int.TryParse(saveIndexStr, out int saveIndex) && saveIndex >= 1 && saveIndex <= 3)
                        {
                            string saveFileName = $"save_{saveIndex - 1}.sav";
                            Console.ForegroundColor = ConsoleColor.Red;
                            if (File.Exists(saveFileName))
                            {
                                Console.Write($"\n                          Save already exists in slot {saveIndex}. Do you want to overwrite? (y/n) : ");
                                string overwriteChoiceInput = Console.ReadLine().ToLower();
                                if (overwriteChoiceInput == "y")
                                {
                                    SaveGame(saveIndex - 1);
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write($"\n                                             Save overwritten in slot {saveIndex}.");
                                    Thread.Sleep(250);
                                    break;
                                }
                                else
                                {
                                    Console.Write("\n                                             Save operation cancelled.");
                                    Thread.Sleep(250);
                                }
                            }
                            else
                            {
                                SaveGame(saveIndex - 1);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write($"\n                                           Game saved in slot {saveIndex}.");
                                Thread.Sleep(250);
                                break;

                            }
                        }

                        else if (saveIndexStr == "B" || saveIndexStr == "b")
                        {
                            break;
                        }
                        else
                        {
                            Console.ForegroundColor= ConsoleColor.Red;
                            Console.WriteLine("\n                                                Invalid save index.");
                            Thread.Sleep(250);
                        }
                    }
                    break;

                case "4":
                    _gameOver = true;
                    validChoice = true;
                    break;

                default:
                    Console.SetCursorPosition(43,45);
                    Console.ForegroundColor= ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Please try again.");
                    Thread.Sleep(250);
                    break;
            }
        }
        _paused= false;
    }

    private void ShootBullet()
    {
        if (DateTime.Now - _lastBulletTime >= _currentGunReloadTime)
        {
            _lastBulletTime = DateTime.Now;
            _canShootBullet = false;

            Bullet bullet = new Bullet(_paddleX + PaddleWidth / 2, Console.WindowHeight - 2, _currentGunType, _currentGunMaxBlocks);
            _bullets.Add(bullet);

        }
    }
    private void UpdateBullets()
    {
    
        for (int i = _bullets.Count - 1; i >= 0; i--)
        {
            Bullet bullet = _bullets[i];
            bullet.MoveUp();

            if (bullet.Y < 0)
            {
                _bullets.RemoveAt(i);
                Console.SetCursorPosition(bullet.X, bullet.Y + 1);
                Console.Write(" ");
                _canShootBullet = true;
                continue;
            }

            int blockX = bullet.X / BlockWidth;
            int blockY = bullet.Y / BlockHeight;

            if (blockY >= 0 && blockY < NumBlocksY && blockX >= 0 && blockX < NumBlocksX)
            {
                Block block = _blocks[blockX, blockY];
                if (block != null && !block.Destroyed)
                {
                    block.Destroyed = true;
                    bullet.MaxBlocks -= 1;
                    _score++;
                    if (bullet.MaxBlocks <= 0)
                    {
                        Console.SetCursorPosition(bullet.X, bullet.Y + 1);
                        Console.Write(" ");
                        _bullets.RemoveAt(i);
                    }
                    

                }
            }
        }
    
    }
    private void DrawBullets()
    {
        foreach (Bullet bullet in _bullets)
        {
            if (bullet.GunType == GunType.Glock)
            {
                Console.ForegroundColor= ConsoleColor.Green;
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

        double countdown = Math.Max(0, _currentGunReloadTime.TotalSeconds - (DateTime.Now - _lastBulletTime).TotalSeconds);

        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(Console.WindowWidth - 7 - _currentGunType.ToString().Length, Console.WindowHeight - 2);
        Console.Write("  " + _currentGunType + " " + countdown.ToString("F1"));
    }
    private void SwitchGunType(GunType gunType)
    {
        _currentGunType = gunType;

        switch (_currentGunType)
        {
            case GunType.Glock:
                _currentGunMaxBlocks = 1;
                _currentGunReloadTime = TimeSpan.FromSeconds(1);
                break;

            case GunType.Sniper:
                _currentGunMaxBlocks = 2;
                _currentGunReloadTime = TimeSpan.FromSeconds(3);
                break;

            case GunType.AK47:
                _currentGunMaxBlocks = 1;
                _currentGunReloadTime = TimeSpan.FromSeconds(0.2);
                break;
        }
    }
}