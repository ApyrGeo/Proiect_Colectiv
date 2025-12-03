using FluentValidation;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs;
using TrackForUBB.Domain.Exceptions.Custom;
using TrackForUBB.Service.Interfaces;
using IValidatorFactory = TrackForUBB.Service.Interfaces.IValidatorFactory;

namespace TrackForUBB.Service;

public class ExamService(IExamRepository examRepository, IGradeRepository gradeRepository, IValidatorFactory validatorFactory) : IExamService
{
    private readonly IExamRepository _examRepository = examRepository;
    private readonly IGradeRepository _gradeRepository = gradeRepository;
    private readonly IValidatorFactory _validatorFactory = validatorFactory;

    public async Task<List<ExamEntryResponseDTO>> GetExamsBySubjectId(int subjectId)
    {
        var _ = await _gradeRepository.GetSubjectById(subjectId)
            ?? throw new NotFoundException("Subject not found");

        return await _examRepository.GetExamsBySubjectId(subjectId);
    }

    public async Task<List<ExamEntryResponseDTO>> UpdateExamEntries(List<ExamEntryPutDTO> examEntries)
    {
        var validator = _validatorFactory.Get<ExamEntryPutDTO>();
        int index = 0;
        foreach (var examEntry in examEntries)
        {
            var validationResult = await validator.ValidateAsync(examEntry);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(index.ToString(), validationResult.Errors);
            }
            index++;
        }

        return await _examRepository.UpdateExamEntries(examEntries);
    }
}
