using System.Runtime.InteropServices.ComTypes;
using AutoMapper;
using Backend.Domain;
using Backend.Domain.DTOs;
using Backend.Domain.Enums;
using Backend.Exceptions.Custom;
using Backend.Interfaces;
using Backend.Service;
using Backend.Service.Validators;
using EmailService.Configuration;
using EmailService.Interfaces;
using EmailService.Models;
using EmailService.Providers;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using IValidatorFactory = Backend.Interfaces.IValidatorFactory;

namespace BackendTests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher = new();
    private readonly Mock<IEmailProvider> _mockEmailProvider = new();
    private readonly IValidatorFactory _validatorFactory;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        var realValidator = new UserPostDTOValidator(_mockUserRepository.Object);

        var mockValidatorFactory = new Mock<IValidatorFactory>();
        mockValidatorFactory.Setup(v => v.Get<UserPostDTO>()).Returns(realValidator);

        _validatorFactory = mockValidatorFactory.Object;

        _userService = new UserService(_mockUserRepository.Object, _mockMapper.Object, _validatorFactory,
            _mockPasswordHasher.Object, _mockEmailProvider.Object);
    }

    [Theory]
    [InlineData("Vanya", "Doktorovic", "+40712345678", "vandok@gmail.com", "pass1234", "Admin")]
    [InlineData("Andrei", "Horo", "40712345678", "horo@gmail.com", "password", "Teacher")]
    public async Task CreateUserValidData(string firstName, string lastName, string phone, string email,
        string password, string role)
    {
        var userDTO = new UserPostDTO
        {
            FirstName = firstName, LastName = lastName, PhoneNumber = phone, Email = email, Password = password,
            Role = role
        };


        var userEntity = new User
        {
            FirstName = firstName, LastName = lastName, Email = email, Password = password, PhoneNumber = phone,
            Role = Enum.Parse<UserRole>(role)
        };
        var userResponseDTO = new UserResponseDTO
        {
            Id = 1, FirstName = firstName, LastName = lastName, Email = email, PhoneNumber = phone, Password = password,
            Role = role
        };

        _mockMapper.Setup(m => m.Map<User>(userDTO)).Returns(userEntity);
        _mockMapper.Setup(m => m.Map<UserResponseDTO>(userEntity)).Returns(userResponseDTO);

        _mockPasswordHasher.Setup(h => h.HashPassword(userEntity, password)).Returns("hashedPassword");

        _mockUserRepository.Setup(r => r.AddAsync(userEntity)).ReturnsAsync(userEntity);
        _mockUserRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);


        var result = await _userService.CreateUser(userDTO);

        Assert.NotNull(result);
        Assert.Equal(firstName, result.FirstName);
        Assert.Equal(lastName, result.LastName);
        Assert.Equal(email, result.Email);

        _mockUserRepository.Verify(r => r.AddAsync(userEntity), Times.Once);
        _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData("", "Doktorovic", "+40759305094", "vandok@gmail.com", "pass1234", "Admin")]
    [InlineData("Vanya", "", "+40759305094", "vandok@gmail.com", "pass1234", "Admin")]
    [InlineData("Vanya", "Doktorovic", "", "vandok@gmail.com", "pass1234", "Admin")]
    [InlineData("Vanya", "Doktorovic", "0759305094", "invalid-email", "pass1234", "Admin")]
    [InlineData("Vanya", "Doktorovic", "+40759305094", "vandok@gmail.com", "", "Admin")]
    [InlineData("Vanya", "Doktorovic", "+10adwd21234", "vandok@gmail.com", "pass1234", "Admin")]
    [InlineData("Vanya", "Doktorovic", "+40759305094", "vandok@gmail.com", "pass1234", "Lvbhk")]
    public async Task CreateUserInvalidData(string firstName, string lastName, string phone, string email,
        string password, string role)
    {
        var userDTO = new UserPostDTO
        {
            FirstName = firstName, LastName = lastName, PhoneNumber = phone, Email = email, Password = password,
            Role = role
        };

        await Assert.ThrowsAsync<EntityValidationException>(() => _userService.CreateUser(userDTO));

        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);

        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Theory]
    [InlineData(1, "Vanya", "Doktorovic", "+40759305094", "vandok@gmail.com", "pass1234", "Admin")]
    [InlineData(2, "Andrei", "Horo", "+40779725710", "horo@gmail.com", "password", "Teacher")]
    public async Task GetUserByIdExistingUser(int id, string firstName, string lastName, string phone, string email,
        string password, string role)
    {
        var user = new User
        {
            Id = id, FirstName = firstName, LastName = lastName, Email = email, Password = password,
            PhoneNumber = phone, Role = Enum.Parse<UserRole>(role)
        };
        var userDto = new UserResponseDTO
        {
            Id = id, FirstName = firstName, LastName = lastName, PhoneNumber = phone, Email = email,
            Password = password, Role = role
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserResponseDTO>(user)).Returns(userDto);

        var result = await _userService.GetUserById(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(email, result.Email);

        _mockUserRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    [Theory]
    [InlineData(999)]
    [InlineData(0)]
    public async Task GetUserByIdNonExistingUser(int invalidId)
    {
        _mockUserRepository.Setup(r => r.GetByIdAsync(invalidId))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUserById(invalidId));

        _mockUserRepository.Verify(r => r.GetByIdAsync(invalidId), Times.Once);
        _mockMapper.Verify(m => m.Map<UserResponseDTO>(It.IsAny<User>()), Times.Never);
    }
}