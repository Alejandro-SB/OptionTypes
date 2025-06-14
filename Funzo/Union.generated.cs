#nullable enable
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Funzo;

/// <summary>
/// Base class for the union types
/// </summary>
public abstract class UnionBase
{
    internal abstract object GetValue();
}


/// <summary>
/// Class an entity that can be different types
/// </summary>
/// <typeparam name="T0"></typeparam>
/// <typeparam name="T1"></typeparam>
public class Union<T0, T1> : UnionBase, IEquatable<Union<T0, T1>>
    where T0 : notnull
where T1 : notnull
{
    /// <summary>
    /// Stores lazily all the types of this instance to easily check if matches another one
    /// </summary>
    private static readonly Lazy<Type[]> UnionTypeDefinitions = new(() => typeof(Union<T0, T1>).GetGenericArguments());

    internal override object GetValue() =>
        _index switch
        {
            0 => _value0!,
            1 => _value1!,

            _ => throw new IndexOutOfRangeException("Union went out of range")
        };

    private readonly int _index;

    private readonly T0 _value0 = default!;
    private readonly T1 _value1 = default!;

    /// <summary>
    /// Create an instance of <see cref="Union{T0,T1}" /> with the type <typeparamref name="T0" />
    /// </summary>
    /// <param name="value">The value to initialize this instance</param>"
    public Union(T0 value)
    {
        _index = 0;

        _value0 = value;
    }

    /// <summary>
    /// Create an instance of <see cref="Union{T0,T1}" /> with the type <typeparamref name="T1" />
    /// </summary>
    /// <param name="value">The value to initialize this instance</param>"
    public Union(T1 value)
    {
        _index = 1;

        _value1 = value;
    }


    /// <summary>
    /// Implicitly converts <typeparamref name="T0" /> into <see cref="Union{T0,T1}" />
    /// </summary>
    /// <param name="x">The value to convert from</param>
    public static implicit operator Union<T0, T1>(T0 x) => new(x);
    /// <summary>
    /// Implicitly converts <typeparamref name="T1" /> into <see cref="Union{T0,T1}" />
    /// </summary>
    /// <param name="x">The value to convert from</param>
    public static implicit operator Union<T0, T1>(T1 x) => new(x);

    /// <summary>
    /// Checks if this <see cref="Union{T0,T1}" /> instance is of type <typeparamref name="T" /> and gets the value if it is
    /// </summary>
    /// <remarks>
    /// NOTE: Please be aware that this method may return unexpected results if this <see cref="Union{T0,T1}" /> instance has two equal generic type parameters
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">If this instance is of type <typeparamref name="T" />, has its value assigned, <see langword="default" /> otherwise</param>
    /// <returns><see langword="true" /> if this instance is of type <typeparamref name="T" />, <see langword="false" /> otherwise</returns>
    public bool Is<T>([NotNullWhen(true)] out T? value)
    {
        var valueType = GetValue().GetType();
        var isSameType = valueType == typeof(T);

        if (isSameType)
        {
            value = (T?)GetValue();
        }
        else
        {
            value = default;
        }

        return isSameType;
    }

    /// <summary>
    /// Executes an action based on the type of this <see cref="Union{T0,T1}" /> instance
    /// </summary>
    /// <param name="action0">The action to take if this instance is of type <typeparamref name="T0" /></param>
    /// <param name="action1">The action to take if this instance is of type <typeparamref name="T1" /></param>
    public void Switch(Action<T0> action0, Action<T1> action1)
    {
        switch (_index)
        {
            case 0:
                action0(_value0);
                return;
            case 1:
                action1(_value1);
                return;
            default:
                throw new IndexOutOfRangeException("Union went out of range");
        }
    }

    /// <summary>
    /// Executes an action based on the type of this <see cref="Union{T0,T1}" /> instance
    /// </summary>
    /// <param name="action0">The action to take if this instance is of type <typeparamref name="T0" /></param>
    /// <param name="action1">The action to take if this instance is of type <typeparamref name="T1" /></param>
    public async Task SwitchAsync(Func<T0, Task> action0, Func<T1, Task> action1)
    {
        switch (_index)
        {
            case 0:
                await action0(_value0);
                return;
            case 1:
                await action1(_value1);
                return;
            default:
                throw new IndexOutOfRangeException("Union went out of range");
        }
    }


    /// <summary>
    /// Matches the value of this instance against a mapping function
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    /// <param name="func0"></param>
    /// <param name="func1"></param>
    /// <returns></returns>
    public TOut Match<TOut>(Func<T0, TOut> func0, Func<T1, TOut> func1)
    {
        return _index switch
        {
            0 => func0(_value0),
            1 => func1(_value1),
            _ => throw new IndexOutOfRangeException("Union went out of range")
        };
    }


    /// <inheritdoc />
    public bool Equals(Union<T0, T1>? other)
    {
        return other is not null && other._index == _index && other.GetValue().Equals(GetValue());
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Union<T0, T1> union && union.Equals(this) || obj is UnionBase u && this.Equals(u);
    }

    /// <inhericdoc />
    internal bool Equals(UnionBase other)
    {
        return other.GetValue().Equals(GetValue());
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
#if NETSTANDARD2_0
        unchecked
        {
            return (GetValue().GetHashCode() * 397) ^ _index;
        }
#else
        return HashCode.Combine(_index, GetValue().GetHashCode());
#endif
    }
}



