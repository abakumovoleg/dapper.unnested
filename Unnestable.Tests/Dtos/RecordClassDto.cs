using Dapper.Unnest.Generator;

namespace Unnestable.Tests.Dtos;

[Unnestable]
public record RecordClassDto(int Id, string Name, int Value);