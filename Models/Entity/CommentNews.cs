namespace StudentApp.Models.Entity
{
    public class CommentNews
    {
        private string id;
        private PieceOfNews pieceOfNews;
        private User user;
        private string text;
        private DateTime dateTime;
        private int totalResponses;

        public CommentNews()
        {
        }

        public CommentNews(PieceOfNews pieceOfNews, User user, string text, DateTime dateTime)
        {
            id = Guid.NewGuid().ToString();
            this.pieceOfNews = pieceOfNews;
            this.user = user;
            this.text = text;
            this.dateTime = dateTime;
        }

        public string Id { get => id; set => id = value; }
        public PieceOfNews PieceOfNews { get => pieceOfNews; set => pieceOfNews = value; }
        public User User { get => user; set => user = value; }
        public string Text { get => text; set => text = value; }
        public DateTime DateTime { get => dateTime; set => dateTime = value; }
        public int TotalResponses { get => totalResponses; set => totalResponses = value; }
    }
}
