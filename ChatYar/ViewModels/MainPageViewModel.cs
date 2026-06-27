using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ChatYar.Models;
using ChatYar.Services;
using System.Collections.ObjectModel;

namespace ChatYar.ViewModels;
// <summary>
// ViewModel for the main chat page - handles user input, messages, and chat actions
// </summary>
public partial class MainPageViewModel : ObservableObject
{
	#region Private Fields
	private readonly IChatService _chatService;
	private readonly LocalDbService _dbService;

	[ObservableProperty]
	private string _userInput = string.Empty;

	[ObservableProperty]
	private ObservableCollection<ChatMessage> _messages = new();

	[ObservableProperty]
	private bool _isBusy;

	[ObservableProperty]
	private bool _isWelcomeVisible = true;

	#endregion
	#region Constructor
	public MainPageViewModel(IChatService chatService, LocalDbService dbService)
	{
		_chatService = chatService;
		_dbService = dbService;
		LoadMessages();
	}
	#endregion
	#region Message Loading
	// <summary>
	// Loads saved messages from local database
	// </summary>
	private async void LoadMessages()
	{
		var msgs = await _dbService.GetMessagesAsync();
		foreach (var msg in msgs)
		{
			Messages.Add(msg);
		}
		IsWelcomeVisible = Messages.Count == 0;
	}
	#endregion
	#region Commands
	// <summary>
	// Sends user message to bot and get response
	// </summary>
	[RelayCommand]
	private async Task SendMessage()
	{
		if (string.IsNullOrWhiteSpace(UserInput) || IsBusy)
			return;
		// Save and send user message
		var userMessage = new ChatMessage
		{
			Text = UserInput,
			IsUser = true
		};

		Messages.Add(userMessage);
		await _dbService.SaveMessageAsync(userMessage);

		IsWelcomeVisible = false;

		var userText = UserInput;
		UserInput = string.Empty;
		IsBusy = true;

		try
		{
			// Get bot response
			var botReply = await _chatService.GetResponseAsync(userText);

			var botMessage = new ChatMessage
			{
				Text = botReply,
				IsUser = false
			};

			Messages.Add(botMessage);
			await _dbService.SaveMessageAsync(botMessage);
		}
		catch (Exception ex)
		{
			await Application.Current.MainPage.DisplayAlert("خطا", ex.Message, "باشه");
		}
		finally
		{
			IsBusy = false;
		}
	}
	// <summary>
	// Clears all messages and starts a new conversation
	// </summary>
	[RelayCommand]
	private async Task NewConversation()
	{
		var confirm = await Application.Current.MainPage.DisplayAlert(
			"شروع مکالمه جدید",
			"آیا از پاک کردن همه پیام‌ها مطمئن هستید؟",
			"بله",
			"خیر"
		);

		if (confirm)
		{
			await _dbService.DeleteAllMessagesAsync();
			Messages.Clear();
			IsWelcomeVisible = true;

			await Application.Current.MainPage.DisplayAlert(
				"✅",
				"مکالمه جدید شروع شد!",
				"باشه"
			);
		}
	}
	#endregion
	#region Message Actions
	// <summary>
	// Copies message text to clipboard
	// </summary>
	[RelayCommand]
	private async Task CopyMessage(ChatMessage message)
	{

		try
		{


			await Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.SetTextAsync(message.Text);
			await Application.Current.MainPage.DisplayAlert("✅", "متن کپی شد!", "باشه");
		}
		catch (Exception ex)
		{
		}
	}

	// <summary>
	// Like message
	// </summary>
	[RelayCommand]
	private async Task LikeMessage(ChatMessage message)
	{
		
		if (message.IsLiked)
		{
			message.IsLiked = false;
			await Application.Current.MainPage.DisplayAlert("💬", "لایک برداشته شد!", "باشه");
		}
		else
		{
			
			message.IsLiked = true;

			
			if (message.IsDisliked)
			{
				message.IsDisliked = false;
			}

			await Application.Current.MainPage.DisplayAlert("👍", "پیام مفید بود! ✅", "باشه");
		}

		// Save in database
		await _dbService.UpdateMessageAsync(message);
	}
	// <summary>
	// Dislike message
	// </summary>
	[RelayCommand]
	private async Task DislikeMessage(ChatMessage message)
	{
		
		if (message.IsDisliked)
		{
			message.IsDisliked = false;
			await Application.Current.MainPage.DisplayAlert("💬", "دیسلایک برداشته شد!", "باشه");
		}
		else
		{
		
			message.IsDisliked = true;

		
			if (message.IsLiked)
			{
				message.IsLiked = false;
			}

			await Application.Current.MainPage.DisplayAlert("👎", "پیام نیاز به بهبود دارد! 🔧", "باشه");
		}

		// Save in database
		await _dbService.UpdateMessageAsync(message);
	}
	#endregion
}