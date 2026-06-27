
using System.ComponentModel.DataAnnotations.Schema;
using SQLite;
namespace ChatYar.Models
{

	/// <summary>
	/// its our model
	/// </summary>
	[SQLite.Table("Messages")]
	public class ChatMessage
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string Text { get; set; } = string.Empty;

		public bool IsUser { get; set; } //True is user - false is bot

		public DateTime Timestamp { get; set; } = DateTime.Now;
		public bool IsLiked { get; set; }
		public bool IsDisliked { get; set; }
	}
}
