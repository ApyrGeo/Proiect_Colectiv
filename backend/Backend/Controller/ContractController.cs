using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackForUBB.Controller.Interfaces;

namespace TrackForUBB.Controller;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ContractController(IContractService service) : ControllerBase
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(ContractController));

    [HttpGet("{userId}")]
    [ProducesResponseType(200)]
    public async Task<FileResult> ContractOfUser(int userId, int promotionId, int year)
    {
        _logger.InfoFormat("Received request for contaract of user {0} promotion {1} year {2}", userId, promotionId, year);

        var bytes = await service.GenerateContract(userId, promotionId, year);

        return File(bytes, "application/pdf", "Contract Studii.pdf");
    }
}
