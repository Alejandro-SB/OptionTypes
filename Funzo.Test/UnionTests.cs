using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Funzo.Test;

public class UnionTests
{
    [Fact]
    public void Union_Type_Can_Be_Constructed_From_Base_Types()
    {
        _ = new Union<DateTime, string>(DateTime.Now);
        _ = new Union<DateTime, string>("2025-02-11");
    }

    [Fact]
    public void Is_Returns_True_When_Type_Of_Union_Matches()
    {
        var expected = "text";
        Union<string, int> union = expected;

        var isString = union.Is<string>(out var result);

        Assert.True(isString);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Is_Returns_False_When_Type_Of_Union_Does_Not_Match()
    {
        var expected = "text";
        Union<string, int> union = expected;

        var isString = union.Is<int>(out _);

        Assert.False(isString);
    }

    [Fact]
    public void Switch_Only_Calls_Matching_Type_Function()
    {
        Union<DateTime, string, int, float> union = 45;

        union.Switch(ThrowingAction, ThrowingAction, _ => { }, ThrowingAction);
    }

    [Fact]
    public async Task SwitchAsync_Only_Calls_Matching_Type_Function()
    {
        Union<DateTime, string, int, float> union = 45.0f;

        await union.SwitchAsync(ThrowingActionAsync, ThrowingActionAsync, ThrowingActionAsync, _ => Task.CompletedTask);
    }

    [Fact]
    public void Match_Only_Calls_Matching_Type_Function()
    {
        var expected = "2025-01-01";
        Union<DayOfWeek, DateTime, DateTimeOffset, string> union = expected;

        var result = union.Match(ThrowingFunc, ThrowingFunc, ThrowingFunc, x => x);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Union_Type_Equals_Compares_Internal_Values()
    {
        Union<string, int> first = "text";
        Union<string, int> second = "text";
        Union<string, float> third = "text";

        Assert.True(first.Equals(second));
        Assert.True(third.Equals(first));
    }

    [DoesNotReturn]
    private static void ThrowingAction<T>(T _)
    {
        throw new Exception();
    }

    [DoesNotReturn]
    private static Task ThrowingActionAsync<T>(T _)
    {
        throw new Exception();
    }

    [DoesNotReturn]
    private static object ThrowingFunc<T>(T _)
    {
        throw new Exception();
    }
}
