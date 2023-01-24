namespace OptionTypes;

public sealed class Maybe<T> : IEquatable<Maybe<T>>
{
    private readonly T? _value;

    public Maybe(T value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    private Maybe()
    {

    }

    public Maybe<TResult> Map<TResult>(Func<T, TResult> map) 
        => _value is null 
        ? Maybe<TResult>.None() 
        : map(_value);

    public Maybe<TResult> Bind<TResult>(Func<T, Maybe<TResult>> map) 
        => _value is null 
        ? Maybe<TResult>.None() 
        : map(_value);

    public T ValueOr(Func<T> valueProvider) 
        => _value ?? valueProvider();

    public T ValueOr(T value) 
        => _value ?? value;

    public static implicit operator Maybe<T>(T value) 
        => new(value);

    public static Maybe<T> None() 
        => new();

    public override string ToString()
        => _value is null
        ? "None"
        : $"Some {_value}";

    public override int GetHashCode()
        => _value?.GetHashCode() ?? 0;

    public override bool Equals(object? obj) 
        => Equals(obj as Maybe<T>);

    public bool Equals(Maybe<T>? other)
    {
        if(other is null)
        {
            return false;
        }

        if(_value is null && other._value is null)
        {
            return true;
        }

        return _value is not null
            ? _value.Equals(other._value)
            : other._value!.Equals(_value);            
    }
}

public static class Maybe
{
    public static Maybe<T> FromValue<T>(T? value) 
        => value is null ? Maybe<T>.None() : new Maybe<T>(value);
    public static Task<Maybe<TResult>> Map<T, TResult>(this Task<Maybe<T>> task, Func<T, TResult> map) 
        => task.ContinueWith(r => r.Result.Map(map), TaskContinuationOptions.OnlyOnRanToCompletion);
}
