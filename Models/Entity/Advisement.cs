namespace StudentApp.Models.Entity
{
    public class Advisement
    {
        private string id;
        private Course course;
        private string content;
        private string status;
        private bool isPublic;
        private User user; //Student
        private DateTime createdAt;

        public Advisement()
        {
        }

        public Advisement( Course course, string content, bool isPublic, User user) //Constructor para registro de una consulta
        {
            id = Guid.NewGuid().ToString();
            this.course = course;
            this.content = content;
            status = "Pending"; //estado inicial
            this.isPublic = isPublic;
            this.user = user;
            createdAt = DateTime.Now;
        }

        public string Id { get => id; set => id = value; }
        public Course Course { get => course; set => course = value; }
        public string Content { get => content; set => content = value; }
        public string Status { get => status; set => status = value; }
        public bool IsPublic { get => isPublic; set => isPublic = value; }
        public User User { get => user; set => user = value; }
        public DateTime CreatedAt { get => createdAt; set => createdAt = value; }
    }
}
