using AutoMapper;
using Backend.Domain;
using Backend.Domain.DTOs;
using Backend.Exceptions.Custom;
using Backend.Interfaces;
using log4net;
using System.Text.Json;
using Backend.Domain.Enums;

namespace Backend.Service;

public class TimetableService(ITimetableRepository timetableRepository, IMapper mapper, IValidatorFactory validatorFactory)  : ITimetableService
{
    private readonly ITimetableRepository _timetableRepository = timetableRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILog _logger = LogManager.GetLogger(typeof(TimetableService));
    private readonly IValidatorFactory _validatorFactory = validatorFactory;

    public async Task<SubjectResponseDTO> CreateSubject(SubjectPostDTO subjectPostDto)
    {
        _logger.InfoFormat("Validating SubjectPostDTO: {0}", JsonSerializer.Serialize(subjectPostDto));
        var validator = _validatorFactory.Get<SubjectPostDTO>();
        var validationResult = await validator.ValidateAsync(subjectPostDto);
        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        var subject = _mapper.Map<Subject>(subjectPostDto);

        _logger.InfoFormat("Adding new subject to repository: {0}", JsonSerializer.Serialize(subject));
        subject = await _timetableRepository.AddSubjectAsync(subject);
        await _timetableRepository.SaveChangesAsync();

        var subjectDto = _mapper.Map<SubjectResponseDTO>(subject);
        return subjectDto;
    }
    
    public async Task<SubjectResponseDTO> GetSubjectById(int subjectId)
    {
        _logger.InfoFormat("Trying to retrieve subject with id {0}", JsonSerializer.Serialize(subjectId));
        var subject = await _timetableRepository.GetSubjectByIdAsync(subjectId)
                      ?? throw new NotFoundException($"Subject with ID {subjectId} not found.");

        _logger.InfoFormat("Mapping subject entity to DTO for ID {0}", JsonSerializer.Serialize(subjectId));
        var subjectDto = _mapper.Map<SubjectResponseDTO>(subject);

        return subjectDto;
    }
}