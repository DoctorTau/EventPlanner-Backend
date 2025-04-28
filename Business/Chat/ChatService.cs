using System.Net.Http.Json;
using EventPlanner.Entities.Models.Dto;
using EventPlanner.Repository;
using Microsoft.Extensions.Configuration;

namespace EventPlanner.Business
{
    public class ChatService : IChatService
    {
        private readonly IEventsRepository _eventRepository;
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;
        private readonly HttpClient _httpClient;
        private readonly string _chatServiceUrl;

        public ChatService(IEventsRepository eventRepository,
                           ITaskService taskService,
                           IUserService userService,
                           HttpClient httpClient,
                           IConfiguration configuration)
        {
            _eventRepository = eventRepository;
            _taskService = taskService;
            _userService = userService;
            _httpClient = httpClient;
            _chatServiceUrl = configuration["ChatService:Url"] ??
                              throw new ArgumentNullException("ChatService:Url is not configured");
        }

        public async Task CreatePollAsync(BotPollCreateDto pollCreateDto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_chatServiceUrl}/create-poll", pollCreateDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task SendSummaryMessageAsync(int eventId)
        {
            try
            {
                var eventToSendSummary = await _eventRepository.GetEventWithDetailsAsync(eventId);
                if (eventToSendSummary == null)
                    throw new KeyNotFoundException($"Event with id {eventId} not found");

                SummaryMessageDto summaryMessageDto = new SummaryMessageDto
                {
                    ChatId = eventToSendSummary.TelegramChatId,
                    Title = eventToSendSummary.Title,
                    Description = eventToSendSummary.Description,
                };

                if (eventToSendSummary.EventDate != null)
                    summaryMessageDto.Date = eventToSendSummary.EventDate.Value.ToString("yyyy-MM-dd");
                else
                    summaryMessageDto.Date = "To be decided";
                if (eventToSendSummary.Location != null)
                    summaryMessageDto.Location = eventToSendSummary.Location;
                else
                    summaryMessageDto.Location = "To be decided";

                var response = await _httpClient.PostAsJsonAsync($"{_chatServiceUrl}/send-event-summary", summaryMessageDto);
            }
            catch (HttpRequestException ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("Error while calling Chat service", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new Exception("An unexpected error occurred", ex);
            }
        }
    }
}