/// <summary>
/// Class an entity that can be different types
/// </summary>
/// <typeparam name="T0"></typeparam>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
public class Union<T0, T1, T2> : UnionBase, IEquatable<Union<T0, T1, T2>>
    where T0 : notnull
where T1 : notnull
where T2 : notnull
{
    /// <summary>
    /// Stores lazily all the types of this instance to easily check if matches another one
    /// </summary>
    private static readonly Lazy<Type[]> UnionTypeDefinitions = new(() => typeof(Union<T0, T1, T2>).GetGenericArguments());

    internal override object GetValue() =>
        _index switch
        {
            0 => _value0!,
            1 => _value1!,
            2 => _value2!,

            _ => throw new IndexOutOfRangeException("Union went out of range")
        };

    private readonly int _index;

    private readonly T0 _value0 = default!;
    private readonly T1 _value1 = default!;
    private readonly T2 _value2 = default!;

    /// <summary>
    /// Create an instance of <see cref="Union{T0,T1,T2}" /> with the type <typeparamref name="T0" />
    /// </summary>
    /// <param name="value">The value to initialize this instance</param>"
    public Union(T0 value)
    {
        _index = 0;

        _value0 = value;
    }

    /// <summary>
    /// Create an instance of <see cref="Union{T0,T1,T2}" /> with the type <typeparamref name="T1" />
    /// </summary>
    /// <param name="value">The value to initialize this instance</param>"
    public Union(T1 value)
    {
        _index = 1;

        _value1 = value;
    }

    /// <summary>
    /// Create an instance of <see cref="Union{T0,T1,T2}" /> with the type <typeparamref name="T2" />
    /// </summary>
    /// <param name="value">The value to initialize this instance</param>"
    public Union(T2 value)
    {
        _index = 2;

        _value2 = value;
    }


    /// <summary>
    /// Implicitly converts <typeparamref name="T0" /> into <see cref="Union{T0,T1,T2}" />
    /// </summary>
    /// <param name="x">The value to convert from</param>
    public static implicit operator Union<T0, T1, T2>(T0 x) => new(x);
    /// <summary>
    /// Implicitly converts <typeparamref name="T1" /> into <see cref="Union{T0,T1,T2}" />
    /// </summary>
    /// <param name="x">The value to convert from</param>
    public static implicit operator Union<T0, T1, T2>(T1 x) => new(x);
    /// <summary>
    /// Implicitly converts <typeparamref name="T2" /> into <see cref="Union{T0,T1,T2}" />
    /// </summary>
    /// <param name="x">The value to convert from</param>
    public static implicit operator Union<T0, T1, T2>(T2 x) => new(x);

    /// <summary>
    /// Checks if this <see cref="Union{T0,T1,T2}" /> instance is of type <typeparamref name="T" /> and gets the value if it is
    /// </summary>
    /// <remarks>
    /// NOTE: Please be aware that this method may return unexpected results if this <see cref="Union{T0,T1,T2}" /> instance has two equal generic type parameters
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">If this instance is of type <typeparamref name="T" />, has its value assigned, <see langword="default" /> otherwise</param>
    /// <returns><see langword="true" /> if this instance is of type <typeparamref name="T" />, <see langword="false" /> otherwise</returns>
    public bool Is<T>([NotNullWhen(true)] out T? value)
    {
        var valueType = GetValue().GetType();
        var isSameType = valueType == typeof(T);

        if (isSameType)
        {
            value = (T?)GetValue();
        }
        else
        {
            value = default;
        }

        return isSameType;
    }

    /// <summary>
    /// Executes an action based on the type of this <see cref="Union{T0,T1,T2}" /> instance
    /// </summary>
    /// <param name="action0">The action to take if this instance is of type <typeparamref name="T0" /></param>
    /// <param name="action1">The action to take if this instance is of type <typeparamref name="T1" /></param>
    /// <param name="action2">The action to take if this instance is of type <typeparamref name="T2" /></param>
    public void Switch(Action<T0> action0, Action<T1> action1, Action<T2> action2)
    {
        switch (_index)
        {
            case 0:
                action0(_value0);
                return;
            case 1:
                action1(_value1);
                return;
            case 2:
                action2(_value2);
                return;
            default:
                throw new IndexOutOfRangeException("Union went out of range");
        }
    }

    /// <summary>
    /// Executes an action based on the type of this <see cref="Union{T0,T1,T2}" /> instance
    /// </summary>
    /// <param name="action0">The action to take if this instance is of type <typeparamref name="T0" /></param>
    /// <param name="action1">The action to take if this instance is of type <typeparamref name="T1" /></param>
    /// <param name="action2">The action to take if this instance is of type <typeparamref name="T2" /></param>
    public async Task SwitchAsync(Func<T0, Task> action0, Func<T1, Task> action1, Func<T2, Task> action2)
    {
        switch (_index)
        {
            case 0:
                await action0(_value0);
                return;
            case 1:
                await action1(_value1);
                return;
            case 2:
                await action2(_value2);
                return;
            default:
                throw new IndexOutOfRangeException("Union went out of range");
        }
    }


    /// <summary>
    /// Matches the value of this instance against a mapping function
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    /// <param name="func0"></param>
    /// <param name="func1"></param>
    /// <param name="func2"></param>
    /// <returns></returns>
    public TOut Match<TOut>(Func<T0, TOut> func0, Func<T1, TOut> func1, Func<T2, TOut> func2)
    {
        return _index switch
        {
            0 => func0(_value0),
            1 => func1(_value1),
            2 => func2(_value2),
            _ => throw new IndexOutOfRangeException("Union went out of range")
        };
    }


    /// <inheritdoc />
    public bool Equals(Union<T0, T1, T2>? other)
    {
        return other is not null && other._index == _index && other.GetValue().Equals(GetValue());
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Union<T0, T1, T2> union && union.Equals(this) || obj is UnionBase u && this.Equals(u);
    }

    /// <inhericdoc />
    internal bool Equals(UnionBase other)
    {
        return other.GetValue().Equals(GetValue());
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
#if NETSTANDARD2_0
        unchecked
        {
            return (GetValue().GetHashCode() * 397) ^ _index;
        }
#else
        return HashCode.Combine(_index, GetValue().GetHashCode());
#endif
    }
}



