using Dapper.Unnest.Generator;

namespace Unnestable.Tests.Dtos;

[Unnestable]
public class StructDto
{
    public Point Point { get; set; }
    public TimeSpan TimeSpan { get; set; }
}