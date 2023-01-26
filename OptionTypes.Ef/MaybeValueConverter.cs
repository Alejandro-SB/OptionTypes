using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace OptionTypes.Ef;

internal sealed class MaybeValueConverter<T> : ValueConverter<Maybe<T>, T?>
    where T : class
{
    public MaybeValueConverter()
#pragma warning disable EF1001 // Internal EF Core API usage.
        : base(v => v.ValueOr(() => null!), v => Maybe.FromValue(v), true)
#pragma warning restore EF1001 // Internal EF Core API usage.
    {
    }
}

internal sealed class MaybeStructValueConverter<T> : ValueConverter<Maybe<T>, T?>
    where T : struct
{
    public MaybeStructValueConverter()
#pragma warning disable EF1001 // Internal EF Core API usage.
        : base(v => v.Match<T?>(v => v, () => null), v => v == null ? Maybe<T>.None() : Maybe.FromValue(v), true)
#pragma warning restore EF1001 // Internal EF Core API usage.
    {
    }
}