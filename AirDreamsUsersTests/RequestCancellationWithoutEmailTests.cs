using AirDreams.API.Repositories.Interfaces;
using AirDreams.API.Services;
using Moq;
using NUnit.Framework;

namespace AirDreamsUsersTests;

[TestFixture]
public class RequestCancellationWithoutEmailTests
{
    [Test]
    public void RequestCancellationAsync_WhenEmailDoesNotExist_ShouldThrowInvalidOperationException()
    {
        var repositoryMock = new Mock<ICancellationRepository>();
        var emailServiceMock = new Mock<IEmailService>();

        var service = new CancellationService(
            repositoryMock.Object,
            emailServiceMock.Object
        );

        var transactionId = "TXN-TEST";

        repositoryMock
            .Setup(r => r.ItineraryExistsAsync(transactionId))
            .ReturnsAsync(true);

        repositoryMock
            .Setup(r => r.GetBuyerEmailByTransactionIdAsync(transactionId))
            .ReturnsAsync((string?)null);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.RequestCancellationAsync(transactionId)
        );

        emailServiceMock.Verify(
            e => e.SendCancellationEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>()
            ),
            Times.Never
        );
    }
}