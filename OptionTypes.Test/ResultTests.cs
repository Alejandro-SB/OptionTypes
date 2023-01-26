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
        var instance = Result<int, int>.Error(value);

        instance.Match(Throw<int>("Match executed ok function"), Pass);
    }

    private static int OkOperation(int value) => value + 1;
    private static int ErrOperation(int value) => value - 1;
    private static void Pass<T>(T _) { }
    private static Action<T> Throw<T>(string message) => (T _) => throw new Exception(message);

}
