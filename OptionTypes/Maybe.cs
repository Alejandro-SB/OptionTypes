using System.Diagnostics;

namespace OptionTypes;

/// <summary>
/// Represents a variable of type <typeparamref name="T"/> that may have no value
/// </summary>
/// <typeparam name="T">The type of the internal value</typeparam>
public sealed class Maybe<T> : IEquatable<Maybe<T>>
{
    /// <summary>
    /// If true, a value has been supplied
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly bool _hasValue;

    /// <summary>
    /// The value of the instance
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly T? _value;

    private Maybe(T value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
        _hasValue = true;
    }

    private Maybe()
    {
        _value = default;
        _hasValue = false;
    }

    /// <summary>
    /// Maps the current instance to another type
    /// </summary>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="map">The mapper function to execute</param>
    /// <returns>If this instance has a value, <paramref name="map"/> will be executed, if not, <see cref="Maybe{T}.None"/> will be returned</returns>
    public Maybe<TResult> Map<TResult>(Func<T, TResult> map)
        => _hasValue
        ? map(_value!)
        : Maybe<TResult>.None();

    /// <summary>
    /// Maps and flattens the current instance to another type
    /// </summary>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="map">The mapper function to execute</param>
    /// <returns>If this instance has a value, <paramref name="map"/> will be executed and then flattened, if not, <see cref="Maybe{T}.None"/> will be returned</returns>
    public Maybe<TResult> Map<TResult>(Func<T, Maybe<TResult>> map)
        => _hasValue
        ? map(_value!)
        : Maybe<TResult>.None();

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
    /// Returns the value of this instance or executes <paramref name="valueProvider"/> if none
    /// </summary>
    /// <param name="valueProvider">Function that will return the value if there is no value in this instance</param>
    /// <returns>The value of this instance or the value returned by <paramref name="valueProvider"/></returns>
    public T ValueOr(Func<T> valueProvider)
        => _hasValue ? _value! : valueProvider();

    /// <summary>
    /// Returns the value of this instance or <paramref name="value"/> if none
    /// </summary>
    /// <param name="value">The value returned if this instance has no value</param>
    /// <returns>The value of this instance or <paramref name="value"/> if none</returns>
    public T ValueOr(T value)
        => _hasValue ? _value! : value;

    /// <summary>
    /// Returns the value if exists or throws a <see cref="NullReferenceException"/>
    /// </summary>
    /// <returns>The value of the maybe</returns>
    /// <exception cref="NullReferenceException"></exception>
    public T Unwrap() => _value ?? throw new NullReferenceException();

    /// <summary>
    /// Converts between a value and a Maybe instance
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator Maybe<T>(T value)
       => new(value);

    /// <summary>
    /// Creates an instance of <see cref="Maybe{T}"/> with the value <paramref name="value"/>
    /// </summary>
    /// <param name="value">The value of this instance</param>
    /// <returns>The <see cref="Maybe{T}"/> instance created</returns>
    public static Maybe<T> Some(T value)
        => new(value);

    /// <summary>
    /// Creates an empty instance of <see cref="Maybe{T}"/>
    /// </summary>
    /// <returns>An empty instance of <see cref="Maybe{T}"/></returns>
    public static Maybe<T> None()
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
        => obj is Maybe<T> maybe && maybe.Equals(this);

    /// <inheritdoc/>
    public bool Equals(Maybe<T>? other)
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
/// Helper class to create <see cref="Maybe{T}"/> instances
/// </summary>
public static class Maybe
{
    /// <summary>
    /// Creates a new <see cref="Maybe{T}"/> with some value <paramref name="value"/>
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value to set in the <see cref="Maybe{T}"/> instance</param>
    /// <returns>The <see cref="Maybe{T}"/> instance</returns>
    public static Maybe<T> Some<T>(T value)
        => Maybe<T>.Some(value);

    /// <summary>
    /// Creates a new <see cref="Maybe{T}"/> instance based on the value supplied
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value that will be used to create the instance</param>
    /// <returns>The <see cref="Maybe{T}"/> instance</returns>
    public static Maybe<T> FromValue<T>(T? value)
        where T : class
        => value is null ? Maybe<T>.None() : Maybe<T>.Some(value);

    /// <summary>
    /// Creates a new <see cref="Maybe{T}"/> instance based on the value supplied
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value that will be used to create the instance</param>
    /// <returns>The <see cref="Maybe{T}"/> instance</returns>
    public static Maybe<T> FromValue<T>(T? value)
        where T : struct
        => value is { } v ? Maybe<T>.Some(v) : Maybe<T>.None();

    /// <summary>
    /// Creates a continuation of the current task that will map the value returned
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="task">The task to attach the continuation to</param>
    /// <param name="map">The function that maps the value</param>
    /// <returns>A new task that has the original value mapped through <paramref name="map"/></returns>
    public static Task<Maybe<TResult>> Map<T, TResult>(this Task<Maybe<T>> task, Func<T, TResult> map)
        => task.Then(t => t.Map(map));

    /// <summary>
    /// Creates a continuation of the current task that will map and flatten the value returned
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="task">The task to attach the continuation to</param>
    /// <param name="map">The function that maps the value</param>
    /// <returns>A new task that has the original value mapped through <paramref name="map"/></returns>
    public static Task<Maybe<TResult>> Map<T, TResult>(this Task<Maybe<T>> task, Func<T, Maybe<TResult>> map)
        => task.Then(t => t.Map(map));

    /// <summary>
    /// Creates a continuation of the current task that will match against the <see cref="Maybe{T}"/> instance
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="task">The task to attach the continuation to</param>
    /// <param name="some">The function to run when exists value</param>
    /// <param name="none">The function to run when does not exist a value</param>
    /// <returns>A new task wrapping the result</returns>
    public static Task<TResult> Match<TIn, TResult>(this Task<Maybe<TIn>> task, Func<TIn, TResult> some, Func<TResult> none)
        => task.Then(t => t.Match(some, none));

    /// <summary>
    /// Creates a continuation of the current task that will return the <see cref="Maybe{T}"/> value or <paramref name="value"/> if <see cref="Maybe{T}.None"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task">The task to attach the continuation to</param>
    /// <param name="value">The value to return in the task if none</param>
    /// <returns>A new task wrapping the operation</returns>
    public static Task<T> ValueOr<T>(this Task<Maybe<T>> task, T value)
        => task.Then(t => t.ValueOr(value));

    /// <summary>
    /// Creates a continuation of the current task that will return the inner value inside <see cref="Maybe{T}"/> or throw if none
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task">The task to attach the continuation to</param>
    /// <returns>The value of the <see cref="Maybe{T}"/> or throws</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Task<T> Unwrap<T>(this Task<Maybe<T>> task)
        => task.Then(t => t.Unwrap());
}
