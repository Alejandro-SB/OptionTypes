namespace OptionTypes;

public sealed class Result<TOk, TErr>
{
    private readonly bool _isOk;

    private readonly TOk? _ok;
    private readonly TErr? _err;

    private Result(TOk ok)
    {
        _ok = ok;
        _isOk = true;
    }

    private Result(TErr err)
    {
        _err = err;
        _isOk = false;
    }

    public TResult Match<TResult>(Func<TOk, TResult> ok, Func<TErr, TResult> err) => _isOk ? ok(_ok!) : err(_err!);

    public static Result<TOk, TErr> Ok(TOk ok) => new(ok);
    public static Result<TOk, TErr> Error(TErr err) => new(err);
}
