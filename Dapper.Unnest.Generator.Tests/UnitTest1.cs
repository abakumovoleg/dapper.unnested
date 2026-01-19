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

    [Fact]
    public void ToUnnested_WithVariousTypes()
    {
        var items = new[]
        {
            new TestItem2 { IntProp = 1, StringProp = "test", BoolProp = true, DoubleProp = 1.5, NullableInt = 10 },
            new TestItem2 { IntProp = 2, StringProp = "test2", BoolProp = false, DoubleProp = 2.5, NullableInt = null },
        };

        var result = items.ToUnnested();

        Assert.Equal(new[] { 1, 2 }, result.IntProp);
        Assert.Equal(new[] { "test", "test2" }, result.StringProp);
        Assert.Equal(new[] { true, false }, result.BoolProp);
        Assert.Equal(new[] { 1.5, 2.5 }, result.DoubleProp);
        Assert.Equal(new int?[] { 10, null }, result.NullableInt);
    }

    [Fact]
    public void ToUnnested_WithArrays()
    {
        var items = new[]
        {
            new TestItem3 { IntArray = new[] { 1, 2 }, StringArray = new[] { "a", "b" } },
            new TestItem3 { IntArray = new[] { 3, 4 }, StringArray = new[] { "c", "d" } },
        };

        var result = items.ToUnnested();

        Assert.Equal(new[] { new[] { 1, 2 }, new[] { 3, 4 } }, result.IntArray);
        Assert.Equal(new[] { new[] { "a", "b" }, new[] { "c", "d" } }, result.StringArray);
    }

    [Fact]
    public void ToUnnested_EmptyCollection()
    {
        var items = Array.Empty<TestItem>();

        var result = items.ToUnnested();

        Assert.Empty(result.PropA);
        Assert.Empty(result.PropB);
    }
}
