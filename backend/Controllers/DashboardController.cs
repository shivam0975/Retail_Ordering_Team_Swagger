using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await dashboardService.GetSummaryAsync();

        return Ok(new ApiResponseDto<DashboardSummaryDto>
        {
            Success = true,
            Message = "Dashboard summary fetched successfully.",
            Data = summary
        });
    }
}
