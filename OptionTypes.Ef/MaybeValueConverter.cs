using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace OptionTypes.Ef;

public class MaybeValueConverter<T> : ValueConverter<Maybe<T>, T>
{
    public MaybeValueConverter() 
        : base(v => v.ValueOr(null!), v => Maybe.FromValue(v))
    {
    }
}
