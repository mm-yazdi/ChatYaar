using SQLite;
using ChatYar.Models;

namespace ChatYar.Services;

public class LocalDbService
{
	private readonly SQLiteAsyncConnection _database;

	public LocalDbService()
	{
		var dbPath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"chat.db3"
		);
		_database = new SQLiteAsyncConnection(dbPath);
		_database.CreateTableAsync<ChatMessage>().Wait();
	}

	// Receive all messages
	public async Task<List<ChatMessage>> GetMessagesAsync()
	{
		return await _database.Table<ChatMessage>()
			.OrderBy(m => m.Timestamp)
			.ToListAsync();
	}

	// Save the new message
	public async Task<int> SaveMessageAsync(ChatMessage message)
	{
		return await _database.InsertAsync(message);
	}
	public async Task UpdateMessageAsync(ChatMessage message)
	{
		await _database.UpdateAsync(message);
	}
	// Delete all messages
	public async Task DeleteAllMessagesAsync()
	{
		await _database.DeleteAllAsync<ChatMessage>();
	}
}