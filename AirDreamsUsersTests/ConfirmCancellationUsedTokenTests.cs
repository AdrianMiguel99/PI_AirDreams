using AirDreams.API.Repositories.Interfaces;
using AirDreams.API.Services;
using Moq;
using NUnit.Framework;

namespace AirDreamsUsersTests;

[TestFixture]
public class ConfirmCancellationUsedTokenTests
{
    [Test]
    public void ConfirmCancellationAsync_WhenTokenIsUsed_ShouldThrowInvalidOperationException()
    {
        var repositoryMock = new Mock<ICancellationRepository>();
        var emailServiceMock = new Mock<IEmailService>();

        var service = new CancellationService(
            repositoryMock.Object,
            emailServiceMock.Object
        );

        var token = "used-token";

        repositoryMock
            .Setup(r => r.GetCancellationTokenAsync(token))
            .ReturnsAsync(("TXN-TEST", true, DateTime.UtcNow.AddHours(1)));

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.ConfirmCancellationAsync(token)
        );

        repositoryMock.Verify(
            r => r.CancelItineraryAsync(It.IsAny<string>()),
            Times.Never
        );
    }
}