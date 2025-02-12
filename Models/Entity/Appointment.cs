using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace StudentApp.Models.Entity
{
    public class Appointment
    {
        private Guid id;
        private DateTime date;
        private string mode;
        private string status;
        private Course course;
        private User user;
        private string professorComment;

        public Appointment()
        {
        }
        public Appointment(DateTime date, string mode, Course course, User user)
        {
            Id = Guid.NewGuid();
            Date = date;
            Mode = mode;
            this.status = "pending"; //VALOR DEFAULT
            Course = course;
            User = user;
        }
        

        public Guid Id { get => id; set => id = value; }
        public DateTime Date { get => date; set => date = value; }
        public string Mode { get => mode; set => mode = value; }
        public string Status { get => status; set => status = value; }
        public Course Course { get => course; set => course = value; }
        public User User { get => user; set => user = value; }
    }
}
