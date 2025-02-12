namespace StudentApp.Models.Entity
{
    public class Course
    {
        private Guid id; //Si es solo para traer el dato debería ser string
        private string code;
        private string name;
        private string professorId;
        private string semester;
        private int year;
        private bool isActive;

        public Course()
        {
        }



        public Course(string code, string name, string professorId, string semester, int year, bool isActive)
        {
            id = Guid.NewGuid();
            this.code = code;
            this.name = name;
            this.professorId = professorId;
            this.semester = semester;
            this.year = year;
            this.isActive = isActive;
        }

        public Course(Guid id)
        {
            this.id = id;
        } // se usa para crear una consulta en el controller de AdvisementController

        public Guid Id { get => id; set => id = value; }
        public string Code { get => code; set => code = value; }
        public string Name { get => name; set => name = value; }
        public string ProfessorId { get => professorId; set => professorId = value; }
        public string Semester { get => semester; set => semester = value; }
        public int Year { get => year; set => year = value; }
        public bool IsActive { get => isActive; set => isActive = value; }
    }
}
