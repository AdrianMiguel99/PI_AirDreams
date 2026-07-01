using AirDreams.API.Repositories.Interfaces;
using AirDreams.API.Services;
using Moq;
using NUnit.Framework;

namespace AirDreamsUsersTests;



[TestFixture]
public class CancelReservationAlreadyCancelledTests
{
    [Test]
    public void CancelReservationAsync_WhenReservationIsAlreadyCancelled_ShouldThrowInvalidOperationException()
    {
        var repositoryMock = new Mock<ICancellationRepository>();
        var emailServiceMock = new Mock<IEmailService>();

        var service = new CancellationService(
            repositoryMock.Object,
            emailServiceMock.Object
        );

        var transactionId = "TXN-CANCELLED";

        repositoryMock
            .Setup(r => r.ItineraryExistsAsync(transactionId))
            .ReturnsAsync(true);

        repositoryMock
            .Setup(r => r.IsItineraryCancelledAsync(transactionId))
            .ReturnsAsync(true);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CancelReservationAsync(transactionId)
        );

        repositoryMock.Verify(
            r => r.CancelItineraryAsync(It.IsAny<string>()),
            Times.Never
        );
    }
}