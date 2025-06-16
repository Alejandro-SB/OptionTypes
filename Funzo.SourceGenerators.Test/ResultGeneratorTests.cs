namespace Funzo.SourceGenerators.Test;

public class ResultGeneratorTests
{
    [Fact]
    public void Generates_Implicit_Conversors_And_Constructors()
    {
        TestResult okImplicit = Unit.Default;
        TestResult errImplicit = "FAILURE";

        Assert.False(okImplicit.IsErr(out _));
        Assert.True(errImplicit.IsErr(out _));
    }

    [Fact]
    public void Generates_Ok_And_Err_Methods()
    {
        var ok = TestResult.Ok(Unit.Default);
        var err = TestResult.Err("FAIL");

        Assert.False(ok.IsErr(out _));
        Assert.True(err.IsErr(out _));
    }

    [Fact]
    public void Generates_Result_With_Just_Err_Type()
    {
        var ok = TestUnitResult.Ok();
        var fail = TestUnitResult.Err("FAILURE");

        Assert.False(ok.IsErr(out _));
        Assert.True(fail.IsErr(out _));
    }
}

[Result]
public partial class TestResult : Result<Unit, string>;

[Result]
public partial class TestUnitResult : Result<string>;