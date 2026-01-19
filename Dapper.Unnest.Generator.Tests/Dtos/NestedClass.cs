namespace Dapper.Unnest.Generator.Tests.Dtos;

public class NestedClass
{
    public required string Value { get; init; }

    public override bool Equals(object? obj)
    {
        return obj is NestedClass other && Value == other.Value;
    }
    
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
}