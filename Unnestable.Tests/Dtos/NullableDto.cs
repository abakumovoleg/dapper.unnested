using Dapper.Unnest.Generator;

namespace Unnestable.Tests.Dtos;

[Unnestable]
public class NullableDto
{
    public string RequiredString { get; set; } = null!; // Не-nullable
    public string? OptionalString { get; set; } // Nullable
}