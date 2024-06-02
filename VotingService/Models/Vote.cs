namespace VotingService.Models
{
    public class Vote
    {
        public int VoteId { get; set; }
        public string UserId { get; set; }
        public int ElectionId { get; set; }
        public int CandidateId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
