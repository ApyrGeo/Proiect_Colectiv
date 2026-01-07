using TrackForUBB.Domain.DTOs;

namespace TrackForUBB.Controller.Interfaces;

public interface IGenerationService
{
    Task<List<ExamEntryResponseDTO>> GenerateExamEntries(GenerateExamEntriesRequestDTO request);
}
