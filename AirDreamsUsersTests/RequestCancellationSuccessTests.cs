using AirDreams.API.Repositories.Interfaces;
using AirDreams.API.Services;
using Moq;
using NUnit.Framework;

namespace AirDreamsUsersTests;

[TestFixture]
public class RequestCancellationSuccessTests
{
    [Test]
    public async Task RequestCancellationAsync_WhenReservationExists_ShouldSaveTokenAndSendEmail()
    {
        var repositoryMock = new Mock<ICancellationRepository>();
        var emailServiceMock = new Mock<IEmailService>();

        var service = new CancellationService(
            repositoryMock.Object,
            emailServiceMock.Object
        );

        var transactionId = "TXN-TEST";
        var email = "test@email.com";

        repositoryMock
            .Setup(r => r.ItineraryExistsAsync(transactionId))
            .ReturnsAsync(true);

        repositoryMock
            .Setup(r => r.GetBuyerEmailByTransactionIdAsync(transactionId))
            .ReturnsAsync(email);

        await service.RequestCancellationAsync(transactionId);

        repositoryMock.Verify(
            r => r.SaveCancellationTokenAsync(
                transactionId,
                It.IsAny<string>(),
                It.IsAny<DateTime>()
            ),
            Times.Once
        );

        emailServiceMock.Verify(
            e => e.SendCancellationEmailAsync(
                email,
                It.IsAny<string>()
            ),
            Times.Once
        );
    }
}