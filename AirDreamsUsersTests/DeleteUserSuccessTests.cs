using AirDreams.API.Models;
using AirDreams.API.Repositories;
using AirDreams.API.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace AirDreamsUsersTests;

[TestFixture]
public class DeleteUserSuccessTests
{
    [Test]
    public async Task DeleteUserAsync_WhenAdminDeletesUser_ShouldSoftDeleteUser()
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

        repositoryMock
            .Setup(r => r.GetAirlineEmployeeByIdAsync(2))
            .ReturnsAsync(new AirlineEmployee
            {
                EmployeeID = 2,
                EmailInternalUser = "operator@air.com",
                IsAdmin = false
            });

        var result = await service.DeleteUserAsync(
            2,
            currentUserEmail
        );

        Assert.That(result, Is.True);

        repositoryMock.Verify(
            r => r.SoftDeleteUserAsync(2),
            Times.Once
        );
    }
}