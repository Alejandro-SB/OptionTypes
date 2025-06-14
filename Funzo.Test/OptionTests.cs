using Funzo;
using System.Text.Json;

namespace Funzo.Test;

public class OptionTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(15)]
    public void Some_Creates_An_Instance_With_Value(int value)
    {
        var instance = Option.Some(value);

        var instanceValue = instance.Unwrap();

        Assert.Equal(value, instanceValue);
    }

    [Fact]
    public void None_Creates_Empty_Instance()
    {
        var instance = Option<int>.None();

        Assert.Equal(Option<int>.None(), instance);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(14)]
    [InlineData(-7)]
    public void Map_Executes_When_Option_Has_Value(int value)
    {
        var instance = Option.Some(value);

        var expected = MapOperation(value);
        var result = instance.Map(MapOperation);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Map_Omits_Function_When_Option_Has_No_Value()
    {
        var instance = Option<int>.None();

        var result = instance.Map(MapOperation);

        Assert.Equal(Option<int>.None(), result);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(0)]
    [InlineData(-7)]
    public void Bind_Executes_Function_When_Option_Has_Value(int value)
    {
        var instance = Option.Some(value);

        var expected = BindOperation(value);
        var result = instance.Map(BindOperation);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Bind_Omits_Function_When_Option_Has_No_Value()
    {
        var instance = Option<int>.None();
        var result = instance.Map(BindOperation);

        Assert.Equal(Option<int>.None(), result);
    }

    [Theory]
    [InlineData(5)]

    [InlineData(0)]
    [InlineData(-100)]
    public void Match_Executes_Some_Function_When_Option_Has_Value(int value)
    {
        var instance = Option.Some(value);

        var expected = MapOperation(value);
        var result = instance.Match(MapOperation, Throw<int>("Match should not execute None case"));

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(5)]

    [InlineData(0)]
    [InlineData(-100)]
    public void Match_Executes_None_Function_When_Option_Has_No_Value(int value)
    {
        var instance = Option<int>.None();

        var result = instance.Match(Throw<int, int>("Match should not execute Some case"), () => value);

        Assert.Equal(value, result);
    }

    [Fact]
    public void ValueOr_Returns_Instance_Value_When_Option_Has_Some_Value()
    {
        var expected = 3;
        var instance = Option.Some(expected);
        var result = instance.ValueOr(7);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ValueOr_Returns_Fallback_Value_When_Option_Has_No_Value()
    {
        var expected = 7;
        var instance = Option<int>.None();
        var result = instance.ValueOr(expected);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(7)]
    [InlineData(null)]
    [InlineData(-145)]
    [InlineData(0)]
    public void FromValue_Creates_A_Option_Instance_For_Structs_Depending_On_Value_Supplied(int? value)
    {
        var instance = Option.FromValue(value);
        var expected = value is { } v
            ? Option.Some(v)
            : Option<int>.None();

        Assert.Equal(expected, instance);
    }

    [Theory]
    [InlineData("OK")]
    [InlineData(null)]
    [InlineData("HMMM")]
    public void FromValue_Creates_A_Option_Instance_For_Classes_Depending_On_Value_Supplied(string? value)
    {
        var instance = Option.FromValue(value);
        var expected = value is not null
            ? Option.Some(value)
            : Option<string>.None();

        Assert.Equal(expected, instance);
    }

    [Theory]
    [InlineData(7)]
    [InlineData(0)]
    public async Task Map_In_Task_Creates_A_Continuation_For_Completed_Tasks(int value)
    {
        var task = Task.Run(() =>
        {
            Task.Delay(10);

            return Option.Some(value);
        });

        var expected = MapOperation(value);
        var result = await task.Map(MapOperation);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(78)]
    public async Task Map_Returning_Option_In_Task_Creates_A_Continuation_For_Completed_Tasks(int value)
    {
        var task = Task.Run(() =>
        {
            return Option.Some(value);
        });

        var expected = BindOperation(value);
        var result = await task.Map(BindOperation);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsSome_Returns_False_When_There_Is_No_Value()
    {
        var none = Option<int>.None();

        Assert.False(none.IsSome(out _));
    }

    [Fact]
    public void IsSome_Returns_True_When_There_Is_Value()
    {
        var expectedValue = 1;
        var some = Option.Some(expectedValue);

        Assert.True(some.IsSome(out var value));
        Assert.Equal(expectedValue, value);
    }

    [Fact]
    public void Inspect_Is_Called_When_Value_Is_Some()
    {
        var value = Option.Some(3);
        var test = "NOT NULL";

        value.Inspect(_ => test = null);

        Assert.Null(test);
    }

    [Fact]
    public async Task Inspect_Is_Called_Async_When_Some()
    {
        var value = Option.Some(3);
        var test = "NOT NULL";

        await value.Inspect(async x =>
        {
            test = null;
            await Task.CompletedTask;
        });

        Assert.Null(test);
    }

    [Fact]
    public void Inspect_Is_Not_Called_When_Value_Is_None()
    {
        var value = Option<int>.None();
        static void throwF(int _) => throw new Exception();

        value.Inspect(throwF);
    }

    [Fact]
    public async Task Inspect_Is_Not_Called_Async_When_Value_Is_None()
    {
        var value = Option<int>.None();
        static Task throwF(int _) => throw new Exception();

        await value.Inspect(throwF);
    }

    [Fact]
    public void ValueOrDefault_Returns_Value_When_Some()
    {
        var expected = 34;

        var some = Option.Some(expected);

        var result = some.ValueOrDefault();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ValueOrDefault_Returns_Default_When_None()
    {
        var none = Option<string>.None();

        var result = none.ValueOrDefault();

        Assert.Null(result);
    }

    private static int MapOperation(int v) => v + 1;
    private static Option<int> BindOperation(int v) => Option.Some(v + 1);
    private static Func<T> Throw<T>(string message) => () => throw new Exception(message);
    private static Func<T, U> Throw<T, U>(string message) => _ => throw new Exception(message);
}