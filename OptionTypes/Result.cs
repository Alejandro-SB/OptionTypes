using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace OptionTypes;

/// <summary>
/// Represents the result of an operation
/// </summary>
/// <typeparam name="TOk">The type of the result if successful</typeparam>
/// <typeparam name="TErr">The type of the result if an error occurs</typeparam>
public class Result<TOk, TErr>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly bool _isOk;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly TOk? _ok;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly TErr? _err;

    /// <summary>
    /// Creates a new instance of the <see cref="Result{TOk, TErr}"/> class as an OK result
    /// </summary>
    /// <param name="ok">The value for the OK type</param>
    protected Result(TOk ok)
    {
        _ok = ok;
        _isOk = true;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Result{TOk, TErr}"/> class as an Error result
    /// </summary>
    /// <param name="err">The value for the Error type</param>
    protected Result(TErr err)
    {
        _err = err;
        _isOk = false;
    }

    /// <summary>
    /// Matches the result depending on the state
    /// </summary>
    /// <typeparam name="TResult">The final result type</typeparam>
    /// <param name="ok">The function to execute if the operation was successful</param>
    /// <param name="err">The function to execute if the operation failed</param>
    /// <returns>The result of the mapping function depending on the result</returns>
    public TResult Match<TResult>(Func<TOk, TResult> ok, Func<TErr, TResult> err) => _isOk ? ok(_ok!) : err(_err!);

    /// <summary>
    /// Matches the result depending on the state
    /// </summary>
    /// <param name="ok">The function to execute if the operation was successful</param>
    /// <param name="err">The function to execute if the operation failed</param>
    public Unit Match(Action<TOk> ok, Action<TErr> err)
    {
        if (_isOk)
        {
            ok(_ok!);
        }
        else
        {
            err(_err!);
        }

        return default;
    }

    /// <summary>
    /// Returns a new <see cref="Result{T, TErr}"/> with the value corresponding to <typeparamref name="TOk"/> transformed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="map">The transformation to apply</param>
    /// <returns>A new <see cref="Result{T, TErr}"/> transformed</returns>
    public Result<T, TErr> Map<T>(Func<TOk, T> map)
    {
        if (_isOk)
        {
            return Result<T, TErr>.Ok(map(_ok!));
        }

        return Result<T, TErr>.Err(_err!);
    }

    /// <summary>
    /// Returns a new <see cref="Result{TOk, T}"/> with the value corresponding to <typeparamref name="TErr"/> transformed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="map">The transformation to apply</param>
    /// <returns>A new <see cref="Result{TOk, T}"/> transformed</returns>
    public Result<TOk, T> MapErr<T>(Func<TErr, T> map)
    {
        if (_isOk)
        {
            return Result<TOk, T>.Ok(_ok!);
        }

        return Result<TOk, T>.Err(map(_err!));
    }

    /// <summary>
    /// Converts the result into a <see cref="Maybe{TOk}"/>
    /// </summary>
    /// <returns><see cref="Maybe.Some{TOk}(TOk)" /> if the result is successful, <see cref="Maybe{TOk}.None"/> otherwise</returns>
    public Maybe<TOk> Ok() => _isOk ? Maybe.Some(_ok!) : Maybe<TOk>.None();

    /// <summary>
    /// Returns <see langword="true" /> and assigns <paramref name="err"/> when <see cref="Result{TOk, TErr}"/> is an Error, <see langword="false"/> otherwise.
    /// </summary>
    /// <param name="err">The error contained in this instance, or <see langword="default"/> if no value present</param>
    /// <returns><see langword="true" /> when a value exists, <see langword="false" /> otherwise</returns>
    public bool IsErr([NotNullWhen(true)]out TErr? err)
    {
        err = _err;

        return !_isOk;
    }

    /// <summary>
    /// Returns <see langword="true" /> and assigns <paramref name="err"/> when <see cref="Result{TOk, TErr}"/> is an Error, <see langword="false"/> and assigns <paramref name="ok"/> otherwise.
    /// </summary>
    /// <param name="ok">The ok parameter contained in this instance, or <see langword="default"/> if no value present</param>
    /// <param name="err">The error contained in this instance, or <see langword="default"/> if no value present</param>
    /// <returns><see langword="true" /> when a value exists, <see langword="false" /> otherwise</returns>
    public bool IsErr([NotNullWhen(false)]out TOk? ok,  [NotNullWhen(true)] out TErr? err)
    {
        err = _err;
        ok = _ok;

        return !_isOk;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Result{TOk, TErr}"/> as a successful operation
    /// </summary>
    /// <param name="ok">The result of the successful operation</param>
    /// <returns>An instance of <see cref="Result{TOk, TErr}"/> as a successful operation</returns>
    public static Result<TOk, TErr> Ok(TOk ok) => new(ok);

    /// <summary>
    /// Creates a new instance of <see cref="Result{TOk, TErr}"/> as a failed operation
    /// </summary>
    /// <param name="err">The result of the failed operation</param>
    /// <returns>An instance of <see cref="Result{TOk, TErr}"/> as a failed operation</returns>
    public static Result<TOk, TErr> Err(TErr err) => new(err);
}