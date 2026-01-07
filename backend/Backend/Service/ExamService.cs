using FluentValidation;
using log4net;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Exceptions.Custom;
using TrackForUBB.Service.Interfaces;
using TrackForUBB.Service.Utils;
using IValidatorFactory = TrackForUBB.Service.Interfaces.IValidatorFactory;

namespace TrackForUBB.Service;

public class ExamService(IExamRepository examRepository, ITimetableRepository timetableRepository, IUserRepository userRepository, IValidatorFactory validatorFactory, IAcademicRepository academicRepository) : IExamService
{
    private readonly IExamRepository _examRepository = examRepository;
    private readonly ITimetableRepository _timetableRepository = timetableRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IValidatorFactory _validatorFactory = validatorFactory;
    private readonly IAcademicRepository _academicRepository = academicRepository;
    private readonly ILog _logger = LogManager.GetLogger(typeof(ExamService));

    public async Task DeleteExamEntry(int id)
    {
        var _ = await _examRepository.GetExamEntryByIdAsync(id)
            ?? throw new NotFoundException("Exam entry not found");

        _logger.Info($"Deleting exam entry with ID: {id}");
        await _examRepository.DeleteExamEntryAsync(id);
    }

    public async Task<List<ExamEntryResponseDTO>> GenerateExamEntries(GenerateExamEntriesRequestDTO request)
    {
        List<ExamEntryResponseDTO> examEntries = [];

        _logger.Info("Validating GenerateExamEntriesRequestDTO");
        var validator = _validatorFactory.Get<GenerateExamEntriesRequestDTO>();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new EntityValidationException(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        _logger.Info("Fetching subject for exam entry generation");
        var subject = await _timetableRepository.GetSubjectByIdAsync(request.SubjectId);

        _logger.Info("Generating exam entries for student groups");
        foreach (var studentGroupId in request.StudentGroupIds)
        {
            var studentGroup = await _academicRepository.GetGroupByIdAsync(studentGroupId);

            //verify that an exam entry does not already exist for this subject and student group
            var existingExamEntry = await _examRepository.GetExamEntryBySubjectAndGroupAsync(request.SubjectId, studentGroupId);
            if (existingExamEntry != null)
            {
                _logger.Warn($"Exam entry already exists for subject ID {request.SubjectId} and student group ID {studentGroupId}. Skipping generation.");
                continue;
            }

            var createdExamEntry = await _examRepository.CreateExamEntryAsync(new ExamEntryRequestDTO
            {
                Date = null,
                Duration = null,
                ClassroomId = null,
                SubjectId = subject!.Id,
                StudentGroupId = studentGroup!.Id
            });
            _logger.Info($"Generated exam entry with ID {createdExamEntry.Id} for student group ID {studentGroup.Id}");
            examEntries.Add(createdExamEntry);
        }

        return examEntries;
    }

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
