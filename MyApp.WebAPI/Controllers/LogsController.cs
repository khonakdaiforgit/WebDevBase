using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Authorization;
using MyApp.Application.Abstractions.Logging;
using MyApp.Application.Abstractions.Logging.Dtos;
using MyApp.Application.Common;
using MyApp.WebAPI.Extensions;

namespace MyApp.WebAPI.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    [Route("api/logs")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly IAuthorizationService _authz;

        public LogsController(ILogService logService, IAuthorizationService authz)
        {
            _logService = logService;
            _authz = authz;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<LogEntryDto>>> Get(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string? level,
            [FromQuery] string? project,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var callerId = this.GetUserId();
            if (!await _authz.CanViewLogsAsync(callerId))
                return Forbid();

            var result = await _logService.GetLogsAsync(from, to, level, project, null, page, pageSize);
            return Ok(result);
        }

        [HttpGet("analytics")]
        public async Task<ActionResult<object>> GetAnalytics(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string? path)
        {
            var callerId = this.GetUserId();
            if (!await _authz.IsOwnerAsync(callerId))
                return Forbid();

            var logs = await _logService.GetLogsAsync(from, to, project: "API");

            var stats = logs.Items
                .Where(l => string.IsNullOrEmpty(path) || l.Path?.Contains(path) == true)
                .GroupBy(l => l.Path)
                .Select(g => new
                {
                    Path = g.Key,
                    Visits = g.Count(),
                    UniqueUsers = g.Select(x => x.HashedIp).Distinct().Count(),
                    AvgDuration = g.Average(x => x.DurationMs ?? 0)
                })
                .OrderByDescending(x => x.Visits);

            return Ok(stats);
        }

        // در LogsController
        [HttpGet("top-pages")]
        public async Task<IActionResult> TopPages(int top = 10)
        {
            var topPage = await _logService.GetLogsAsync(project: "API");
            var stats = topPage
                .Items
                .GroupBy(l => l.Path)
                .OrderByDescending(g => g.Count())
                .Take(top)
                .Select(g => new { Path = g.Key, Views = g.Count() });

            return Ok(stats);
        }
    }
}
