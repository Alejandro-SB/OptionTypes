using System.Diagnostics;

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
    /// Converts the result into a <see cref="Maybe{TOk}"/>
    /// </summary>
    /// <returns><see cref="Maybe.Some{TOk}(TOk)" /> if the result is successful, <see cref="Maybe{TOk}.None"/> otherwise</returns>
    public Maybe<TOk> Ok() => _isOk ? Maybe.Some(_ok!) : Maybe<TOk>.None();

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
    public static Result<TOk, TErr> Error(TErr err) => new(err);
}