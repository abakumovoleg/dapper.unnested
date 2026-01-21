using Dapper.Unnest.Generator;

namespace Unnestable.Tests.Dtos;

[Unnestable]
public record RecordWithNullableProps(int Id, string? Name, int? Value);