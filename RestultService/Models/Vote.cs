using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResultService.Models
{
    public class Vote
    {
        [Key]
        public int Id { get; set; } 
        public int VoteId { get; set; }
        public string UserId { get; set; }
        public int ElectionId { get; set; }
        public int CandidateId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
