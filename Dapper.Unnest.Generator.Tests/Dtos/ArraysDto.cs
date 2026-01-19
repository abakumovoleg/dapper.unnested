namespace Dapper.Unnest.Generator.Tests.Dtos;

[Unnestable]
public class ArraysDto
{
    public int[] IntArray { get; set; } = Array.Empty<int>();
    public string[] StringArray { get; set; } = Array.Empty<string>();
}
