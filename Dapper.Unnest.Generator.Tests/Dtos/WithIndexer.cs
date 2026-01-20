namespace Dapper.Unnest.Generator.Tests.Dtos;

[Unnestable]
public class WithIndexer
{
    // Обычное свойство
    public required string Value { get; set; }
    
    // Индексатор - это особый вид свойства
    // По текущей логике генератора он будет отфильтрован,
    // т.к. IPropertySymbol для индексаторов имеет параметры
    public string this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }
    
    private readonly List<string> _items = new();
}