using AirDreams.API.DTOs;
using AirDreams.API.Models;
using AirDreams.API.Repositories;
using AirDreams.API.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace AirDreams.API.Tests;

[TestFixture]
public class AdminUpdateUserTests
{
    [Test]
    public async Task UpdateUserAsync_WhenAdminEditsAnotherUser_ShouldUpdateUser()
    {
        var userRepositoryMock = new Mock<IUserRepository>();

        var userService = new UserService(
            userRepositoryMock.Object,
            Mock.Of<IEmailService>(),
            Mock.Of<IConfiguration>()
        );

        var currentUserEmail = "admin@airdreams.com";
        byte currentUserId = 1;
        byte targetUserId = 2;

        var currentUser = new AirlineEmployee
        {
            EmployeeID = currentUserId,
            NameEmployee = "Admin",
            Lastnames = "Principal",
            EmailInternalUser = currentUserEmail,
            IsAdmin = true,
            IsOperator = false
        };

        var targetUser = new AirlineEmployee
        {
            EmployeeID = targetUserId,
            NameEmployee = "Operador",
            Lastnames = "Uno",
            EmailInternalUser = "operador@airdreams.com",
            IsAdmin = false,
            IsOperator = true
        };

        var updateDto = new UpdateUserDTO
        {
            FirstName = "Nuevo",
            LastName = "Nombre",
            IsAdmin = false,
            IsOperator = true,
            IsActive = true
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

        var result = await userService.UpdateUserAsync(
            targetUserId,
            updateDto,
            currentUserEmail
        );

        Assert.That(result, Is.True);

        userRepositoryMock.Verify(repo =>
            repo.UpdateAirlineEmployeeAsync(
                targetUserId,
                updateDto.FirstName,
                updateDto.LastName,
                updateDto.IsAdmin,
                updateDto.IsOperator
            ),
            Times.Once
        );

        userRepositoryMock.Verify(repo =>
            repo.UpdateInternalUserActiveStatusAsync(
                targetUserId,
                updateDto.IsActive
            ),
            Times.Once
        );
    }
}