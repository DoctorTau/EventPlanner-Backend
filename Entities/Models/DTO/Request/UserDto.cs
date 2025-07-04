namespace EventPlanner.Entities.Models.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public long TelegramId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}