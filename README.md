# Unnestable

[![NuGet](https://img.shields.io/nuget/v/Unnestable.svg)](https://www.nuget.org/packages/Unnestable/)
[![NuGet](https://img.shields.io/nuget/dt/Unnestable.svg)](https://www.nuget.org/packages/Unnestable/)

A C# source generator that automatically generates extension methods to "unnest" collections of objects into arrays for each property. This is particularly useful for [Dapper](https://github.com/DapperLib/Dapper) bulk operations where you need to pass arrays of values for each column.

## Table of Contents

- [Installation](#installation)
- [How It Works](#how-it-works)
- [Usage](#usage)
- [Supported Types](#supported-types)
- [Examples](#examples)
- [Generated Code](#generated-code)
- [Performance](#performance)
- [Contributing](#contributing)
- [License](#license)

## Installation

Install the NuGet package:

```bash
dotnet add package Unnestable
```

The package includes both the attribute library and the source generator.

## How It Works

1. Mark your class/record with the `[Unnestable]` attribute
2. The source generator automatically creates:
   - A `{YourType}Unnestable` class with array properties for each public property
   - Extension methods `ToUnnestable()` on `IEnumerable<YourType>` and `IReadOnlyCollection<YourType>`

## Usage

### Basic Example

```csharp
using Dapper.Unnest;

[Unnestable]
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

class Program
{
    static void Main()
    {
        var products = new[]
        {
            new Product { Id = 1, Name = "Laptop", Price = 999.99m },
            new Product { Id = 2, Name = "Mouse", Price = 29.99m },
        };

        var unnestable = products.ToUnnestable();

        // unnestable.Id contains [1, 2]
        // unnestable.Name contains ["Laptop", "Mouse"]
        // unnestable.Price contains [999.99m, 29.99m]
    }
}
```

### Dapper Bulk Insert Example

```csharp
[Unnestable]
public class User
{
    public string Email { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}

public async Task BulkInsertUsers(IEnumerable<User> users)
{
    var unnestable = users.ToUnnestable();

    using var connection = new SqlConnection(connectionString);
    await connection.ExecuteAsync(@"
        INSERT INTO Users (Email, Name, CreatedAt)
        SELECT Email, Name, CreatedAt
        FROM UNNEST(@Email, @Name, @CreatedAt) AS u(Email, Name, CreatedAt)",
        unnestable);
}
```

## Supported Types

The generator supports:

- **Classes and Records**: Both regular classes and record types
- **All Property Types**: Primitive types, custom classes, structs, arrays, nullable types
- **Accessibility Modifiers**: Matches the accessibility of the source type
- **Complex Types**: Nested objects, collections, enums, etc.
- **Nullable Reference Types**: Properly handles nullable and non-nullable reference types

### Limitations

- Only public properties with public getters are included (indexers are excluded)
- Properties must have a getter method
- The generated code inherits the accessibility of the source type

## Examples

### Arrays and Collections

```csharp
[Unnestable]
public class Order
{
    public int OrderId { get; set; }
    public string[] Tags { get; set; }
    public List<string> Categories { get; set; }
}

var orders = new[]
{
    new Order { OrderId = 1, Tags = ["urgent", "priority"], Categories = ["electronics"] },
    new Order { OrderId = 2, Tags = ["normal"], Categories = ["books", "fiction"] }
};

var unnestable = orders.ToUnnestable();
// unnestable.OrderId: [1, 2]
// unnestable.Tags: [["urgent", "priority"], ["normal"]]
// unnestable.Categories: [["electronics"], ["books", "fiction"]]
```

### Records and Structs

```csharp
[Unnestable]
public record Person(string Name, int Age);

[Unnestable]
public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }
}
```

### Nullable Types

```csharp
[Unnestable]
public class OptionalData
{
    public string? Description { get; set; }
    public int? Count { get; set; }
}
```

## Generated Code

For a class like:

```csharp
[Unnestable]
public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

The generator produces:

```csharp
public sealed class ItemUnnestable
{
    public int[] Id { get; set; } = Array.Empty<int>();
    public string[] Name { get; set; } = Array.Empty<string>();
}

public static class ItemUnnestExtensions
{
    public static ItemUnnestable ToUnnestable(this System.Collections.Generic.IEnumerable<Item> source, int count)
    {
        var idArray = new int[count];
        var nameArray = new string[count];

        int i = 0;
        foreach (var item in source)
        {
            idArray[i] = item.Id;
            nameArray[i] = item.Name;
            i++;
        }

        var result = new ItemUnnestable();
        result.Id = idArray;
        result.Name = nameArray;

        return result;
    }

    public static ItemUnnestable ToUnnestable(this System.Collections.Generic.IReadOnlyCollection<Item> source)
    {
        return source.ToUnnestable(source.Count);
    }
}
```

## Performance

- **Memory Efficient**: Uses exact-sized arrays based on collection count
- **Zero-Allocation for Arrays**: Pre-allocates arrays and fills them in a single pass
- **Fast Execution**: Simple loop-based transformation with minimal overhead
- **Type-Safe**: Compile-time generated code with full type safety

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

### Development Setup

1. Clone the repository
2. Run tests: `dotnet test`
3. Build the solution: `dotnet build`

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

Made with ❤️ for the .NET community
