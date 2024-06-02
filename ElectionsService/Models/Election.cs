using System.ComponentModel.DataAnnotations;

namespace ElectionsService.Models
{
    public class Election
    {
        [Key]
        public int ElectionId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public ICollection<Candidate> Candidates { get; set; }
    }
}
