namespace Dapper.Unnest.Generator.Tests.Dtos;

[Unnestable]
public class SimplePropsDto
{
    public int IntProp { get; set; }
    public string? StringProp { get; set; }
    public bool BoolProp { get; set; }
    public double DoubleProp { get; set; }
    public int? NullableInt { get; set; }
}
