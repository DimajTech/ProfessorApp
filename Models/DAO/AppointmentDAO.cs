using Microsoft.Data.SqlClient;
using StudentApp.Models.Entity;

namespace StudentApp.Models.DAO
{
    public class AppointmentDAO
    {
        private readonly IConfiguration _configuration;
        string connectionString;

        public AppointmentDAO(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        public int CreateAppointment(Appointment appointment)
        {
            int result = 0; 

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {

                    connection.Open();
                    using (SqlCommand command = new SqlCommand("CreateAppointment", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString());
                        command.Parameters.AddWithValue("@Date", appointment.Date);
                        command.Parameters.AddWithValue("@Mode", appointment.Mode);
                        command.Parameters.AddWithValue("@Status", "pending");
                        command.Parameters.AddWithValue("@CourseId", appointment.Course.Id);
                        command.Parameters.AddWithValue("@StudentId", appointment.User.Id); 

                        result = command.ExecuteNonQuery();
                        connection.Close();

                    }
                }
                catch (SqlException e)
                {
                    
                    throw; //se lanza excepcion para ser manejada en el controller
                }
            }

            return result;
        }



        public Appointment GetAppointment(Guid id)
        {
            Appointment appointment = null;

      
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("GetAppointmentById", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Id", id);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read()) 
                    {
                        appointment = new Appointment
                        {
                            Id = reader["Id"] != DBNull.Value ? Guid.Parse(reader["Id"].ToString()) : Guid.Empty,
                            Date = reader["Date"] != DBNull.Value ? Convert.ToDateTime(reader["Date"]) : DateTime.MinValue,
                            Mode = reader["Mode"].ToString(),
                            Status = reader["Status"].ToString(),
                            Course = new Course(
                                reader["Code"].ToString(),
                                reader["Name"].ToString(),
                                null, null, 0, true // 0 porque no deja pasar null en year, se debe sobrecargar constructor.
                            )
                        };
                    }
                    connection.Close();
                }
                catch (SqlException) 
                {
                    throw; 
                }
                return appointment;
            }
        }

        //El estudiente puede ver todos las horas consultas solicitadas

        public List<Appointment> GetAll(string email)
        {
            List<Appointment> appointments = new List<Appointment>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand("GetAppointmentsByStudent", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Email", email);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        appointments.Add(new Appointment
                        {
                            Id = reader["Id"] != DBNull.Value ? Guid.Parse(reader["Id"].ToString()) : Guid.Empty,
                            Date = Convert.ToDateTime(reader["Date"]),
                            Mode = reader["Mode"].ToString(),
                            Status = reader["Status"].ToString(),
                            Course = new Course(
                                reader["Code"].ToString(),
                                reader["Name"].ToString(),
                                null, null, 0, true
                            )
                        });
                    }
                    connection.Close();
                }
                catch (SqlException) 
                {
                    throw; 
                }
                return appointments;
            }
          
        }


    }
}
