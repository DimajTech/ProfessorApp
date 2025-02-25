using Microsoft.Data.SqlClient;
using StudentApp.Models.Entity;
using System.Data;

namespace StudentApp.Models.DAO
{
    public class UserDAO
    {
        private readonly IConfiguration _configuration;
        string connectionString;

        public UserDAO(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public int Insert(User user)
        {
            int result = 0; //Saves 1 or 0 depending on the insertion result
            using (SqlConnection connection = new SqlConnection(connectionString))
                try
                {
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("InsertUser", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        //TODO AddValue
                        command.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
                        command.Parameters.AddWithValue("@name", user.Name);
                        command.Parameters.AddWithValue("@email", user.Email);
                        command.Parameters.AddWithValue("@password", user.Password);
                        command.Parameters.AddWithValue("@role", "Student");

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
        public User Get(string email)
        {
            User user = new User();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("GetUserById", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id", email);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read()) //ask if an user has been found with the given id
                {
                   //TODO
                }
                connection.Close();
            }
            return user;
        }


        public User GetByEmail(string email)
        {
            User user = new User();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("GetUserByEmail", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@email", email);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        user.Id = reader.GetString(0);
                        user.Email = reader.GetString(1);
                        user.Password = reader.GetString(2);
                        user.IsActive = reader.GetBoolean(3);
                        user.RegistrationStatus = reader.GetString(4);
                        user.Role = reader.GetString(5);
                        user.Name = reader.GetString(6);

                        //Validación de los datos opcionales:

                        user.Description = reader.IsDBNull(7) ? "" : reader.GetString(7);
                        user.LinkedIn = reader.IsDBNull(8) ? "" : reader.GetString(8);
                        user.Picture = reader.IsDBNull(9) ? "/images/user.png" : reader.GetString(9);

                    }
                    connection.Close();
                }
                catch (SqlException)
                {
                    throw;
                }
            }
            return user;
        }


        public int Delete(string id)
        {
            int result = 0; //Saves 1 or 0 depending on the insertion result
            using (SqlConnection connection = new SqlConnection(connectionString))
                try
                {
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("DeleteUser", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", id);
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

        
        public int Update(User user)
        {
            int resultToReturn = 0;//it will save 1 or 0 depending on the result of insertion
            Exception? exception = new Exception();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {


                    connection.Open();
                    SqlCommand command = new SqlCommand("UpdateUser", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Description", user.Description);
                    command.Parameters.AddWithValue("@LinkedIn", user.LinkedIn);
                    command.Parameters.AddWithValue("@Picture", user.Picture);

                    resultToReturn = command.ExecuteNonQuery();
                    connection.Close();

                }
            }
            catch (Exception ex)
            {
                exception = ex;
                throw exception;
            }


            return resultToReturn;

        }
    }
}
