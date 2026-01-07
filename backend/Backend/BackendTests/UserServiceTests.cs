using Microsoft.Extensions.Configuration;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Exceptions.Custom;
using TrackForUBB.Service;
using TrackForUBB.Service.Validators;
using Moq;
using TrackForUBB.Domain.Enums;
using Xunit;
using IValidatorFactory = TrackForUBB.Service.Interfaces.IValidatorFactory;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Domain.Security;
using TrackForUBB.Service.Interfaces;
using Microsoft.Graph;
using AutoMapper;

namespace TrackForUBB.BackendTests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IAdapterPasswordHasher<InternalUserPostDTO>> _mockPasswordHasher = new();
    private readonly Mock<IEmailProvider> _mockEmailProvider = new();
    private readonly IValidatorFactory _validatorFactory;
    private readonly UserService _userService;
    private readonly Mock<IConfiguration> conf = new();
    private readonly Mock<IAcademicRepository> _mockAcademicRepository = new();
    private readonly Mock<GraphServiceClient> _mockGraph = new();
    private readonly Mock<IMapper> _mockMapper = new();

    public UserServiceTests()
    {
        var realValidator = new InternalUserPostDTOValidator(_mockUserRepository.Object);

        var mockValidatorFactory = new Mock<IValidatorFactory>();
        mockValidatorFactory.Setup(v => v.Get<InternalUserPostDTO>()).Returns(realValidator);

        _validatorFactory = mockValidatorFactory.Object;

        _userService = new UserService(_mockUserRepository.Object, _mockAcademicRepository.Object, _mockMapper.Object, _validatorFactory,
            _mockEmailProvider.Object, conf.Object, _mockGraph.Object);
    }

    [Theory]
    [InlineData("Vanya", "Doktorovic", "+40712345678", "vandok@gmail.com", "Admin")]
    [InlineData("Andrei", "Horo", "40712345678", "horo@gmail.com", "Teacher")]
    public async Task CreateUserValidData(string firstName, string lastName, string phone, string email,
        string role)
    {
        var userDTO = new UserPostDTO
        {
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phone,
            Email = email,
            Role = role
        };

        var internalUserDTO = new InternalUserPostDTO
        {
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phone,
            Email = email,
            Role = role
        };

        var userResponseDTO = new UserResponseDTO
        {
            Id = 1,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phone,
            Role = Enum.Parse<UserRole>(role),
            TenantEmail = "",
            Owner = ""
        };

        _mockUserRepository.Setup(r => r.AddAsync(internalUserDTO)).ReturnsAsync(userResponseDTO);


        var result = await _userService.CreateUser(userDTO);

        Assert.NotNull(result);
        Assert.Equal(firstName, result.FirstName);
        Assert.Equal(lastName, result.LastName);
        Assert.Equal(email, result.Email);

        _mockUserRepository.Verify(r => r.AddAsync(internalUserDTO), Times.Once);
    }

    [Theory]
    [InlineData("", "Doktorovic", "+40759305094", "vandok@gmail.com", "Admin")]
    [InlineData("Vanya", "", "+40759305094", "vandok@gmail.com", "Admin")]
    [InlineData("Vanya", "Doktorovic", "", "vandok@gmail.com", "Admin")]
    [InlineData("Vanya", "Doktorovic", "0759305094", "invalid-email", "Admin")]
    [InlineData("Vanya", "Doktorovic", "+40759305094", "vandok@gmail.com", "Admin")]
    [InlineData("Vanya", "Doktorovic", "+10adwd21234", "vandok@gmail.com", "Admin")]
    [InlineData("Vanya", "Doktorovic", "+40759305094", "vandok@gmail.com", "Lvbhk")]
    public async Task CreateUserInvalidData(string firstName, string lastName, string phone, string email,
        string role)
    {
        var userDTO = new UserPostDTO
        {
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phone,
            Email = email,
            Role = role
        };

        await Assert.ThrowsAsync<EntityValidationException>(() => _userService.CreateUser(userDTO));

        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<InternalUserPostDTO>()), Times.Never);

        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<InternalUserPostDTO>()), Times.Never);
    }

    [Theory]
    [InlineData(1, "Vanya", "Doktorovic", "+40759305094", "vandok@gmail.com", "Admin")]
    [InlineData(2, "Andrei", "Horo", "+40779725710", "horo@gmail.com", "Teacher")]
    public async Task GetUserByIdExistingUser(int id, string firstName, string lastName, string phone, string email,
        string role)
    {
        var userDto = new UserResponseDTO
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phone,
            Email = email,
            Role = Enum.Parse<UserRole>(role),
            TenantEmail = "",
            Owner = ""
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(userDto);

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
            .ReturnsAsync((UserResponseDTO?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUserById(invalidId));

        _mockUserRepository.Verify(r => r.GetByIdAsync(invalidId), Times.Once);
    }
}