/// <summary>
/// Class an entity that can be different types
/// </summary>
/// <typeparam name="T0"></typeparam>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
public class Union<T0, T1, T2, T3> : UnionBase, IEquatable<Union<T0, T1, T2, T3>>
    where T0 : notnull
where T1 : notnull
where T2 : notnull
where T3 : notnull
{
    /// <summary>
    /// Stores lazily all the types of this instance to easily check if matches another one
    /// </summary>
    private static readonly Lazy<Type[]> UnionTypeDefinitions = new(() => typeof(Union<T0, T1, T2, T3>).GetGenericArguments());

    internal override object GetValue() =>
        _index switch
        {
            0 => _value0!,
            1 => _value1!,
            2 => _value2!,
            3 => _value3!,

            _ => throw new IndexOutOfRangeException("Union went out of range")
        };

    private readonly int _index;

    private readonly T0 _value0 = default!;
    private readonly T1 _value1 = default!;
    private readonly T2 _value2 = default!;
    private readonly T3 _value3 = default!;

    /// <summary>
    /// Create an instance of <see cref="Union{T0,T1,T2,T3}" /> with the type <typeparamref name="T0" />
    /// </summary>
    /// <param name="value">The value to initialize this instance</param>"
    public Union(T0 value)
    {
        _index = 0;

        _value0 = value;
    }

    /// <summary>
    /// Create an instance of <see cref="Union{T0,T1,T2,T3}" /> with the type <typeparamref name="T1" />
    /// </summary>
    /// <param name="value">The value to initialize this instance</param>"
    public Union(T1 value)
    {
        _index = 1;

        _value1 = value;
    }

    /// <summary>
    /// Create an instance of <see cref="Union{T0,T1,T2,T3}" /> with the type <typeparamref name="T2" />
    /// </summary>
    /// <param name="value">The value to initialize this instance</param>"
    public Union(T2 value)
    {
        _index = 2;

        _value2 = value;
    }

    /// <summary>
    /// Create an instance of <see cref="Union{T0,T1,T2,T3}" /> with the type <typeparamref name="T3" />
    /// </summary>
    /// <param name="value">The value to initialize this instance</param>"
    public Union(T3 value)
    {
        _index = 3;

        _value3 = value;
    }


    /// <summary>
    /// Implicitly converts <typeparamref name="T0" /> into <see cref="Union{T0,T1,T2,T3}" />
    /// </summary>
    /// <param name="x">The value to convert from</param>
    public static implicit operator Union<T0, T1, T2, T3>(T0 x) => new(x);
    /// <summary>
    /// Implicitly converts <typeparamref name="T1" /> into <see cref="Union{T0,T1,T2,T3}" />
    /// </summary>
    /// <param name="x">The value to convert from</param>
    public static implicit operator Union<T0, T1, T2, T3>(T1 x) => new(x);
    /// <summary>
    /// Implicitly converts <typeparamref name="T2" /> into <see cref="Union{T0,T1,T2,T3}" />
    /// </summary>
    /// <param name="x">The value to convert from</param>
    public static implicit operator Union<T0, T1, T2, T3>(T2 x) => new(x);
    /// <summary>
    /// Implicitly converts <typeparamref name="T3" /> into <see cref="Union{T0,T1,T2,T3}" />
    /// </summary>
    /// <param name="x">The value to convert from</param>
    public static implicit operator Union<T0, T1, T2, T3>(T3 x) => new(x);

    /// <summary>
    /// Checks if this <see cref="Union{T0,T1,T2,T3}" /> instance is of type <typeparamref name="T" /> and gets the value if it is
    /// </summary>
    /// <remarks>
    /// NOTE: Please be aware that this method may return unexpected results if this <see cref="Union{T0,T1,T2,T3}" /> instance has two equal generic type parameters
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">If this instance is of type <typeparamref name="T" />, has its value assigned, <see langword="default" /> otherwise</param>
    /// <returns><see langword="true" /> if this instance is of type <typeparamref name="T" />, <see langword="false" /> otherwise</returns>
    public bool Is<T>([NotNullWhen(true)] out T? value)
    {
        var valueType = GetValue().GetType();
        var isSameType = valueType == typeof(T);

        if (isSameType)
        {
            value = (T?)GetValue();
        }
        else
        {
            value = default;
        }

        return isSameType;
    }

    /// <summary>
    /// Executes an action based on the type of this <see cref="Union{T0,T1,T2,T3}" /> instance
    /// </summary>
    /// <param name="action0">The action to take if this instance is of type <typeparamref name="T0" /></param>
    /// <param name="action1">The action to take if this instance is of type <typeparamref name="T1" /></param>
    /// <param name="action2">The action to take if this instance is of type <typeparamref name="T2" /></param>
    /// <param name="action3">The action to take if this instance is of type <typeparamref name="T3" /></param>
    public void Switch(Action<T0> action0, Action<T1> action1, Action<T2> action2, Action<T3> action3)
    {
        switch (_index)
        {
            case 0:
                action0(_value0);
                return;
            case 1:
                action1(_value1);
                return;
            case 2:
                action2(_value2);
                return;
            case 3:
                action3(_value3);
                return;
            default:
                throw new IndexOutOfRangeException("Union went out of range");
        }
    }

    /// <summary>
    /// Executes an action based on the type of this <see cref="Union{T0,T1,T2,T3}" /> instance
    /// </summary>
    /// <param name="action0">The action to take if this instance is of type <typeparamref name="T0" /></param>
    /// <param name="action1">The action to take if this instance is of type <typeparamref name="T1" /></param>
    /// <param name="action2">The action to take if this instance is of type <typeparamref name="T2" /></param>
    /// <param name="action3">The action to take if this instance is of type <typeparamref name="T3" /></param>
    public async Task SwitchAsync(Func<T0, Task> action0, Func<T1, Task> action1, Func<T2, Task> action2, Func<T3, Task> action3)
    {
        switch (_index)
        {
            case 0:
                await action0(_value0);
                return;
            case 1:
                await action1(_value1);
                return;
            case 2:
                await action2(_value2);
                return;
            case 3:
                await action3(_value3);
                return;
            default:
                throw new IndexOutOfRangeException("Union went out of range");
        }
    }


    /// <summary>
    /// Matches the value of this instance against a mapping function
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    /// <param name="func0"></param>
    /// <param name="func1"></param>
    /// <param name="func2"></param>
    /// <param name="func3"></param>
    /// <returns></returns>
    public TOut Match<TOut>(Func<T0, TOut> func0, Func<T1, TOut> func1, Func<T2, TOut> func2, Func<T3, TOut> func3)
    {
        return _index switch
        {
            0 => func0(_value0),
            1 => func1(_value1),
            2 => func2(_value2),
            3 => func3(_value3),
            _ => throw new IndexOutOfRangeException("Union went out of range")
        };
    }


    /// <inheritdoc />
    public bool Equals(Union<T0, T1, T2, T3>? other)
    {
        return other is not null && other._index == _index && other.GetValue().Equals(GetValue());
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Union<T0, T1, T2, T3> union && union.Equals(this) || obj is UnionBase u && this.Equals(u);
    }

    /// <inhericdoc />
    internal bool Equals(UnionBase other)
    {
        return other.GetValue().Equals(GetValue());
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
#if NETSTANDARD2_0
        unchecked
        {
            return (GetValue().GetHashCode() * 397) ^ _index;
        }
#else
        return HashCode.Combine(_index, GetValue().GetHashCode());
#endif
    }
}



