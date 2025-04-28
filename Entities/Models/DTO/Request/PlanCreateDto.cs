
namespace EventPlanner.Entities.Models.Dto
{
    public class PlanCreateDto
    {
        public string title { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        public string event_date { get; set; }
        public string event_type { get; set; }
        public int participants { get; set; }
        public string user_prompt { get; set; }


        public PlanCreateDto(Event eventToAddPlan, string prompt)
        {
            title = eventToAddPlan.Title;
            description = eventToAddPlan.Description;
            location = eventToAddPlan.Location ?? string.Empty;
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