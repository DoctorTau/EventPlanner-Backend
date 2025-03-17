using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Entities.Models.Dto
{
    public class VoteCreateDto
    {
        public int UserId { get; set; }
        [Required]
        public int VotingId { get; set; }

        public VoteType Type { get; set; } = VoteType.Date;
        public required string VoteOption { get; set; }
    }
}