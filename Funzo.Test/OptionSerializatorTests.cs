using Funzo.Serialization;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Funzo.Test;
public class OptionSerializatorTests
{
    private readonly OptionConverterFactory _optionConverterFactory = new();
    
    [Fact]
    public void Can_Convert_From_Option()
    {
        var option = Option.Some("HAS_VALUE");

        var canConvert = _optionConverterFactory.CanConvert(option.GetType());

        Assert.True(canConvert);
    }

    [Fact]
    public void String_Serializer_Serializes_Correctly_When_Has_Some_Value()
    {
        var text = "Option inner value";
        var expected = @$"{{""HasValue"":true,""Value"":""{text}""}}";
        var option = Option.Some(text);


        var converter = _optionConverterFactory.CreateConverter(option.GetType(), JsonSerializerOptions.Default)!;

        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.Converters.Add(converter);

        var result = JsonSerializer.Serialize(option, serializerOptions);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void String_Serializer_Serializes_Correctly_When_Has_None()
    {
        var expected = @$"{{""HasValue"":false}}";
        var option = Option<string>.None();

        var converter = _optionConverterFactory.CreateConverter(option.GetType(), JsonSerializerOptions.Default)!;

        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.Converters.Add(converter);

        var result = JsonSerializer.Serialize(option, serializerOptions);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Serializes_Complex_Class()
    {
        var number = 4;
        var text = "M";
        var dateSerialized = "2025-02-11T02:22:22+01:00";
        var date = DateTimeOffset.Parse(dateSerialized);

        var expected = $@"{{""HasValue"":true,""Value"":{{""Number"":{number},""Text"":""{text}"",""Date"":""{dateSerialized}""}}}}";

        var option = Option.Some(new ComplexClass
        {
            Number = number,
            Text = text,
            Date = date
        });

        var converter = _optionConverterFactory.CreateConverter(option.GetType(), JsonSerializerOptions.Default)!;

        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.Converters.Add(converter);

        var result = JsonSerializer.Serialize(option, serializerOptions);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Deserializes_Correctly_When_Some()
    {
        var number = 4;
        var text = "M";
        var dateSerialized = "2025-02-11T02:22:22+01:00";
        var date = DateTimeOffset.Parse(dateSerialized);

        var serializedValue = $@"{{""HasValue"":true,""Value"":{{""Number"":{number},""Text"":""{text}"",""Date"":""{dateSerialized}""}}}}";

        var converter = _optionConverterFactory.CreateConverter(typeof(Option<ComplexClass>), JsonSerializerOptions.Default)!;

        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.Converters.Add(converter);

        var result = JsonSerializer.Deserialize<Option<ComplexClass>>(serializedValue, serializerOptions);

        var expected = new ComplexClass
        {
            Number = number,
            Text = text,
            Date = date
        };

        Assert.NotNull(result);
        var isSome = result.IsSome(out var innerOption);
        Assert.True(isSome);
        Assert.Equal(expected, innerOption);
    }

    [Fact]
    public void Deserializes_Correctly_When_None()
    {
        var text = @$"{{""HasValue"":false}}";

        var converter = _optionConverterFactory.CreateConverter(typeof(Option<ComplexClass>), JsonSerializerOptions.Default)!;

        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.Converters.Add(converter);

        var option = JsonSerializer.Deserialize<Option<ComplexClass>>(text, serializerOptions);

        Assert.NotNull(option);

        var isSome = option.IsSome(out var innerValue);

        Assert.False(isSome);
        Assert.Null(innerValue);
    }

    private class ComplexClass : IEquatable<ComplexClass>
    {
        public int Number { get; set; }
        public string Text { get; set; } = null!;
        public DateTimeOffset Date { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ComplexClass cc && cc.Equals(this);
        }

        public bool Equals(ComplexClass? other)
        {
            return other is not null && other.Date.Equals(Date) && other.Number.Equals(Number) && other.Text.Equals(Text);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Number, Text, Date);
        }
    }
}
