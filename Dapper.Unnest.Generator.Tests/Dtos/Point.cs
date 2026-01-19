namespace Dapper.Unnest.Generator.Tests.Dtos;

public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }
    
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
    
    public override bool Equals(object obj)
    {
        return obj is Point other && X == other.X && Y == other.Y;
    }
    
    public override int GetHashCode() => HashCode.Combine(X, Y);
}