namespace Dapper.Unnest.Generator.Tests.Dtos;

[Unnestable]
public class NullableDto
{
    public string RequiredString { get; set; } = null!; // Не-nullable
    public string? OptionalString { get; set; } // Nullable
}