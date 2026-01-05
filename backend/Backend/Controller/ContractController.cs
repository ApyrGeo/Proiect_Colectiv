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
    public async Task<FileResult> ContractOfUser(int userId)
    {
        _logger.InfoFormat("Received request for contaract of user {0}", userId);

        var bytes = await service.GenerateContract(userId);

        return File(bytes, "application/pdf", "Contract Studii.pdf");
    }
}
