using log4net;
using Microsoft.AspNetCore.Mvc;
using TrackForUBB.Controller.Interfaces;
using TrackForUBB.Domain.DTOs.Contracts;

namespace TrackForUBB.Controller;

[ApiController]
[Route("api/[controller]")]
public class ContractController(IContractService service) : ControllerBase
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(ContractController));

    [HttpPost("{userId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ContractOfUser(int userId, [FromBody] ContractPostRequest request)
    {
        _logger.InfoFormat("Received request for contaract of user {0} promotion {1} year {2}", userId, request.PromotionId, request.Fields.Year);

        var bytes = await service.GenerateContract(userId, request);

        return File(bytes, "application/pdf", "Contract Studii.pdf");
    }
}
