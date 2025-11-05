using AutoMapper;
using Backend.Domain;
using Backend.Domain.DTOs;
using Backend.Exceptions.Custom;
using Backend.Interfaces;
using Backend.Service;
using Backend.Service.Validators;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;
using IValidatorFactory = Backend.Interfaces.IValidatorFactory;

namespace BackendTests;

public class TimetableServiceTests
{
    private readonly Mock<ITimetableRepository> _mockRepository = new();
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly IValidatorFactory _validatorFactory;
    private readonly Mock<IValidator<SubjectPostDTO>> _mockValidator = new();
    private readonly Mock<IAcademicRepository> _mockAcademicRepository = new();

    private readonly TimetableService _service;
    
    public TimetableServiceTests()
    {
        var realValidator = new SubjectPostDTOValidator(_mockRepository.Object,_mockAcademicRepository.Object);
        
        var mockValidatorFactory = new Mock<IValidatorFactory>();
        mockValidatorFactory.Setup(v => v.Get<SubjectPostDTO>()).Returns(realValidator);

        _validatorFactory = mockValidatorFactory.Object;

        _service = new TimetableService(
            _mockRepository.Object,
            _mockMapper.Object,
            _validatorFactory
        );
    }
    
    [Theory]
    [InlineData("Analiza", 6, 1)]
    [InlineData("FP", 5, 2)]
    public async Task CreateSubjectValidData(string name, int credits, int groupYearId)
    {
        var postDto = new SubjectPostDTO { Name = name, NumberOfCredits = credits, GroupYearId = groupYearId };
        
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty};
       
        var groupYear = new GroupYear { Id = groupYearId, Year = "IR1", Specialisation = specialisation};
        
        _mockAcademicRepository
            .Setup(r => r.GetGroupYearByIdAsync(groupYearId))
            .ReturnsAsync(groupYear);
        
        var subjectEntity = new Subject { Id = 1, Name = name, NumberOfCredits = credits, GroupYearId = groupYearId, GroupYear = groupYear };

        var responseDto = new SubjectResponseDTO { Id = 1, Name = name, NumberOfCredits = credits };
        

        _mockMapper.Setup(m => m.Map<Subject>(postDto)).Returns(subjectEntity);
        _mockMapper.Setup(m => m.Map<SubjectResponseDTO>(subjectEntity)).Returns(responseDto);

        _mockRepository.Setup(r => r.AddSubjectAsync(subjectEntity)).ReturnsAsync(subjectEntity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        
        var result = await _service.CreateSubject(postDto);
        
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(credits, result.NumberOfCredits);

        _mockRepository.Verify(r => r.AddSubjectAsync(subjectEntity), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData("Analiza", 10, 1)]
    [InlineData("", 5, 2)]
    [InlineData("FP", 0, 3)]
    public async Task CreateSubjectInvalidData(string name, int credits, int groupYearId)
    {
        var postDto = new SubjectPostDTO { Name = name, NumberOfCredits = credits, GroupYearId = groupYearId };
        
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty};
       
        var groupYear = new GroupYear { Id = groupYearId, Year = "IR1", Specialisation = specialisation};
        
        _mockAcademicRepository
            .Setup(r => r.GetGroupYearByIdAsync(groupYearId))
            .ReturnsAsync(groupYear);
        
        var subjectEntity = new Subject { Id = 1, Name = name, NumberOfCredits = credits, GroupYearId = groupYearId, GroupYear = groupYear };

        var responseDto = new SubjectResponseDTO { Id = 1, Name = name, NumberOfCredits = credits };
        
        
        await Assert.ThrowsAsync<EntityValidationException>(() => _service.CreateSubject(postDto));
        
        _mockRepository.Verify(r => r.AddSubjectAsync(subjectEntity), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    
    [Theory]
    [InlineData(1, "Analiza", 6)]
    [InlineData(2, "FP", 5)]
    public async Task GetSubjectByIdValidId(int id, string name, int credits)
    {
        var faculty = new Faculty { Id = 1, Name = "Facultate de Mate-Info" };
        var specialisation = new Specialisation { Id = 1, Name = "Computer Science", Faculty = faculty };
        var groupYear = new GroupYear { Id = 1, Year = "IR1", Specialisation = specialisation };
        var subjectEntity = new Subject { Id = id, Name = name, NumberOfCredits = credits, GroupYear = groupYear, GroupYearId = groupYear.Id };
        var responseDto = new SubjectResponseDTO { Id = id, Name = name, NumberOfCredits = credits };

        _mockRepository.Setup(r => r.GetSubjectByIdAsync(id))
            .ReturnsAsync(subjectEntity);

        _mockMapper.Setup(m => m.Map<SubjectResponseDTO>(subjectEntity))
            .Returns(responseDto);
        
        var result = await _service.GetSubjectById(id);
        
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(name, result.Name);
        Assert.Equal(credits, result.NumberOfCredits);

        _mockRepository.Verify(r => r.GetSubjectByIdAsync(id), Times.Once);
        _mockMapper.Verify(m => m.Map<SubjectResponseDTO>(subjectEntity), Times.Once);
    }
    
    [Theory]
    [InlineData(999)]
    [InlineData(0)]
    public async Task GetSubjectByIdInvalidId(int invalidId)
    {
        _mockRepository.Setup(r => r.GetSubjectByIdAsync(invalidId))
            .ReturnsAsync((Subject?)null);
        
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetSubjectById(invalidId));

        _mockRepository.Verify(r => r.GetSubjectByIdAsync(invalidId), Times.Once);
        _mockMapper.Verify(m => m.Map<SubjectResponseDTO>(It.IsAny<Subject>()), Times.Never);
    }
    
}