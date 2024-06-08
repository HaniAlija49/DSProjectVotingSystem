using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResultService.Data;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResultService.Models;

namespace ResultService.Controllers
{
    [ApiController]
    [Route("api/results")]
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusReceiver _receiver;

        public ResultsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

            var connectionString = _configuration["ServiceBus:ConnectionString"];
            var queueName = _configuration["ServiceBus:QueueName"];
            _client = new ServiceBusClient(connectionString);
            _receiver = _client.CreateReceiver(queueName);
        }

        private async Task<bool> ReceiveMessagesAsync()
        {
            IReadOnlyList<ServiceBusReceivedMessage> receivedMessages = await _receiver.ReceiveMessagesAsync(10, TimeSpan.FromSeconds(1));
            if (receivedMessages == null || receivedMessages.Count == 0)
            {
                return false;
            }

            foreach (ServiceBusReceivedMessage receivedMessage in receivedMessages)
            {
                string body = receivedMessage.Body.ToString();
                var vote = JsonConvert.DeserializeObject<Vote>(body);
                await _context.Votes.AddAsync(vote);
                await _context.SaveChangesAsync();
                await _receiver.CompleteMessageAsync(receivedMessage);
            }

            return true;
        }

        [HttpGet("{electionId}")]
        [Authorize]
        public async Task<IActionResult> GetResults(int electionId)
        {
            bool messagesProcessed = await ReceiveMessagesAsync();

            var results = await _context.Votes
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
