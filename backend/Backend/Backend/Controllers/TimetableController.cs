using Backend.Interfaces;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimetableController(ITimetableService service) : ControllerBase
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(AcademicsController));
    private readonly ITimetableService _service = service;
    
    
    
}