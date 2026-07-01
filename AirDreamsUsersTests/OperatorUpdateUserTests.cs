using AirDreams.API.DTOs;
using AirDreams.API.Models;
using AirDreams.API.Repositories;
using AirDreams.API.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace AirDreams.API.Tests;

[TestFixture]
public class OperatorUpdateUserTests
{
    [Test]
    public void UpdateUserAsync_WhenOperatorEditsAnotherUser_ShouldThrowUnauthorizedAccessException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();

        var userService = new UserService(
            userRepositoryMock.Object,
            Mock.Of<IEmailService>(),
            Mock.Of<IConfiguration>()
        );

        var currentUserEmail = "operador@airdreams.com";
        byte currentUserId = 2;
        byte targetUserId = 3;

        var currentUser = new AirlineEmployee
        {
            EmployeeID = currentUserId,
            NameEmployee = "Operador",
            Lastnames = "Uno",
            EmailInternalUser = currentUserEmail,
            IsAdmin = false,
            IsOperator = true
        };

        var targetUser = new AirlineEmployee
        {
            EmployeeID = targetUserId,
            NameEmployee = "Otro",
            Lastnames = "Usuario",
            EmailInternalUser = "otro@airdreams.com",
            IsAdmin = false,
            IsOperator = true
        };

        var updateDto = new UpdateUserDTO
        {
            FirstName = "Intento",
            LastName = "NoPermitido"
        };

        userRepositoryMock
            .Setup(repo => repo.GetCurrentUserIdFromEmailAsync(currentUserEmail))
            .ReturnsAsync(currentUserId);

        userRepositoryMock
            .Setup(repo => repo.GetAirlineEmployeeByIdAsync(currentUserId))
            .ReturnsAsync(currentUser);

        userRepositoryMock
            .Setup(repo => repo.GetAirlineEmployeeByIdAsync(targetUserId))
            .ReturnsAsync(targetUser);

        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await userService.UpdateUserAsync(
                targetUserId,
                updateDto,
                currentUserEmail
            )
        );

        userRepositoryMock.Verify(repo =>
            repo.UpdateAirlineEmployeeAsync(
                It.IsAny<byte>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<bool?>(),
                It.IsAny<bool?>()
            ),
            Times.Never
        );
    }
}