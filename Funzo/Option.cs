using System.Diagnostics.CodeAnalysis;

namespace Funzo;

/// <summary>
/// Represents a variable of type <typeparamref name="T"/> that may have no value
/// </summary>
/// <typeparam name="T">The type of the internal value</typeparam>
public sealed class Option<T> : IEquatable<Option<T>>
{
    /// <summary>
    /// If true, a value has been supplied
    /// </summary>
    private readonly bool _hasValue;

    /// <summary>
    /// The value of the instance
    /// </summary>
    private readonly T? _value;

    private Option(T value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
        _hasValue = true;
    }

    private Option() 
    {
        _value = default;
        _hasValue = false;
    }

    /// <summary>
    /// Maps the current instance to another type
    /// </summary>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="map">The mapper function to execute</param>
    /// <returns>If this instance has a value, <paramref name="map"/> will be executed, if not, <see cref="Option{T}.None"/> will be returned</returns>
    public Option<TResult> Map<TResult>(Func<T, TResult> map)
        => _hasValue
        ? map(_value!)
        : Option<TResult>.None();

    /// <summary>
    /// Maps and flattens the current instance to another type
    /// </summary>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="map">The mapper function to execute</param>
    /// <returns>If this instance has a value, <paramref name="map"/> will be executed and then flattened, if not, <see cref="Option{T}.None"/> will be returned</returns>
    public Option<TResult> Map<TResult>(Func<T, Option<TResult>> map)
        => _hasValue
        ? map(_value!)
        : Option<TResult>.None();

    /// <summary>
    /// Returns a result based on the presence or absence of value
    /// </summary>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="some">The function to execute if this instance has a value</param>
    /// <param name="none">The function to execute if this instance does not have a value</param>
    /// <returns>The result of the function executed depending on the value</returns>
    public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
        => _hasValue
        ? some(_value!)
        : none();

    /// <summary>
    /// Inspects the value of the option if it exists
    /// </summary>
    /// <param name="action">The action to take</param>
    public void Inspect(Action<T> action)
    {
        if(_hasValue)
        {
            action(_value!);
        }
    }

    /// <summary>
    /// Inspects the value of the option if it exists
    /// </summary>
    /// <param name="action">The action to take</param>
    /// <returns></returns>
    public async Task Inspect(Func<T, Task> action)
    {
        if(_hasValue)
        {
            await action(_value!);
        }
    }

    /// <summary>
    /// Returns the value of this instance or <paramref name="value"/> if none
    /// </summary>
    /// <param name="value">The value returned if this instance has no value</param>
    /// <returns>The value of this instance or <paramref name="value"/> if none</returns>
    public T ValueOr(T value)
        => _hasValue ? _value! : value;

    /// <summary>
    /// Returns the value of this instance if exists or the default value
    /// </summary>
    /// <returns>The value of this instance or default if none</returns>
    public T? ValueOrDefault() => _hasValue ? _value! : default;

    /// <summary>
    /// Returns the value if exists or throws a <see cref="NullReferenceException"/>. This method should be avoided whenever possible in favour of <see cref="IsSome(out T)"/>
    /// </summary>
    /// <returns>The value of the option</returns>
    /// <exception cref="NullReferenceException"></exception>
    public T Unwrap() => _hasValue ? _value! : throw new NullReferenceException();

    /// <summary>
    /// Returns <see langword="true" /> and assigns <paramref name="value"/> when it has value, <see langword="false"/> otherwise.
    /// </summary>
    /// <param name="value">The value contained in this instance, or <see langword="default"/> if no value present</param>
    /// <returns><see langword="true" /> when a value exists, <see langword="false" /> otherwise</returns>
    public bool IsSome([NotNullWhen(true)]out T? value)
    {
        value = _value!;

        return _hasValue;
    }

    /// <summary>
    /// Converts between a value and a <see cref="Option{T}"/> instance
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator Option<T>(T value)
       => new(value);

    /// <summary>
    /// Creates an instance of <see cref="Option{T}"/> with the value <paramref name="value"/>. This method throws if <paramref name="value"/> is <see langword="null" />
    /// </summary>
    /// <param name="value">The value of this instance</param>
    /// <returns>The <see cref="Option{T}"/> instance created</returns>
    /// <exception cref="ArgumentNullException"  />
    public static Option<T> Some(T value)
        => new(value);

