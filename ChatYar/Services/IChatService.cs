using System;

namespace ChatYar.Services
{
	public interface IChatService
	{
		Task<string> GetResponseAsync(string userMessage);
	}
}
