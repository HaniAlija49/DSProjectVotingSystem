using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VotingService.Models;
using VotingService.Data;
using Microsoft.EntityFrameworkCore;

namespace VotingService.Controllers
{
    [ApiController]
    [Route("api/voting")]
    public class VotingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VotingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("vote")]
        [Authorize]
        public async Task<IActionResult> SubmitVote([FromBody] VoteModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var hasVoted = await _context.Votes.AnyAsync(v => v.UserId == userId && v.ElectionId == model.ElectionId);
            if (hasVoted)
                return BadRequest("User has already voted.");

            var vote = new Vote
            {
                UserId = userId,
                ElectionId = model.ElectionId,
                CandidateId = model.CandidateId,
                Timestamp = DateTime.UtcNow
            };

            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Vote recorded successfully" });
        }
    }
}
