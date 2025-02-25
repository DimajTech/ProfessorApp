using Microsoft.Data.SqlClient;
using StudentApp.Models.Entity;

namespace StudentApp.Models.DAO 
{ 
    public class CourseDAO
    {
        private readonly IConfiguration _configuration;
        string connectionString;

        public CourseDAO(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public List<Course> Get()
        {
            List<Course> courses = new List<Course>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("GetAllAvailableCourses", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            courses.Add(new Course
                            {
                                Id = reader["Id"] != DBNull.Value ? Guid.Parse(reader["Id"].ToString()) : Guid.Empty,
                                Code = reader["Code"].ToString(),
                                Name = reader["Name"].ToString(),
                                ProfessorId = reader["ProfessorId"].ToString(), // hay campos que en la bd pueden ser null ->  aca deberia manejarse algo para esos casos
                                Semester = reader["Semester"].ToString(),
                                Year = Convert.ToInt32(reader["Year"]),
                                IsActive = true
                            });
                        }
                    }
                }
                connection.Close();
            }

            return courses;
        }

        public Course GetByCode(string code)
        {
            Course course = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("GetCourseByCode", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Code", code);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) 
                        {
                            course = new Course
                            {
                                Id = reader["Id"] != DBNull.Value ? Guid.Parse(reader["Id"].ToString()) : Guid.Empty,
                                Code = reader["Code"].ToString(),
                                Name = reader["Name"].ToString(),
                                ProfessorId = reader["ProfessorId"].ToString(),
                                Semester = reader["Semester"].ToString(),
                                Year = Convert.ToInt32(reader["Year"]),
                                IsActive = true
                            };
                        }
                    }
                }
                connection.Close(); 
            }

            return course;
        }
     

    }
}
