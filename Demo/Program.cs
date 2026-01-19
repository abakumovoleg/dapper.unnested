using System.Collections.Generic;
using Dapper.Unnest;

namespace Demo;

[Unnestable]
public class A
{
    public int PropA { get; set; }
    public string PropB { get; set; } = string.Empty;
    
    public required int[] X { get; set; }
    
}

class Program
{
    static void Main()
    {
        var items = new List<A>
        {
            new A { PropA = 1, PropB = "one", X = new[] { 1, 2, 3 } },
            new A { PropA = 2, PropB = "two", X = new[] { 1, 2, 3 } },
        };

        var unnested = items.ToUnnested();

        Console.WriteLine("PropA: " + string.Join(", ", unnested.PropA));
        Console.WriteLine("PropB: " + string.Join(", ", unnested.PropB));


        var i = new List<ExecmplarCompensationAddDb>().ToArray();
        var r = i.ToUnnested();
    }
}



[Unnestable]
internal sealed record ExecmplarCompensationAddDb
{
    public long ExemplarIds { get; init; }
    public long ItemIds { get; init; }
    public long MetazonSellerIds { get; init; }
    public long MarketplaceSellerIds { get; init; }
    public double CompensationPercents { get; init; }
    public DateOnly CompensationDates { get; init; }
    public long MetazonSourceDocumentIds { get; init; }
    public long MetazonSourceDocumentTypeIds { get; init; }
    public long CostCenterIds { get; init; }
    public required string CompensationReasons { get; init; }
    public required byte[] SystemOriginators { get; init; }
    public decimal BaseCosts { get; init; }
    public decimal WithheldAmounts { get; init; }
    public string? CurrencyCodes { get; init; }
    public required string States { get; init; }
    public long CompensationDocumentIds { get; init; }
    public int TransientVersions { get; init; }
    public required int? ErrorCodes { get; init; }
    public string? ErrorMessages { get; init; }
    public required long EventExemplarIds { get; init; }
    public required string EventNames { get; init; }
    public required int EventVersions { get; init; }
    public DateTimeOffset EventCreatedDates { get; init; }
    public string? EventData { get; init; }
    public string? PublicationErrorCodes { get; init; }
    public string? PublicationErrorMessages { get; init; }
    public string? PostingNumbers { get; init; }
    public long? CompensationReasonIds { get; init; }
    public long PreviousCompensationIds { get; init; }
    public required string CompensationSourceTypes { get; init; }
}