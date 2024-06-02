using ElectionService.Data;
using ElectionService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectionService.Controllers
{
    [ApiController]
    [Route("api/elections")]
    public class ElectionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ElectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateElection([FromBody] ElectionModel model)
        {
            var election = new Election
            {
                Name = model.Name,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            _context.Elections.Add(election);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Election created successfully", ElectionId = election.ElectionId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetElections()
        {
            var elections = await _context.Elections.ToListAsync();
            return Ok(elections);
        }
    }
}
