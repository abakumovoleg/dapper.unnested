using Dapper.Unnest.Generator;

namespace Unnestable.Tests.Dtos;

[Unnestable]
public record MixedPropsRecord(int Id, string Name)
{
    public int AdditionalValue { get; init; }
}