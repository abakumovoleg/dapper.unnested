using Dapper.Unnest.Generator;

namespace Unnestable.Tests.Dtos;

[Unnestable]
public class ComplexDto
{
    public DateTime DateTimeProp { get; set; }
    public Guid GuidProp { get; set; }
    public required NestedClass Nested { get; set; }
}