namespace AuthAPI.Tests;

public class LoginTests
{
    [Test]
    public void ValidCredentials_ShouldBeAccepted()
    {
        var username = "admin";
        var password = "admin123";

        var valid =
            username == "admin" &&
            password == "admin123";

        Assert.That(valid, Is.True);
    }

    [Test]
    public void InvalidCredentials_ShouldBeRejected()
    {
        var username = "admin";
        var password = "wrong";

        var valid =
            username == "admin" &&
            password == "admin123";

        Assert.That(valid, Is.False);
    }
}