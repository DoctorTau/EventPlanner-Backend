using System.Net.Http.Json;
using EventPlanner.Entities.Models;
using EventPlanner.Entities.Models.Dto;
using Microsoft.Extensions.Configuration;

namespace EventPlanner.Business
{
    public class PlanGenerator : IPlanGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly string _llmServiceUrl;

        public PlanGenerator(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _llmServiceUrl = configuration["LLMService:Url"] ?? throw new ArgumentNullException("LLMService:Url is not configured");
        }

        public async Task<string> GeneratePlanAsync(Event eventToAddPlan, string prompt)
        {
            try
            {
                PlanCreateDto planCreateDto = new PlanCreateDto(eventToAddPlan, prompt);

                var response = await _httpClient.PostAsJsonAsync($"{_llmServiceUrl}/plan/generate-plan", planCreateDto);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    throw new Exception($"Error from LLM service: {response.ReasonPhrase}");
                }
                // Read as json and get "plan_text" field
                var responseContent = await response.Content.ReadAsStringAsync();
                var planText = responseContent.Split('"')[3];
                Console.WriteLine($"Generated plan text: {planText}");
                return planText;
            }
            catch (HttpRequestException ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("Error while calling LLM service", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new Exception("An unexpected error occurred", ex);
            }
        }

        public async Task<string> ModifyPlanAsync(Event eventToModifyPlan, string planToModify, string prompt)
        {
            try
            {
                Console.WriteLine($"Event to modify plan: {eventToModifyPlan}");
                PlanUpdateDto planModifyDto = new PlanUpdateDto
                {
                    original_plan = planToModify,
                    user_comment = prompt
                };

                var response = await _httpClient.PostAsJsonAsync($"{_llmServiceUrl}/plan/update-plan", planModifyDto);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    throw new Exception($"Error from LLM service: {response.ReasonPhrase}");
                }
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("Error while calling LLM service", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw new Exception("An unexpected error occurred", ex);
            }
        }
    }
}