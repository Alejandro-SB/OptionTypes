using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptionTypes.Fody;
using Xunit.Sdk;

namespace OptionTypes.Test;

public class ResultOnReturnTests
{
    [Fact]
    public void OnReturn_Exists_Method_When_Err()
    {
        static Result<int, string> TestMethod()
        {
            Result<int, string>.Err("Should exit").OrReturn();

            return Result<int, string>.Ok(1);
        }

        var result = TestMethod();

        Assert.True(result.IsErr(out _));
    }

    [Fact]
    public void OnReturn_Continues_Execution_When_Ok()
    {
        static Result<int,string> TestMethod()
        {
            Result<int, string>.Ok(1).OrReturn();

            return Result<int, string>.Err("Should not reach this");
        }

        var result = TestMethod();

        Assert.False(result.IsErr(out _));
    }
}
