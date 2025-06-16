namespace Funzo.SourceGenerators.Test;
public class UnionGeneratorTests
{
    [Fact]
    public void Generates_Implicit_Conversors()
    {
        TestUnion u1 = "TEST";
        TestUnion u2 = 3;
        TestUnion u3 = DateTime.UtcNow;
        TestUnion u4 = DateTimeOffset.UtcNow;

        Assert.True(u1.Is<string>(out _));
        Assert.True(u2.Is<int>(out _));
        Assert.True(u3.Is<DateTime>(out _));
        Assert.True(u4.Is<DateTimeOffset>(out _));
    }

    [Fact]
    public void Generates_Constructors()
    {
        TestUnion u1 = new("TEST");
        TestUnion u2 = new(3);
        TestUnion u3 = new(DateTime.UtcNow);
        TestUnion u4 = new(DateTimeOffset.UtcNow);

        Assert.True(u1.Is<string>(out _));
        Assert.True(u2.Is<int>(out _));
        Assert.True(u3.Is<DateTime>(out _));
        Assert.True(u4.Is<DateTimeOffset>(out _));
    }
}


[Union]
public partial class TestUnion : Union<string, int, DateTime, DateTimeOffset>;