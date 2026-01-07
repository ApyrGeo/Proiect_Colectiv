using AutoMapper;
using TrackForUBB.Controller.Interfaces;
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
    public async Task<byte[]> GenerateContract(int userId, int promotionId, int year)
    {
        var model = await CreateModel(userId, promotionId, year);
        var path = Path.Combine(pdfConfig.ContractsPath, "ContractStudii.ro.odt");
        return await pdfGenerator.Generate(path, model);
    }

    private async Task<ContractViewModel> CreateModel(int userId, int promotionId, int year)
    {
        var contract = await contractUoW.GetData(userId, promotionId, year);

        var viewModel = mapper.Map<ContractViewModel>(contract);

        ForEachIndexed(viewModel.SubjectsSemester1, (x, i) => x.I = i, 1);
        viewModel.TotalCreditsSemester1 = viewModel.SubjectsSemester1.Sum(x => x.Credits);
        ForEachIndexed(viewModel.SubjectsSemester2, (x, i) => x.I = i, 1);
        viewModel.TotalCreditsSemester2 = viewModel.SubjectsSemester2.Sum(x => x.Credits);

        var now = DateTime.Now;
        viewModel.Now = now.ToString("dd.MM.yyyy, H:m");

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
}
