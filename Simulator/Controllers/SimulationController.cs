using Microsoft.AspNetCore.Mvc;
using SimulatorFinalProject.Simulator.Models;
using SimulatorFinalProject.Simulator.Services;

namespace SimulatorFinalProject.Simulator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SimulationController : ControllerBase
{
    private readonly SimulationService _simulationService;

    public SimulationController(SimulationService simulationService)
    {
        _simulationService = simulationService;
    }

    [HttpPost("run")]
    public ActionResult<SimulationResponse> Run([FromBody] SimulationRequest? request)
    {
        var payload = request ?? new SimulationRequest();

        try
        {
            var response = _simulationService.Run(payload);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
