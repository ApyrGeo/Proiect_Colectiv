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
    public async Task<byte[]> GenerateContract(int userId)
    {
        var model = await CreateModel(userId);
        var path = Path.Combine(pdfConfig.ContractsPath, "ContractStudii.ro.odt");
        return await pdfGenerator.Generate(path, model);
    }

    private async Task<ContractViewModel> CreateModel(int userId)
    {
        var contracts = await contractUoW.GetData(userId);
        if (contracts.Count == 0)
            throw new Exception("User is not register at any faculty");
        if (contracts.Count > 1)
            throw new Exception("User is registered at too many faculties");
        var contract = contracts[0];

        var viewModel = mapper.Map<ContractViewModel>(contract);

        ForEachIndexed(viewModel.SubjectsSemester1, (x, i) => x.I = i, 1);
        viewModel.TotalCreditsSemester1 = viewModel.SubjectsSemester1.Sum(x => x.Credits);
        ForEachIndexed(viewModel.SubjectsSemester2, (x, i) => x.I = i, 1);
        viewModel.TotalCreditsSemester2 = viewModel.SubjectsSemester2.Sum(x => x.Credits);

        var now = DateTime.Now;
        viewModel.Now = now.ToString("dd.MM.yyyy, H:m");

        var startOfUniYear = now.Month >= 7 ? now.Year : now.Year - 1;
        var uniYear = $"{startOfUniYear}-{startOfUniYear + 1}";
        viewModel.UniversityYear = uniYear;

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
