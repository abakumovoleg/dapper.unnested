namespace Dapper.Unnest.Generator.Tests.Dtos;

[Unnestable]
public class WithPrivateProps
{
    // Публичное свойство - должно быть включено в генерацию
    public int PublicProp { get; set; }
    
    // Приватное свойство - должно быть проигнорировано генератором
    private int PrivateProp { get; set; }
    
    // Protected свойство - должно быть проигнорировано
    protected int ProtectedProp { get; set; }
    
    // Internal свойство - должно быть проигнорировано (т.к. DeclaredAccessibility != Public)
    internal int InternalProp { get; set; }
    
    // Публичное свойство только для чтения - должно быть проигнорировано
    // (нет setter, хотя по текущей логике GetMethod != null достаточно)
    public int ReadOnlyProp => 42;
}