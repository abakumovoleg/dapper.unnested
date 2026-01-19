namespace Dapper.Unnest.Generator.Tests.Dtos;

[Unnestable]
public record ReadOnlyRecord(int Id, string Computed)
{
    public string UpperComputed => Computed.ToUpperInvariant();
}