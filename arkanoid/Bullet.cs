using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bullet;
public class Bullet
{
    public int X { get; set; }
    public int Y { get; set; }
    public GunType GunType { get; set; }
    public int MaxBlocks { get;  set; }

    public Bullet(int x, int y, GunType gunType, int maxBlocks)
    {
        X = x;
        Y = y;
        GunType = gunType;
        MaxBlocks = maxBlocks;
    }

    public void MoveUp()
    {
        Y--;
    }

    public void Draw()
    {
        Console.SetCursorPosition(X, Y);
        Console.Write("^");
        Console.SetCursorPosition(X, Y + 1);
        Console.Write(" ");

    }
}
public enum GunType
{
    Glock,
    Sniper,
    AK47
}

