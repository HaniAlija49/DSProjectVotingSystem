using ElectionsService.Data;
using ElectionsService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectionsService.Controllers
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
            var elections = await _context.Elections
                .Include(e => e.Candidates)
                .ToListAsync();

            var result = elections.Select(e => new
            {
                e.ElectionId,
                e.Name,
                e.StartDate,
                e.EndDate,
                Candidates = e.Candidates.Select(c => new
                {
                    c.CandidateId,
                    c.Name,
                    c.Party
                }).ToList()
            }).ToList();

            return Ok(result);
        }


        [HttpPost("{id}/candidates")]
        [Authorize]
        public async Task<IActionResult> AddCandidates(int id, [FromBody] CandidateModel model)
        {
            var election = await _context.Elections.FindAsync(id);
            if (election == null)
            {
                return NotFound(new { Message = "Election not found" });
            }

            var candidate = new Candidate
            {
                ElectionId = id,
                Name = model.Name,
                Party = model.Party
            };

            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Candidate added successfully", CandidateId = candidate.CandidateId });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetElections(int id)
        {
            var election = await _context.Elections
                .Include(e => e.Candidates)
                .FirstOrDefaultAsync(e => e.ElectionId == id);

            if (election == null)
            {
                return NotFound(new { Message = "Election not found" });
            }

            var result = new
            {
                election.ElectionId,
                election.Name,
                election.StartDate,
                election.EndDate,
                Candidates = election.Candidates.Select(c => new
                {
                    c.CandidateId,
                    c.Name,
                    c.Party
                }).ToList()
            };

            return Ok(result);
        }

    }
}
