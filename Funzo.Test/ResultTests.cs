using Funzo;
using System.Text.Json;

namespace Funzo.Test;
public class ResultTests
{
    [Fact]
    public void Ok_Creates_A_Successful_Operation_Result()
    {
        var value = 7;
        var instance = Result<int, int>.Ok(7);
        var expected = OkOperation(value);
        var result = instance.Match(OkOperation, ErrOperation);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Error_Creates_A_Failed_Operation_Result()
    {
        var value = 7;
        var instance = Result<int, int>.Err(7);
        var expected = ErrOperation(value);
        var result = instance.Match(OkOperation, ErrOperation);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Match_When_Result_Is_Ok_Executes_Ok_Action()
    {
        var value = 13;
        var instance = Result<int, int>.Ok(value);

        instance.Match(Pass, Throw<int>("Match executed error function"));
    }

    [Fact]
    public void Match_When_Result_Is_Error_Executes_Error_Action()
    {
        var value = 13;
        var instance = Result<int, int>.Err(value);

        instance.Match(Throw<int>("Match executed ok function"), Pass);
    }

    [Fact]
    public void Map_Returns_New_Error_With_Ok_Value_Mapped()
    {
        var expected = 15;
        var initial = 0;
        var result = Result<int, int>.Ok(initial);

        var mappedResult = result.Map(_ => expected);

        mappedResult.Match(v => Assert.Equal(expected, v), Throw<int>("Match executed on error"));
    }

    [Fact]
    public void MapErr_Returns_New_Error_With_Err_Value_Mapped()
    {
        var expected = 15;
        var initial = 0;
        var result = Result<int, int>.Err(initial);

        var mappedResult = result.MapErr(_ => expected);

        mappedResult.Match(Throw<int>("Match executed on ok"), v => Assert.Equal(expected, v));
    }

    [Fact]
    public void IsErr_Returns_True_When_There_Is_An_Error()
    {
        var expectedText = "ERROR";
        var result = Result<string, string>.Err(expectedText);

        Assert.True(result.IsErr(out var errorText));

        Assert.Equal(expectedText, errorText);
    }

    [Fact]
    public void IsErr_Returns_False_When_There_Is_No_Error()
    {
        var expectedText = "OK";
        var result = Result<string, string>.Ok(expectedText);

        Assert.False(result.IsErr(out var ok, out var errorText));
        Assert.Null(errorText);
        Assert.Equal(expectedText, ok);
    }

    [Fact]
    public void Inspect_Is_Called_When_Result_Is_Ok()
    {
        var result = Result<string, string>.Ok("");

        var test = "TEST";

        result.Inspect(_ => test = null);

        Assert.Null(test);
    }

    [Fact]
    public void Inspect_Is_Not_Called_When_Result_Is_Err()
    {
        var result = Result<string, string>.Err("");

        var test = "TEST";

        result.Inspect(_ => test = null);

        Assert.NotNull(test);
    }

    [Fact]
    public async Task Inspect_Is_Called_Async_When_Result_Is_Ok()
    {
        var result = Result<string, string>.Ok("TEST STRING");

        var test = "TEST";

        await result.Inspect(_ =>
        {
            test = null;
            return Task.CompletedTask;
        });

        Assert.Null(test);
    }

    [Fact]
    public async Task Inspect_Is_Not_Called_Async_When_Result_Is_Err()
    {
        var result = Result<string, string>.Err("");

        var test = "TEST";

        await result.Inspect(_ =>
        {
            test = null;
            return Task.CompletedTask;
        });

        Assert.NotNull(test);
    }

    private static int OkOperation(int value) => value + 1;
    private static int ErrOperation(int value) => value - 1;
    private static void Pass<T>(T _) { }
    private static Action<T> Throw<T>(string message) => (_) => throw new Exception(message);

}
