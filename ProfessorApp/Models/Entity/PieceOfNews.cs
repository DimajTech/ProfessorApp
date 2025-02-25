namespace StudentApp.Models.Entity
{
	public class PieceOfNews
	{
		//private Guid id;
		private string id;
		private DateOnly date;
		private string title;
		private string file;
		private string picture;
		private User user; //author? (Id, Name) incluiría role
		private string description;
		private string authorRole;

		public PieceOfNews()
		{
		}


		public string Id { get => id; set => id = value; }
		public string Title { get => title; set => title = value; }
		public string File { get => file; set => file = value; }
		public string Picture { get => picture; set => picture = value; }
		public User User { get => user; set => user = value; }
		public string Description { get => description; set => description = value; }
		public string AuthorRole { get => authorRole; set => authorRole = value; }
		public DateOnly Date { get => date; set => date = value; }
	}
}
