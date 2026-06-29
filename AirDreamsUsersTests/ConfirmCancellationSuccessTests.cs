using AirDreams.API.Repositories.Interfaces;
using AirDreams.API.Services;
using Moq;
using NUnit.Framework;

namespace AirDreamsUsersTests;

[TestFixture]
public class ConfirmCancellationSuccessTests
{
    [Test]
    public async Task ConfirmCancellationAsync_WhenTokenIsValid_ShouldCancelReservationAndMarkTokenAsUsed()
    {
        var repositoryMock = new Mock<ICancellationRepository>();
        var emailServiceMock = new Mock<IEmailService>();

        var service = new CancellationService(
            repositoryMock.Object,
            emailServiceMock.Object
        );

        var token = "valid-token";
        var transactionId = "TXN-TEST";

        repositoryMock
            .Setup(r => r.GetCancellationTokenAsync(token))
            .ReturnsAsync((transactionId, false, DateTime.UtcNow.AddHours(1)));

        repositoryMock
            .Setup(r => r.ItineraryExistsAsync(transactionId))
            .ReturnsAsync(true);

        repositoryMock
            .Setup(r => r.IsItineraryCancelledAsync(transactionId))
            .ReturnsAsync(false);

        await service.ConfirmCancellationAsync(token);

        repositoryMock.Verify(
            r => r.CancelItineraryAsync(transactionId),
            Times.Once
        );

        repositoryMock.Verify(
            r => r.MarkCancellationTokenAsUsedAsync(token),
            Times.Once
        );
    }
}