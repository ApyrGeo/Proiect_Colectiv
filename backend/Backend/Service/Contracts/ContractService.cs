using System.Text.Json;
using AutoMapper;
using log4net;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs.Contracts;
using TrackForUBB.Domain.Exceptions.Custom;
using TrackForUBB.Service.Contracts.Models;
using TrackForUBB.Service.Interfaces;
using TrackForUBB.Service.PdfGeneration;

namespace TrackForUBB.Service.Contracts;

public class ContractService(
    IPdfGenerator pdfGenerator,
    IContractUnitOfWork contractUoW,
    IMapper mapper,
    PdfConverterConfiguration pdfConfig
) : IContractService
{
    public async Task<byte[]> GenerateContract(int userId, ContractPostRequest request)
    {
        if (!request.Fields.Agree)
            throw new UnprocessableContentException("You must agree to generate a contract");

        var model = await CreateModel(userId, request);
        var path = Path.Combine(pdfConfig.ContractsPath, "ContractStudii.ro.odt");
        return await pdfGenerator.Generate(path, model);
    }

    private async Task<ContractViewModel> CreateModel(int userId, ContractPostRequest request)
    {
        var requestModel = ToRequestModel(userId, request);
        var contract = await contractUoW.GetData(requestModel);

        var viewModel = mapper.Map<ContractViewModel>(contract);

        ForEachIndexed(viewModel.SubjectsSemester1, (x, i) => x.I = i, 1);
        viewModel.TotalCreditsSemester1 = viewModel.SubjectsSemester1.Sum(x => x.Credits);
        ForEachIndexed(viewModel.SubjectsSemester2, (x, i) => x.I = i, 1);
        viewModel.TotalCreditsSemester2 = viewModel.SubjectsSemester2.Sum(x => x.Credits);

        var now = DateTime.Now;
        viewModel.Now = now.ToString("dd.MM.yyyy, HH:mm");

        var fields = request.Fields;

        AssignFilled(v => viewModel.CNP = v, fields.CNP);
        AssignFilled(v => viewModel.StudentEmail = v, fields.Email);
        AssignFilled(v => viewModel.FullName = v, fields.FullName);
        AssignFilled(v => viewModel.IdCardNumber = v, fields.IdCardNumber);
        AssignFilled(v => viewModel.IdCardSeries = v, fields.IdCardSeries);
        AssignFilled(v => viewModel.StudentPhone = v, fields.PhoneNumber);
        AssignFilled(v => viewModel.Signature = v, fields.SignatureBase64);

        return viewModel;
    }

    private static void ForEachIndexed<T>(IEnumerable<T> enumerable, Action<T, int> callback, int start)
    {
        int i = start;
        foreach (var val in enumerable) {
            callback(val, i);
            ++ i;
        }
    }

    private static void AssignFilled(Action<string> setter, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            setter(value);
    }

    private static void AssignFilled<T>(Action<string> setter, T? value)
    {
        if (value is not null)
            setter(value.ToString()!);
    }

    private static ContractRequestModel ToRequestModel(int userId, ContractPostRequest request)
    {
        var fields = request.Fields;

        return new()
        {
            SignatureBase64 = fields.SignatureBase64,

            OptionalToSubjectCodesSem1 = ToOptionalDict([
                fields.OptionalSem1Pak1Id,
                fields.OptionalSem1Pak2Id,
                fields.OptionalSem1Pak3Id,
                fields.OptionalSem1Pak4Id,
                fields.OptionalSem1Pak5Id,
                fields.OptionalSem1Pak6Id,
            ]),
            OptionalToSubjectCodesSem2 = ToOptionalDict([
                fields.OptionalSem2Pak1Id,
                fields.OptionalSem2Pak2Id,
                fields.OptionalSem2Pak3Id,
                fields.OptionalSem2Pak4Id,
                fields.OptionalSem2Pak5Id,
                fields.OptionalSem2Pak6Id,
            ]),

            PromotionId = request.PromotionId,
            Year = fields.Year,
            UserId = userId,
        };
    }

    private static IDictionary<int, int> ToOptionalDict(List<int?> codes) =>
        codes
            .Select((value, index) => (value, index))
            .Where(x => x.value is not null)
            .ToDictionary(x => x.index, x => x.value!.Value);

    private readonly ILog logger = LogManager.GetLogger(typeof(ContractService));
}
