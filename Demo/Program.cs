using Dapper.Unnest.Generator;
using System.Collections.Generic;

namespace Demo;

[Unnestable]
public class A
{
    public int PropA { get; set; }
    public string PropB { get; set; } = string.Empty;
    
    public required int[] X { get; set; }
    
}

class Program
{
    static void Main()
    {
        var items = new List<A>
        {
            new A { PropA = 1, PropB = "one", X = new[] { 1, 2, 3 } },
            new A { PropA = 2, PropB = "two", X = new[] { 1, 2, 3 } },
        };

        var unnested = items.ToUnnested();

        Console.WriteLine("PropA: " + string.Join(", ", unnested.PropA));
        Console.WriteLine("PropB: " + string.Join(", ", unnested.PropB));
    }
}
