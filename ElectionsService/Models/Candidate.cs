using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ElectionsService.Models
{
    public class Candidate
    {
        [Key]
        public int CandidateId { get; set; }

        [Required]
        public int ElectionId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Party { get; set; }

        [ForeignKey("ElectionId")]
        public Election Election { get; set; }
    }
}
