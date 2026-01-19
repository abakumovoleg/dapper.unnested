using Dapper.Unnest.Generator;

namespace Dapper.Unnest.Generator.Tests;

[Unnestable]
public class TestItem3
{
    public int[] IntArray { get; set; } = Array.Empty<int>();
    public string[] StringArray { get; set; } = Array.Empty<string>();
}
