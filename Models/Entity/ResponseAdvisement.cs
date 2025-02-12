namespace StudentApp.Models.Entity
{
    public class ResponseAdvisement
    {
        private string id;
        //references to an object or just the id?
        private string advisementId; //references to other DB
        private User user;
        private string text;
        private DateTime date;

        public ResponseAdvisement()
        {
        }
        
        //todo constructor
        public string Id { get => id; set => id = value; }
        public string AdvisementId { get => advisementId; set => advisementId = value; }
        public string Text { get => text; set => text = value; }
        public DateTime DateTime { get => date; set => date = value; }
        public User User { get => user; set => user = value; }
    }
}
