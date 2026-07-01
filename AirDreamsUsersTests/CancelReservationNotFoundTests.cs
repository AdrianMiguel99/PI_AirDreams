using AirDreams.API.Repositories.Interfaces;
using AirDreams.API.Services;
using Moq;
using NUnit.Framework;

namespace AirDreamsUsersTests;

[TestFixture]
public class CancelReservationNotFoundTests
{
    [Test]
    public void CancelReservationAsync_WhenReservationDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        var repositoryMock = new Mock<ICancellationRepository>();
        var emailServiceMock = new Mock<IEmailService>();

        var service = new CancellationService(
            repositoryMock.Object,
            emailServiceMock.Object
        );
        var transactionId = "TXN-NOT-FOUND";

        repositoryMock
            .Setup(r => r.ItineraryExistsAsync(transactionId))
            .ReturnsAsync(false);

        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await service.CancelReservationAsync(transactionId)
        );

        repositoryMock.Verify(
            r => r.CancelItineraryAsync(It.IsAny<string>()),
            Times.Never
        );
    }
}