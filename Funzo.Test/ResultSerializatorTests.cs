using Funzo.Serialization;
using System;
using System.Text.Json;

namespace Funzo.Test;
public class ResultSerializatorTests
{
    private readonly ResultConverterFactory _resultConverterFactory = new();

    [Fact]
    public void Result_Serializer_Serializes_Ok()
    {
        var num = 4;
        var result = Result<int, string>.Ok(num);
        var converter = _resultConverterFactory.CreateConverter(result.GetType(), JsonSerializerOptions.Default)!;

        var options = new JsonSerializerOptions();
        options.Converters.Add(converter);

        var serialized = JsonSerializer.Serialize(result, options);
        var expected = $@"{{""IsOk"":true,""Ok"":{num}}}";
        
        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void Result_Serializer_Serializes_Err()
    {
        var text = "ERROR";
        var result = Result<int, string>.Err(text);
        var converter = _resultConverterFactory.CreateConverter(result.GetType(), JsonSerializerOptions.Default)!;

        var options = new JsonSerializerOptions();
        options.Converters.Add(converter);

        var serialized = JsonSerializer.Serialize(result, options);
        var expected = $@"{{""IsOk"":false,""Err"":""{text}""}}";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void Result_Serializer_Deserializes_Err_Result()
    {
        var text = "ERROR";
        var converter = _resultConverterFactory.CreateConverter(typeof(Result<int, string>), JsonSerializerOptions.Default)!;

        var options = new JsonSerializerOptions();
        options.Converters.Add(converter);

        var serialized = $@"{{""IsOk"":false,""Err"":""{text}""}}";
        var result = JsonSerializer.Deserialize<Result<int,string>>(serialized, options);

        Assert.NotNull(result);
        var isErr = result.IsErr(out var ok, out var err);

        Assert.True(isErr);
        Assert.Equal(text, err);
    }

    [Fact]
    public void Result_Serializer_Deserializes_Ok_Result()
    {
        var number = 73;
        var text = $@"{{""IsOk"":true,""Ok"":{number}}}";
        var converter = _resultConverterFactory.CreateConverter(typeof(Result<int, string>), JsonSerializerOptions.Default)!;

        var options = new JsonSerializerOptions();
        options.Converters.Add(converter);

        var result = JsonSerializer.Deserialize<Result<int, string>>(text, options);

        Assert.NotNull(result);
        var isErr = result.IsErr(out var ok, out var err);

        Assert.False(isErr);
        Assert.Equal(number, ok);
    }
}
