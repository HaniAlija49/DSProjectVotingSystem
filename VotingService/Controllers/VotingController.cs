using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VotingService.Models;
using VotingService.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace VotingService.Controllers
{
    [ApiController]
    [Route("api/voting")]
    public class VotingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public VotingController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("vote")]
        [Authorize]
        public async Task<IActionResult> SubmitVote([FromBody] VoteModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var hasVoted = await _context.Votes.AnyAsync(v => v.UserId == userId && v.ElectionId == model.ElectionId);
            if (hasVoted)
                return BadRequest("User has already voted.");

            // Validate ElectionId and CandidateId
            if (!await ValidateElectionId(model.ElectionId) || !await ValidateCandidateId(model.CandidateId))
                return BadRequest("Invalid ElectionId or CandidateId.");

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

        [HttpGet("vote/status")]
        [Authorize]
        public async Task<IActionResult> CheckVoteStatus([FromQuery] int electionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var hasVoted = await _context.Votes.AnyAsync(v => v.UserId == userId && v.ElectionId == electionId);
            if (hasVoted)
            {
                var vote = await _context.Votes.FirstOrDefaultAsync(v => v.UserId == userId && v.ElectionId == electionId);
                return Ok(new { HasVoted = true, Vote = vote });
            }

            return Ok(new { HasVoted = false });
        }

        private async Task<bool> ValidateElectionId(int electionId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"http://electionservice/api/elections/{electionId}");
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> ValidateCandidateId(int candidateId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"http://candidateservice/api/candidates/{candidateId}");
            return response.IsSuccessStatusCode;
        }
    }
}
