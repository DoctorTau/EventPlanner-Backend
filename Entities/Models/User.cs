using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace EventPlanner.Entities.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public long TelegramId { get; set; }

        [MaxLength(255)]
        public required string Username { get; set; }

        [MaxLength(255)]
        public required string FirstName { get; set; }

        [MaxLength(255)]
        public required string LastName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual required ICollection<Event> CreatedEvents { get; set; }
        public virtual required ICollection<Participant> Participations { get; set; }
        public virtual required ICollection<Task> AssignedTasks { get; set; }
        public virtual required ICollection<File> UploadedFiles { get; set; }
        public virtual required ICollection<Vote> Votes { get; set; }
        public virtual required ICollection<LLMGeneratedPlan> GeneratedPlans { get; set; }
    }
}