/// <summary>
/// Class an entity that can be different types
/// </summary>
/// <typeparam name="T0"></typeparam>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
/// <typeparam name="T4"></typeparam>
public class Union<T0, T1, T2, T3, T4> : UnionBase, IEquatable<Union<T0, T1, T2, T3, T4>>
    where T0 : notnull
where T1 : notnull
where T2 : notnull
where T3 : notnull
where T4 : notnull
{
    /// <summary>
    /// Stores lazily all the types of this instance to easily check if matches another one
    /// </summary>
    private static readonly Lazy<Type[]> UnionTypeDefinitions = new(() => typeof(Union<T0, T1, T2, T3, T4>).GetGenericArguments());

    internal override object GetValue() =>
        _index switch
        {
            0 => _value0!,
            1 => _value1!,
            2 => _value2!,
            3 => _value3!,
            4 => _value4!,

            _ => throw new IndexOutOfRangeException("Union went out of range")
        };

    private readonly int _index;

    private readonly T0 _value0 = default!;
    private readonly T1 _value1 = default!;
    private readonly T2 _value2 = default!;
    private readonly T3 _value3 = default!;
    private readonly T4 _value4 = default!;

    /// <summary>
    /// Create an instance of <see cref="Union{T0,T1,T2,T3,T4}" /> with the type <typeparamref name="T0" />
    /// </summary>
    /// <param name="value">The value to initialize this instance</param>"
    public Union(T0 value)
    {
        _index = 0;

        _value0 = value;
    }

    /// <summary>
    /// Create an instance of <see cref="Union{T0,T1,T2,T3,T4}" /> with the type <typeparamref name="T1" />
    /// </summary>
    /// <param name="value">The value to initialize this instance</param>"
    public Union(T1 value)
    {
        _index = 1;

        _value1 = value;
    }

    /// <summary>
    /// Create an instance of <see cref="Union{T0,T1,T2,T3,T4}" /> with the type <typeparamref name="T2" />
    /// </summary>
    /// <param name="value">The value to initialize this instance</param>"
    public Union(T2 value)
    {
        _index = 2;

        _value2 = value;
    }

    /// <summary>
    /// Create an instance of <see cref="Union{T0,T1,T2,T3,T4}" /> with the type <typeparamref name="T3" />
    /// </summary>
    /// <param name="value">The value to initialize this instance</param>"
    public Union(T3 value)
    {
        _index = 3;

        _value3 = value;
    }

    /// <summary>
    /// Create an instance of <see cref="Union{T0,T1,T2,T3,T4}" /> with the type <typeparamref name="T4" />
    /// </summary>
    /// <param name="value">The value to initialize this instance</param>"
    public Union(T4 value)
    {
        _index = 4;

        _value4 = value;
    }


    /// <summary>
    /// Implicitly converts <typeparamref name="T0" /> into <see cref="Union{T0,T1,T2,T3,T4}" />
    /// </summary>
    /// <param name="x">The value to convert from</param>
    public static implicit operator Union<T0, T1, T2, T3, T4>(T0 x) => new(x);
    /// <summary>
    /// Implicitly converts <typeparamref name="T1" /> into <see cref="Union{T0,T1,T2,T3,T4}" />
    /// </summary>
    /// <param name="x">The value to convert from</param>
    public static implicit operator Union<T0, T1, T2, T3, T4>(T1 x) => new(x);
    /// <summary>
    /// Implicitly converts <typeparamref name="T2" /> into <see cref="Union{T0,T1,T2,T3,T4}" />
    /// </summary>
    /// <param name="x">The value to convert from</param>
    public static implicit operator Union<T0, T1, T2, T3, T4>(T2 x) => new(x);
    /// <summary>
    /// Implicitly converts <typeparamref name="T3" /> into <see cref="Union{T0,T1,T2,T3,T4}" />
    /// </summary>
    /// <param name="x">The value to convert from</param>
    public static implicit operator Union<T0, T1, T2, T3, T4>(T3 x) => new(x);
    /// <summary>
    /// Implicitly converts <typeparamref name="T4" /> into <see cref="Union{T0,T1,T2,T3,T4}" />
    /// </summary>
    /// <param name="x">The value to convert from</param>
    public static implicit operator Union<T0, T1, T2, T3, T4>(T4 x) => new(x);

    /// <summary>
    /// Checks if this <see cref="Union{T0,T1,T2,T3,T4}" /> instance is of type <typeparamref name="T" /> and gets the value if it is
    /// </summary>
    /// <remarks>
    /// NOTE: Please be aware that this method may return unexpected results if this <see cref="Union{T0,T1,T2,T3,T4}" /> instance has two equal generic type parameters
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">If this instance is of type <typeparamref name="T" />, has its value assigned, <see langword="default" /> otherwise</param>
    /// <returns><see langword="true" /> if this instance is of type <typeparamref name="T" />, <see langword="false" /> otherwise</returns>
    public bool Is<T>([NotNullWhen(true)] out T? value)
    {
        var valueType = GetValue().GetType();
        var isSameType = valueType == typeof(T);

        if (isSameType)
        {
            value = (T?)GetValue();
        }
        else
        {
            value = default;
        }

        return isSameType;
    }

    /// <summary>
    /// Executes an action based on the type of this <see cref="Union{T0,T1,T2,T3,T4}" /> instance
    /// </summary>
    /// <param name="action0">The action to take if this instance is of type <typeparamref name="T0" /></param>
    /// <param name="action1">The action to take if this instance is of type <typeparamref name="T1" /></param>
    /// <param name="action2">The action to take if this instance is of type <typeparamref name="T2" /></param>
    /// <param name="action3">The action to take if this instance is of type <typeparamref name="T3" /></param>
    /// <param name="action4">The action to take if this instance is of type <typeparamref name="T4" /></param>
    public void Switch(Action<T0> action0, Action<T1> action1, Action<T2> action2, Action<T3> action3, Action<T4> action4)
    {
        switch (_index)
        {
            case 0:
                action0(_value0);
                return;
            case 1:
                action1(_value1);
                return;
            case 2:
                action2(_value2);
                return;
            case 3:
                action3(_value3);
                return;
            case 4:
                action4(_value4);
                return;
            default:
                throw new IndexOutOfRangeException("Union went out of range");
        }
    }

    /// <summary>
    /// Executes an action based on the type of this <see cref="Union{T0,T1,T2,T3,T4}" /> instance
    /// </summary>
    /// <param name="action0">The action to take if this instance is of type <typeparamref name="T0" /></param>
    /// <param name="action1">The action to take if this instance is of type <typeparamref name="T1" /></param>
    /// <param name="action2">The action to take if this instance is of type <typeparamref name="T2" /></param>
    /// <param name="action3">The action to take if this instance is of type <typeparamref name="T3" /></param>
    /// <param name="action4">The action to take if this instance is of type <typeparamref name="T4" /></param>
    public async Task SwitchAsync(Func<T0, Task> action0, Func<T1, Task> action1, Func<T2, Task> action2, Func<T3, Task> action3, Func<T4, Task> action4)
    {
        switch (_index)
        {
            case 0:
                await action0(_value0);
                return;
            case 1:
                await action1(_value1);
                return;
            case 2:
                await action2(_value2);
                return;
            case 3:
                await action3(_value3);
                return;
            case 4:
                await action4(_value4);
                return;
            default:
                throw new IndexOutOfRangeException("Union went out of range");
        }
    }


    /// <summary>
    /// Matches the value of this instance against a mapping function
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    /// <param name="func0"></param>
    /// <param name="func1"></param>
    /// <param name="func2"></param>
    /// <param name="func3"></param>
    /// <param name="func4"></param>
    /// <returns></returns>
    public TOut Match<TOut>(Func<T0, TOut> func0, Func<T1, TOut> func1, Func<T2, TOut> func2, Func<T3, TOut> func3, Func<T4, TOut> func4)
    {
        return _index switch
        {
            0 => func0(_value0),
            1 => func1(_value1),
            2 => func2(_value2),
            3 => func3(_value3),
            4 => func4(_value4),
            _ => throw new IndexOutOfRangeException("Union went out of range")
        };
    }


    /// <inheritdoc />
    public bool Equals(Union<T0, T1, T2, T3, T4>? other)
    {
        return other is not null && other._index == _index && other.GetValue().Equals(GetValue());
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Union<T0, T1, T2, T3, T4> union && union.Equals(this) || obj is UnionBase u && this.Equals(u);
    }

    /// <inhericdoc />
    internal bool Equals(UnionBase other)
    {
        return other.GetValue().Equals(GetValue());
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
#if NETSTANDARD2_0
        unchecked
        {
            return (GetValue().GetHashCode() * 397) ^ _index;
        }
#else
        return HashCode.Combine(_index, GetValue().GetHashCode());
#endif
    }
}

