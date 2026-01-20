using Dapper.Unnest.Generator.Tests.Dtos;
using FluentAssertions;

namespace Dapper.Unnest.Generator.Tests;

public class ExtensionTests
{
    [Fact]
    public void ToUnnestable_WithVariousTypes()
    {
        // Arrange
        var items = new[]
        {
            new SimplePropsDto { IntProp = 1, StringProp = "test", BoolProp = true, DoubleProp = 1.5, NullableInt = 10 },
            new SimplePropsDto { IntProp = 2, StringProp = "test2", BoolProp = false, DoubleProp = 2.5, NullableInt = null },
        };

        // Act
        var result = items.ToUnnestable();

        // Assert
        result.IntProp.Should().Equal(1, 2);
        result.StringProp.Should().Equal("test", "test2");
        result.BoolProp.Should().Equal(true, false);
        result.DoubleProp.Should().Equal(1.5, 2.5);
        result.NullableInt.Should().Equal(10, null);
    }

    [Fact]
    public void ToUnnestable_WithArrays()
    {
        // Arrange
        var items = new[]
        {
            new ArraysDto { IntArray = new[] { 1, 2 }, StringArray = new[] { "a", "b" } },
            new ArraysDto { IntArray = new[] { 3, 4 }, StringArray = new[] { "c", "d" } },
        };

        // Act
        var result = items.ToUnnestable();

        // Assert
        result.IntArray.Should().BeEquivalentTo(new[] { new[] { 1, 2 }, new[] { 3, 4 } });
        result.StringArray.Should().BeEquivalentTo(new[] { new[] { "a", "b" }, new[] { "c", "d" } });
    }

    [Fact]
    public void ToUnnestable_EmptyCollection_ReturnsEmptyArrays()
    {
        // Arrange
        var items = Array.Empty<SimplePropsDto>();

        // Act
        var result = items.ToUnnestable();

        // Assert
        result.IntProp.Should().BeEmpty();
        result.StringProp.Should().BeEmpty();
        result.IntProp.Should().NotBeNull();
        result.StringProp.Should().NotBeNull();
    }

    [Fact]
    public void ToUnnestable_SingleItem()
    {
        // Arrange
        var items = new[]
        {
            new SimplePropsDto { IntProp = 42, StringProp = "single" }
        };

        // Act
        var result = items.ToUnnestable();

        // Assert
        result.IntProp.Should().ContainSingle().Which.Should().Be(42);
        result.StringProp.Should().ContainSingle().Which.Should().Be("single");
    }
    
    [Fact]
    public void ToUnnestable_LargeCollection()
    {
        // Arrange
        const int count = 10000;
        var items = Enumerable.Range(1, count)
            .Select(i => new SimplePropsDto
            {
                IntProp = i,
                StringProp = $"Item_{i}"
            })
            .ToList();

        // Act
        var result = items.ToUnnestable();

        // Assert
        result.IntProp.Should().HaveCount(count);
        result.StringProp.Should().HaveCount(count);
        result.StringProp[4999].Should().Be("Item_5000");
    }

    [Fact]
    public void ToUnnestable_WithNullValues()
    {
        // Arrange
        var items = new[]
        {
            new SimplePropsDto { StringProp = null },
            new SimplePropsDto { StringProp = "not null" }
        };

        // Act
        var result = items.ToUnnestable();

        // Assert
        result.StringProp[0].Should().BeNull();
        result.StringProp[1].Should().Be("not null");
    }

    [Fact]
    public void ToUnnestable_FromList()
    {
        // Arrange
        var items = new List<SimplePropsDto>
        {
            new() { IntProp = 1 },
            new() { IntProp = 2 }
        };

        // Act
        var result = items.ToUnnestable();

        // Assert
        result.IntProp.Should().HaveCount(2);
    }

    [Fact]
    public void ToUnnestable_FromReadOnlyCollection()
    {
        // Arrange
        IReadOnlyCollection<SimplePropsDto> items = new List<SimplePropsDto>
        {
            new() { IntProp = 1 },
            new() { IntProp = 2 }
        }.AsReadOnly();

        // Act
        var result = items.ToUnnestable();

        // Assert
        result.IntProp.Should().HaveCount(2);
    }

    [Fact]
    public void ToUnnestable_FromHashSet()
    {
        // Arrange
        var items = new HashSet<SimplePropsDto>
        {
            new() { IntProp = 1, StringProp = "a" },
            new() { IntProp = 2, StringProp = "b" }
        };

        // Act
        var result = items.ToUnnestable(items.Count);

        // Assert
        result.IntProp.Should().HaveCount(2);
    }

