using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Entities.Models.Dto
{
    public class VoteCreateDto
    {
        public int UserId { get; set; }
        [Required]
        public int PollId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Vote index must be greater than or equal to 0.")]
        public int VoteIndex { get; set; }
    }
}