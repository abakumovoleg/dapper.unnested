namespace Dapper.Unnest.Generator.Tests;

public class ExtensionTests
{
    [Fact]
    public void ToUnnested_ConvertsCollectionToUnnested()
    {
        var items = new[]
        {
            new TestItem { PropA = 1, PropB = "a" },
            new TestItem { PropA = 2, PropB = "b" },
        };

        var result = items.ToUnnested();

        Assert.Equal(new[] { 1, 2 }, result.PropA);
        Assert.Equal(new[] { "a", "b" }, result.PropB);
    }


}
