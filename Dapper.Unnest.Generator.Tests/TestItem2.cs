using Dapper.Unnest.Generator;

namespace Dapper.Unnest.Generator.Tests;

[Unnestable]
public class TestItem2
{
    public int IntProp { get; set; }
    public string StringProp { get; set; } = string.Empty;
    public bool BoolProp { get; set; }
    public double DoubleProp { get; set; }
    public int? NullableInt { get; set; }
}
