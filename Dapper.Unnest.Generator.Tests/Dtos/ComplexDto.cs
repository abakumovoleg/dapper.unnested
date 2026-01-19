namespace Dapper.Unnest.Generator.Tests.Dtos;

[Unnestable]
public class ComplexDto
{
    public DateTime DateTimeProp { get; set; }
    public Guid GuidProp { get; set; }
    public NestedClass Nested { get; set; }
}