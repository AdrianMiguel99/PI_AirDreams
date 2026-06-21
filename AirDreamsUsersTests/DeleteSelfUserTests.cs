using AirDreams.API.Models;
using AirDreams.API.Repositories;
using AirDreams.API.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace AirDreamsUsersTests;

[TestFixture]
public class DeleteSelfUserTests
{
    [Test]
    public void DeleteUserAsync_WhenAdminDeletesSelf_ShouldThrowInvalidOperationException()
    {
        var repositoryMock = new Mock<IUserRepository>();

        var service = new UserService(
            repositoryMock.Object,
            Mock.Of<IEmailService>(),
            Mock.Of<IConfiguration>()
        );

        var currentUserEmail = "admin@air.com";

        repositoryMock
            .Setup(r => r.GetCurrentUserIdFromEmailAsync(currentUserEmail))
            .ReturnsAsync((byte)1);

        repositoryMock
            .Setup(r => r.GetAirlineEmployeeByIdAsync(1))
            .ReturnsAsync(new AirlineEmployee
            {
                EmployeeID = 1,
                EmailInternalUser = currentUserEmail,
                IsAdmin = true
            });

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.DeleteUserAsync(
                1,
                currentUserEmail
            )
        );

        repositoryMock.Verify(
            r => r.SoftDeleteUserAsync(It.IsAny<byte>()),
            Times.Never
        );
    }
}