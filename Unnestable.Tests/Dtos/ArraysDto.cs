using Dapper.Unnest.Generator;

namespace Unnestable.Tests.Dtos;

[Unnestable]
public class ArraysDto
{
    public int[] IntArray { get; set; } = Array.Empty<int>();
    public string[] StringArray { get; set; } = Array.Empty<string>();
}
