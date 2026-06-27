using System.Text;
using System.Text.Json;
using ChatYar.Services;
namespace ChatYar.Services;
//<summary>
// Groq API service
//</summary>
public class GroqChatService : IChatService
{
	// HTTP client for sending requests
	private readonly HttpClient _httpClient;
	// API key - get it from console.groq.com
	private readonly string _apiKey = "Enter your API key";
	// API endpoint URL
	private readonly string _apiUrl = "https://api.groq.com/openai/v1/chat/completions";

	// <summary>
	// Constructor - sets the Authorization header
	// </summary>
	public GroqChatService(HttpClient httpClient)
	{
		_httpClient = httpClient;
		_httpClient.DefaultRequestHeaders.Authorization =
	new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
	}
	// <summary>
	// Sends user message to API and gets the AI response
	// </summary>
	// <param name="userMessage">The user's input message</param>
	// <returns>AI generated response</returns>
	public async Task<string> GetResponseAsync(string userMessage)
	{
		try
		{
			// Build the request body
			var requestBody = new
			{
				messages = new[]
				{
					new { role = "system", content = "You are a helpful assistant. پاسخ‌ها را به زبان فارسی بده." },
					new { role = "user", content = userMessage }
				},
				model = "llama-3.3-70b-versatile",
				temperature = 0.7,
				max_tokens = 500
			};
			// Send the request
			var json = JsonSerializer.Serialize(requestBody);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			var response = await _httpClient.PostAsync(_apiUrl, content);
			response.EnsureSuccessStatusCode();
			// Process the response
			var jsonResponse = await response.Content.ReadAsStringAsync();
			using var doc = JsonDocument.Parse(jsonResponse);
			var botReply = doc.RootElement
				.GetProperty("choices")[0]
				.GetProperty("message")
				.GetProperty("content")
				.GetString();
			return botReply ?? "متاسفم، پاسخی دریافت نشد.";
		}
		catch (Exception ex)
		{
			return $"خطا: {ex.Message}";

		}
	}
}
