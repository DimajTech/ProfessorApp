using Microsoft.AspNetCore.Http.HttpResults;
using StudentApp.Models.Entity;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;


namespace StudentApp.Models.DAO
{
    public class AdvisementDAO
    {
        private readonly IConfiguration _configuration;
        string connectionString;

        public AdvisementDAO(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public int Create(Advisement advisement)
        {
            int result = 0; //Saves 1 or 0 depending on the insertion result
            using (SqlConnection connection = new SqlConnection(connectionString))
                try
                {
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("CreateAdvisement", connection); //todo stored procedure
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString());
                        command.Parameters.AddWithValue("@CourseId", advisement.Course.Id);
                        command.Parameters.AddWithValue("@Content", advisement.Content);
                        command.Parameters.AddWithValue("@Status", advisement.Status);
                        command.Parameters.AddWithValue("@IsPublic", advisement.IsPublic);
                        command.Parameters.AddWithValue("@StudentId", advisement.User.Id);
                        DateTime createdAt = advisement.CreatedAt == DateTime.MinValue ? DateTime.Now : advisement.CreatedAt;
                        command.Parameters.AddWithValue("@CreatedAt", createdAt);

                        result = command.ExecuteNonQuery();
                        connection.Close();

                    }
                }
                catch (SqlException e)
                {
                    throw;
                }

            return result;

        }


        public Advisement GetById(string id)
        {
            Advisement advisement = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("GetAdvisementById", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Id", id);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        advisement = new Advisement
                        {
                            Id = reader["Id"].ToString(),
                            Course = new Course(
                                reader["CourseCode"].ToString(),
                                reader["CourseName"].ToString(),
                                null, null, 0, true
                            ), // constructor sobrecargado 
                            Content = reader["Content"].ToString(),
                            Status = reader["Status"].ToString(),
                            IsPublic = reader["IsPublic"] != DBNull.Value && Convert.ToBoolean(reader["IsPublic"]),
                            User = new User(
                                        reader["UserId"] != DBNull.Value ? reader["UserId"].ToString() : string.Empty,
                                        reader["UserName"] != DBNull.Value ? reader["UserName"].ToString() : string.Empty
                                    ),
                            CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : DateTime.MinValue
                        };
                    }
                    connection.Close();
                }
                catch (SqlException)
                {
                    throw;
                }
            }
            return advisement;
        }

        public IEnumerable<Advisement> GetByUser(string email)
        {
            List<Advisement> advisements = new List<Advisement>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("GetAdvisementByUser", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Email", email);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Advisement advisement = new Advisement
                                {
                                    Id = reader["Id"].ToString(),
                                    Course = new Course(
                                        reader["CourseCode"].ToString(),
                                        reader["CourseName"].ToString(),
                                        null, null, 0, true
                                    ),
                                    Content = reader["Content"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    IsPublic = reader["IsPublic"] != DBNull.Value && Convert.ToBoolean(reader["IsPublic"]),
                                    User = new User(
                                        reader["UserId"] != DBNull.Value ? reader["UserId"].ToString() : string.Empty,
                                        reader["UserName"] != DBNull.Value ? reader["UserName"].ToString() : string.Empty
                                    ),
                                    CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : DateTime.MinValue
                                };
                                advisements.Add(advisement);
                            }
                        }
                    }
                    connection.Close();
                }
                catch (SqlException)
                {
                    throw;
                }

                return advisements;
            }
        }



        public IEnumerable<Advisement> GetPublicAdvisements(string email) //TODO: que no traiga las del email del estudiante logeado
        {
            List<Advisement> advisements = new List<Advisement>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("GetPublicAdvisements", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Email", email);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Advisement advisement = new Advisement
                                {
                                    Id = reader["Id"].ToString(),
                                    Course = new Course(
                                        reader["CourseCode"].ToString(),
                                        reader["CourseName"].ToString(),
                                        null, null, 0, true
                                    ),
                                    Content = reader["Content"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    IsPublic = reader["IsPublic"] != DBNull.Value && Convert.ToBoolean(reader["IsPublic"]),
                                    User = new User(
                                        reader["UserId"] != DBNull.Value ? reader["UserId"].ToString() : string.Empty,
                                        reader["UserName"] != DBNull.Value ? reader["UserName"].ToString() : string.Empty
                                    ),
                                    CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : DateTime.MinValue
                                };

                                advisements.Add(advisement);
                            }
                        }
                    }
                    connection.Close();
                }
                catch (SqlException)
                {
                    throw;
                }


                return advisements;
            }

        }


        public List<ResponseAdvisement> GetAdvisementResponsesById(string id)
        {
            List<ResponseAdvisement> comments = new List<ResponseAdvisement>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("GetAdvisementResponsesById", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    comments.Add(new ResponseAdvisement
                    {
                        Id = reader["Id"].ToString(),
                        Text = reader["Text"].ToString(),
                        DateTime = DateTime.Parse(reader["Date"].ToString()),
                        User = new User(null, reader["Name"].ToString(), null, null, reader["Role"].ToString()),
                    });
                }
                connection.Close();
            }
            return comments;
        }

        public int InsertNewResponse(ResponseAdvisement advisement)
        {
            int result = 0; //Saves 1 or 0 depending on the insertion result
            using (SqlConnection connection = new SqlConnection(connectionString))
                try
                {
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("InsertNewResponse", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@AdvisementId", advisement.AdvisementId); // Ajustado al SP
                        command.Parameters.AddWithValue("@UserId", advisement.User.Id); // Ajustado al SP
                        command.Parameters.AddWithValue("@Text", advisement.Text); // Ajustado al SP

                        result = command.ExecuteNonQuery();
                        connection.Close();


                    }
                }
                catch (SqlException e)
                {
                    throw;
                }

            return result;

        }
    }

}