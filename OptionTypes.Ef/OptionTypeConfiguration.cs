using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace OptionTypes.Ef;

/// <summary>
/// Helper class to configure option types
/// </summary>
public static class OptionTypeConfiguration
{
    /// <summary>
    /// Adds the required <see cref="ValueConverter"/> implementations to convert between <see langword="null" /> in database and <see cref="Maybe{T}"/> in code and viceversa
    /// </summary>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/> instance</param>
    public static void AddOptionTypeConverters(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                var type = property.ClrType;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Maybe<>))
                {
                    var innerType = type.GetGenericArguments()[0];

                    if (innerType.IsClass)
                    {
                        var converterType = typeof(MaybeValueConverter<>).MakeGenericType(innerType);
                        property.SetValueConverter(converterType);
                    }
                    else
                    {
                        var converterType = typeof(MaybeStructValueConverter<>).MakeGenericType(innerType);
                        property.SetValueConverter(converterType);
                    }

                    property.IsNullable = true;
                }
            }
        }
    }
}
