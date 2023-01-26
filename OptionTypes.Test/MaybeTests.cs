namespace OptionTypes.Test;

public class MaybeTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(15)]
    public void Some_Creates_An_Instance_With_Value(int value)
    {
        var instance = Maybe<int>.Some(value);

        var instanceValue = instance.ValueOr(() => throw new Exception("Has no value"));

        Assert.Equal(value, instanceValue);
    }

    [Fact]
    public void None_Creates_Empty_Instance()
    {
        var instance = Maybe<int>.None();

        Assert.Equal(Maybe<int>.None(), instance);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(14)]
    [InlineData(-7)]
    public void Map_Executes_When_Maybe_Has_Value(int value)
    {
        var instance = Maybe<int>.Some(value);

        var expected = MapOperation(value);
        var result = instance.Map(MapOperation);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Map_Omits_Function_When_Maybe_Has_No_Value()
    {
        var instance = Maybe<int>.None();

        var result = instance.Map(MapOperation);

        Assert.Equal(Maybe<int>.None(), result);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(0)]
    [InlineData(-7)]
    public void Bind_Executes_Function_When_Maybe_Has_Value(int value)
    {
        var instance = Maybe<int>.Some(value);

        var expected = BindOperation(value);
        var result = instance.Map(BindOperation);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Bind_Omits_Function_When_Maybe_Has_No_Value()
    {
        var instance = Maybe<int>.None();
        var result = instance.Map(BindOperation);

        Assert.Equal(Maybe<int>.None(), result);
    }

    [Theory]
    [InlineData(5)]

    [InlineData(0)]
    [InlineData(-100)]
    public void Match_Executes_Some_Function_When_Maybe_Has_Value(int value)
    {
        var instance = Maybe<int>.Some(value);

        var expected = MapOperation(value);
        var result = instance.Match(MapOperation, Throw<int>("Match should not execute None case"));

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(5)]

    [InlineData(0)]
    [InlineData(-100)]
    public void Match_Executes_None_Function_When_Maybe_Has_No_Value(int value)
    {
        var instance = Maybe<int>.None();

        var result = instance.Match(Throw<int, int>("Match should not execute Some case"), () => value);

        Assert.Equal(value, result);
    }

    [Fact]
    public void ValueOr_Returns_Instance_Value_When_Maybe_Has_Some_Value()
    {
        var expected = 3;
        var instance = Maybe<int>.Some(expected);
        var result = instance.ValueOr(7);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ValueOr_Returns_Fallback_Value_When_Maybe_Has_No_Value()
    {
        var expected = 7;
        var instance = Maybe<int>.None();
        var result = instance.ValueOr(expected);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ValueOr_Returns_Instance_Value_Omitting_Function_When_Maybe_Has_Some_Value()
    {
        var expected = 3;
        var instance = Maybe<int>.Some(expected);
        var result = instance.ValueOr(() => 7);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ValueOr_Executes_Provider_Function_When_Maybe_Has_No_Value()
    {
        var expected = 3;
        var instance = Maybe<int>.None();
        var result = instance.ValueOr(() => expected);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Some_Creates_A_Maybe_Instance_With_Some_Value()
    {
        var expected = 5;
        var instance = Maybe.Some(expected);

        var result = instance.ValueOr(Throw<int>("Instance should have value"));

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(7)]
    [InlineData(null)]
    [InlineData(-145)]
    [InlineData(0)]
    public void FromValue_Creates_A_Maybe_Instance_For_Structs_Depending_On_Value_Supplied(int? value)
    {
        var instance = Maybe.FromValue(value);
        var expected = value is { } v
            ? Maybe<int>.Some(v)
            : Maybe<int>.None();

        Assert.Equal(expected, instance);
    }

    [Theory]
    [InlineData("OK")]
    [InlineData(null)]
    [InlineData("HMMM")]
    public void FromValue_Creates_A_Maybe_Instance_For_Classes_Depending_On_Value_Supplied(string? value)
    {
        var instance = Maybe.FromValue(value);
        var expected = value is not null
            ? Maybe<string>.Some(value)
            : Maybe<string>.None();

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

            return Maybe<int>.Some(value);
        });

        var expected = MapOperation(value);
        var result = await task.Map(MapOperation);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(78)]
    public async Task Map_Returning_Maybe_In_Task_Creates_A_Continuation_For_Completed_Tasks(int value)
    {
        var task = Task.Run(() =>
        {
            return Maybe<int>.Some(value);
        });

        var expected = BindOperation(value);
        var result = await task.Map(BindOperation);

        Assert.Equal(expected, result);
    }

    private static int MapOperation(int v) => v + 1;
    private static Maybe<int> BindOperation(int v) => Maybe<int>.Some(v + 1);
    private static Func<T> Throw<T>(string message) => () => throw new Exception(message);
    private static Func<T, U> Throw<T, U>(string message) => _ => throw new Exception(message);
}