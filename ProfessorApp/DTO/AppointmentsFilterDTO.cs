namespace ProfessorApp.DTO
{
    public class AppointmentsFilterDTO
    {
        public DateOnly? Date { get; set; }
        public string ProfessorId { get; set; }
        public string State { get; set; }
    }
}