    [Fact]
    public void ToUnnestable_WithComplexTypes()
    {
        // Arrange
        var items = new[]
        {
            new ComplexDto
            {
                DateTimeProp = new DateTime(2024, 1, 1),
                GuidProp = Guid.NewGuid(),
                Nested = new NestedClass { Value = "test" }
            },
            new ComplexDto
            {
                DateTimeProp = new DateTime(2024, 1, 2),
                GuidProp = Guid.NewGuid(),
                Nested = new NestedClass { Value = "test2" }
            }
        };

        // Act
        var result = items.ToUnnestable();

        // Assert
        result.DateTimeProp[0].Should().Be(items[0].DateTimeProp);
        result.GuidProp[0].Should().Be(items[0].GuidProp);
        result.Nested[0].Should().Be(items[0].Nested);
    }

    [Fact]
    public void ToUnnestable_WithValueTypes()
    {
        // Arrange
        var items = new[]
        {
            new StructDto
            {
                Point = new Point(1, 2),
                TimeSpan = TimeSpan.FromHours(1)
            },
            new StructDto
            {
                Point = new Point(3, 4),
                TimeSpan = TimeSpan.FromHours(2)
            }
        };

        // Act
        var result = items.ToUnnestable();

        // Assert
        result.Point[0].Should().Be(new Point(1, 2));
        result.TimeSpan[0].Should().Be(TimeSpan.FromHours(1));
    }

    [Fact]
    public void ToUnnestable_IgnoresNonPublicProperties()
    {
        // Arrange
        var items = new[]
        {
            new WithPrivateProps { PublicProp = 1 },
            new WithPrivateProps { PublicProp = 2 }
        };

        // Act
        var result = items.ToUnnestable();

        // Assert
        result.PublicProp.Should().HaveCount(2).And.ContainInOrder(1, 2);
    }

    [Fact]
    public void ToUnnestable_WithIndexers()
    {
        // Arrange
        var items = new[]
        {
            new WithIndexer { Value = "a" },
            new WithIndexer { Value = "b" }
        };

        // Act
        var result = items.ToUnnestable();

        // Assert
        result.Value.Should().HaveCount(2).And.ContainInOrder("a", "b");
    }

    [Fact]
    public void ToUnnestable_WithNullableReferenceTypes()
    {
        // Arrange
        var items = new[]
        {
            new NullableDto {
                RequiredString = "required",
                OptionalString = null
            },
            new NullableDto {
                RequiredString = "required2",
                OptionalString = "optional"
            }
        };

        // Act
        var result = items.ToUnnestable();

        // Assert
        result.RequiredString.Should().Equal("required", "required2");
        result.OptionalString[0].Should().BeNull();
        result.OptionalString[1].Should().Be("optional");
    }

    [Fact]
    public void ToUnnestable_WithRecordClass()
    {
        // Arrange
        var items = new[]
        {
            new RecordClassDto(Id: 1, Name: "Test1", Value: 100),
            new RecordClassDto(Id: 2, Name: "Test2", Value: 200)
        };

        // Act
        var result = items.ToUnnestable();

        // Assert
        result.Id.Should().Equal(1, 2);
        result.Name.Should().Equal("Test1", "Test2");
        result.Value.Should().Equal(100, 200);
    }

    [Fact]
    public void ToUnnestable_WithRecordWithNullableProperties()
    {
        // Arrange
        var items = new[]
        {
            new RecordWithNullableProps(Id: 1, Name: null, Value: 10),
            new RecordWithNullableProps(Id: 2, Name: "Test", Value: null)
        };

        // Act
        var result = items.ToUnnestable();

        // Assert
        result.Id.Should().Equal(1, 2);
        result.Name[0].Should().BeNull();
        result.Name[1].Should().Be("Test");
        result.Value[0].Should().Be(10);
        result.Value[1].Should().BeNull();
    }

    [Fact]
    public void ToUnnestable_WithRecordWithMixedProperties()
    {
        // Arrange
        var items = new[]
        {
            new MixedPropsRecord(Id: 1, Name: "A") { AdditionalValue = 100 },
            new MixedPropsRecord(Id: 2, Name: "B") { AdditionalValue = 200 }
        };

        // Act
        var result = items.ToUnnestable();

        // Assert
        result.Id.Should().Equal(1, 2);
        result.Name.Should().Equal("A", "B");
        result.AdditionalValue.Should().Equal(100, 200);
    }

    [Fact]
    public void ToUnnestable_WithRecordWithReadOnlyProperties()
    {
        // Arrange
        var items = new[]
        {
            new ReadOnlyRecord(Id: 1, Computed: "Value1"),
            new ReadOnlyRecord(Id: 2, Computed: "Value2")
        };

        // Act
        var result = items.ToUnnestable();

        result.Id.Should().Equal(1, 2);
        result.Computed.Should().Equal("Value1", "Value2");
    }
}
