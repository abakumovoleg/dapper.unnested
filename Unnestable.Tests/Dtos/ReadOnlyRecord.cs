using Dapper.Unnest.Generator;

namespace Unnestable.Tests.Dtos;

[Unnestable]
public record ReadOnlyRecord(int Id, string Computed)
{
    public string UpperComputed => Computed.ToUpperInvariant();
}