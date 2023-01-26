namespace OptionTypes.Test;
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
        var instance = Result<int, int>.Error(7);
        var expected = ErrOperation(value);
        var result = instance.Match(OkOperation, ErrOperation);

        Assert.Equal(expected, result);
    }

    private static int OkOperation(int value) => value + 1;
    private static int ErrOperation(int value) => value - 1;
}
