using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Entities.Models.Dto
{
    public class PollCreateDto
    {
        [Required]
        public int EventId { get; set; }
        public required List<string> Options { get; set; }
    }
}