    /// <summary>
    /// Creates an empty instance of <see cref="Option{T}"/>
    /// </summary>
    /// <returns>An empty instance of <see cref="Option{T}"/></returns>
    public static Option<T> None()
        => new();

    /// <summary>
    /// Returns the string representation of this class
    /// </summary>
    /// <returns>The string representation of this class</returns>
    public override string ToString()
        => _hasValue
        ? $"Some {_value}"
        : "None";

    /// <inheritdoc/>
    public override int GetHashCode()
        => _hasValue ? _value!.GetHashCode() : 0;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is Option<T> option && option.Equals(this);

    /// <inheritdoc/>
    public bool Equals(Option<T>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (!_hasValue && !other._hasValue)
        {
            return true;
        }

        return _hasValue
            ? _value!.Equals(other._value)
            : other._value!.Equals(_value);
    }
}

/// <summary>
/// Helper class to create <see cref="Option{T}"/> instances
/// </summary>
public static class Option
{
    /// <summary>
    /// Creates a new <see cref="Option{T}"/> with some value <paramref name="value"/>
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value to set in the <see cref="Option{T}"/> instance</param>
    /// <returns>The <see cref="Option{T}"/> instance</returns>
    public static Option<T> Some<T>(T value)
        => Option<T>.Some(value);

    /// <summary>
    /// Creates a new <see cref="Option{T}"/> instance based on the value supplied
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value that will be used to create the instance</param>
    /// <returns>The <see cref="Option{T}"/> instance</returns>
    public static Option<T> FromValue<T>(T? value)
        where T : class
        => value is null ? Option<T>.None() : Option<T>.Some(value);

    /// <summary>
    /// Creates a new <see cref="Option{T}"/> instance based on the value supplied
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value that will be used to create the instance</param>
    /// <returns>The <see cref="Option{T}"/> instance</returns>
    public static Option<T> FromValue<T>(T? value)
        where T : struct
        => value is { } v ? Option<T>.Some(v) : Option<T>.None();

    /// <summary>
    /// Creates a continuation of the current task that will map the value returned
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="task">The task to attach the continuation to</param>
    /// <param name="map">The function that maps the value</param>
    /// <returns>A new task that has the original value mapped through <paramref name="map"/></returns>
    public static Task<Option<TResult>> Map<T, TResult>(this Task<Option<T>> task, Func<T, TResult> map)
        => task.Then(t => t.Map(map));

    /// <summary>
    /// Creates a continuation of the current task that will map and flatten the value returned
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="task">The task to attach the continuation to</param>
    /// <param name="map">The function that maps the value</param>
    /// <returns>A new task that has the original value mapped through <paramref name="map"/></returns>
    public static Task<Option<TResult>> Map<T, TResult>(this Task<Option<T>> task, Func<T, Option<TResult>> map)
        => task.Then(t => t.Map(map));

    /// <summary>
    /// Creates a continuation of the current task that will match against the <see cref="Option{T}"/> instance
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="task">The task to attach the continuation to</param>
    /// <param name="some">The function to run when exists value</param>
    /// <param name="none">The function to run when does not exist a value</param>
    /// <returns>A new task wrapping the result</returns>
    public static Task<TResult> Match<TIn, TResult>(this Task<Option<TIn>> task, Func<TIn, TResult> some, Func<TResult> none)
        => task.Then(t => t.Match(some, none));

    /// <summary>
    /// Creates a continuation of the current task that will return the <see cref="Option{T}"/> value or <paramref name="value"/> if <see cref="Option{T}.None"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task">The task to attach the continuation to</param>
    /// <param name="value">The value to return in the task if none</param>
    /// <returns>A new task wrapping the operation</returns>
    public static Task<T> ValueOr<T>(this Task<Option<T>> task, T value)
        => task.Then(t => t.ValueOr(value));

    /// <summary>
    /// Creates a continuation of the current task that will return the inner value inside <see cref="Option{T}"/> or throw if none
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task">The task to attach the continuation to</param>
    /// <returns>The value of the <see cref="Option{T}"/> or throws</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Task<T> Unwrap<T>(this Task<Option<T>> task)
        => task.Then(t => t.Unwrap());
}
