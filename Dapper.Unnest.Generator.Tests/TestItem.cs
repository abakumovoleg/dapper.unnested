using Dapper.Unnest.Generator;

namespace Dapper.Unnest.Generator.Tests;

[Unnestable]
public class TestItem
{
    public int PropA { get; set; }
    public string PropB { get; set; }
}
