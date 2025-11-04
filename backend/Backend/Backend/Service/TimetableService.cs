using AutoMapper;
using Backend.Domain;
using Backend.Domain.DTOs;
using Backend.Exceptions.Custom;
using Backend.Interfaces;
using Backend.Utils;
using log4net;
using System.Text.Json;
using IValidatorFactory = Backend.Interfaces.IValidatorFactory;

namespace Backend.Service;

public class TimetableService(ITimetableRepository timetableRepository, IMapper mapper, IValidatorFactory validatorFactory) : ITimetableService
{
    private readonly ITimetableRepository _timetableRepository = timetableRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILog _logger = LogManager.GetLogger(typeof(TimetableService));
    private readonly IValidatorFactory _validatorFactory = validatorFactory;

    public async Task<ClassroomResponseDTO> CreateClassroom(ClassroomPostDTO classroomPostDTO)
    {
        _logger.InfoFormat("Validating ClassroomPostDTO: {0}", JsonSerializer.Serialize(classroomPostDTO));

        var validator = _validatorFactory.Get<ClassroomPostDTO>();
        var validationResult = await validator.ValidateAsync(classroomPostDTO);

        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        var classroom = _mapper.Map<Classroom>(classroomPostDTO);

        _logger.InfoFormat("Adding new classroom to repository: {0}", JsonSerializer.Serialize(classroom));

        classroom = await _timetableRepository.AddClassroomAsync(classroom);
        await _timetableRepository.SaveChangesAsync();

        return _mapper.Map<ClassroomResponseDTO>(classroom);
    }

    public async Task<HourResponseDTO> CreateHour(HourPostDTO hourPostDTO)
    {
        _logger.InfoFormat("Validating HourPostDTO: {0}", JsonSerializer.Serialize(hourPostDTO));

        var validator = _validatorFactory.Get<HourPostDTO>();
        var validationResult = await validator.ValidateAsync(hourPostDTO);

        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        var hour = _mapper.Map<Hour>(hourPostDTO);

        _logger.InfoFormat("Adding new hour to repository: {0}", JsonSerializer.Serialize(hour));

        hour = await _timetableRepository.AddHourAsync(hour);
        await _timetableRepository.SaveChangesAsync();

        return _mapper.Map<HourResponseDTO>(hour);
    }

    public async Task<LocationResponseDTO> CreateLocation(LocationPostDTO locationPostDTO)
    {
        _logger.InfoFormat("Validating LocationPostDTO: {0}", JsonSerializer.Serialize(locationPostDTO));

        var validator = _validatorFactory.Get<LocationPostDTO>();
        var validationResult = await validator.ValidateAsync(locationPostDTO);

        if (!validationResult.IsValid)
        {
            throw new EntityValidationException(validationResult.Errors);
        }

        var location = _mapper.Map<Location>(locationPostDTO);

        _logger.InfoFormat("Adding new location to repository: {0}", JsonSerializer.Serialize(location));

        location = await _timetableRepository.AddLocationAsync(location);
        await _timetableRepository.SaveChangesAsync();

        return _mapper.Map<LocationResponseDTO>(location);
    }

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

        return _mapper.Map<SubjectResponseDTO>(subject);
    }

    public async Task<ClassroomResponseDTO> GetClassroomById(int classroomId)
    {
        _logger.InfoFormat("Trying to retrieve classroom with id {0}", classroomId);

        var classroom = await _timetableRepository.GetClassroomByIdAsync(classroomId)
            ?? throw new NotFoundException($"Classroom with ID {classroomId} not found.");

        _logger.InfoFormat("Mapping classroom entity to DTO for ID {0}", classroomId);

        return _mapper.Map<ClassroomResponseDTO>(classroom);
    }

    public async Task<List<HourResponseDTO>> GetHourByFilter(HourFilter filter)
    {
        _logger.InfoFormat("Trying to retrive hours with filter {0}", JsonSerializer.Serialize(filter));

        var hours = await _timetableRepository.GetHoursAsync(filter);

        _logger.InfoFormat("Mapping hour entities to DTOs for filter {0}", JsonSerializer.Serialize(filter));

        return _mapper.Map<List<HourResponseDTO>>(hours);
    }

    public async Task<HourResponseDTO> GetHourById(int hourId)
    {
        _logger.InfoFormat("Trying to retrieve hour with id {0}", hourId);

        var hour = await _timetableRepository.GetHourByIdAsync(hourId)
            ?? throw new NotFoundException($"Hour with ID {hourId} not found.");

        _logger.InfoFormat("Mapping hour entity to DTO for ID {0}", hourId);

        return _mapper.Map<HourResponseDTO>(hour);
    }

    public async Task<LocationResponseDTO> GetLocationById(int locationId)
    {
        _logger.InfoFormat("Trying to retrieve location with id {0}", locationId);

        var location = await _timetableRepository.GetLocationByIdAsync(locationId)
            ?? throw new NotFoundException($"Location with ID {locationId} not found.");

        _logger.InfoFormat("Mapping location entity to DTO for ID {0}", locationId);

        return _mapper.Map<LocationResponseDTO>(location);
    }

    public async Task<SubjectResponseDTO> GetSubjectById(int subjectId)
    {
        _logger.InfoFormat("Trying to retrieve subject with id {0}", JsonSerializer.Serialize(subjectId));

        var subject = await _timetableRepository.GetSubjectByIdAsync(subjectId)
            ?? throw new NotFoundException($"Subject with ID {subjectId} not found.");

        _logger.InfoFormat("Mapping subject entity to DTO for ID {0}", JsonSerializer.Serialize(subjectId));

        return _mapper.Map<SubjectResponseDTO>(subject);
    }
}