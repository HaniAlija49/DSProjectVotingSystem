using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResultsService.Data;
using ResultsService.Models;

namespace ResultsService.Controllers
{
    [ApiController]
    [Route("api/results")]
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{electionId}")]
        [Authorize]
        public async Task<IActionResult> GetResults(int electionId)
        {
            var results = await _context.Results
                .Where(r => r.ElectionId == electionId)
                .GroupBy(r => r.CandidateId)
                .Select(g => new
                {
                    CandidateId = g.Key,
                    VoteCount = g.Count()
                })
                .ToListAsync();

            return Ok(results);
        }
    }
}
