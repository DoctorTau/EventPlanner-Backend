
namespace EventPlanner.Entities.Models.Dto
{
    public class PlanCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string event_date { get; set; }
        public string event_type { get; set; }
        public int participants { get; set; }
        public string user_prompt { get; set; }


        public PlanCreateDto(Event eventToAddPlan, string prompt)
        {
            Title = eventToAddPlan.Title;
            Description = eventToAddPlan.Description;
            Location = eventToAddPlan.Location ?? string.Empty;
            event_date = eventToAddPlan.EventDate?.ToString("yyyy-MM-dd") ?? string.Empty;
            event_type = eventToAddPlan.EventType.ToString();
            participants = eventToAddPlan.Participants.Count;
            user_prompt = prompt;
        }
    }

    public class PlanUpdateDto
    {
        public required string original_plan { get; set; }
        public required string user_comment { get; set; }
    }
}