using FluentValidation;
using log4net;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Exceptions.Custom;
using TrackForUBB.Service.Interfaces;
using TrackForUBB.Service.Utils;
using IValidatorFactory = TrackForUBB.Service.Interfaces.IValidatorFactory;

namespace TrackForUBB.Service;

public class ExamService(IExamRepository examRepository, ITimetableRepository timetableRepository, IUserRepository userRepository, IValidatorFactory validatorFactory) : IExamService
{
    private readonly IExamRepository _examRepository = examRepository;
    private readonly ITimetableRepository _timetableRepository = timetableRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IValidatorFactory _validatorFactory = validatorFactory;
    private readonly ILog _logger = LogManager.GetLogger(typeof(ExamService));

    public async Task<List<ExamEntryResponseDTO>> GetExamsBySubjectId(int subjectId)
    {
        _logger.Info($"Fetching exams for subject ID: {subjectId}");
        var _ = await _timetableRepository.GetSubjectByIdAsync(subjectId)
            ?? throw new NotFoundException("Subject not found");

        return await _examRepository.GetExamsBySubjectId(subjectId);
    }

    public async Task<List<ExamEntryForStudentDTO>> GetStudentExamsByStudentId(int studentId)
    {
        _logger.Info($"Fetching exams for student ID: {studentId}");

        var _ = await _userRepository.GetByIdAsync(studentId)
            ?? throw new NotFoundException("Student not found");

        return await _examRepository.GetStudentExamsByStudentId(studentId);
    }

    public async Task<List<ExamEntryResponseDTO>> UpdateExamEntries(List<ExamEntryPutDTO> examEntries)
    {
        _logger.Info("Updating exam entries");
        var validator = _validatorFactory.Get<ExamEntryPutDTO>();
        int index = 0;
        foreach (var examEntry in examEntries)
        {
            var validationResult = await validator.ValidateAsync(examEntry);
            if (!validationResult.IsValid)
            {
                throw new EntityValidationException(ValidationHelper.ConvertErrorsToListOfStrings(validationResult.Errors));
            }
            index++;
        }

        return await _examRepository.UpdateExamEntries(examEntries);
    }
}
