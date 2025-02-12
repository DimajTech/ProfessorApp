namespace StudentApp.Models.Entity
{
    public class CommentNewsResponse
    {
        private string id;
        private CommentNews commentNews;
        private User user;
        private string text;
        private DateTime dateTime;

        public CommentNewsResponse()
        {
        }

        public CommentNewsResponse(CommentNews pieceOfNews, User user, string text, DateTime dateTime)
        {
            id = Guid.NewGuid().ToString();
            this.commentNews = pieceOfNews;
            this.user = user;
            this.text = text;
            this.dateTime = dateTime;
        }

        public string Id { get => id; set => id = value; }
        public CommentNews CommentNews { get => commentNews; set => commentNews = value; }
        public User User { get => user; set => user = value; }
        public string Text { get => text; set => text = value; }
        public DateTime DateTime { get => dateTime; set => dateTime = value; }
    }
}
