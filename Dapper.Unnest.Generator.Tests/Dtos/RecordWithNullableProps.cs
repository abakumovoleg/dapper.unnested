namespace Dapper.Unnest.Generator.Tests.Dtos;

[Unnestable]
public record RecordWithNullableProps(int Id, string? Name, int? Value);