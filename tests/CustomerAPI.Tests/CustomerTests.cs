namespace CustomerAPI.Tests;

public class CustomerTests
{
    [Fact]
    public void Customer_Name_Should_Not_Be_Empty()
    {
        var name = "Ravi Kumar";

        Assert.False(string.IsNullOrWhiteSpace(name));
    }

    [Fact]
    public void Customer_Email_Should_Contain_At()
    {
        var email = "ravi@test.com";

        Assert.Contains("@", email);
    }
}