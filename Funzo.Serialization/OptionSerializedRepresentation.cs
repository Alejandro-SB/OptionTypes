namespace Funzo.Serialization;

internal class OptionRepresentation<T>
{
    public bool HasValue { get; set; }
    public T? Value { get; set; }
}

internal class OptionNoneRepresentation
{
    public bool HasValue => false;
}

internal class OptionSomeRepresentation<T>
{
    public bool HasValue => true;
    public T Value { get; set; } = default!;
}
