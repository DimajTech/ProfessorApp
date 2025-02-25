using Microsoft.Data.SqlClient;
using StudentApp.Models.Entity;

namespace StudentApp.Models.DAO
{
    public class ResponseAdvisementDAO
    {
        private readonly IConfiguration _configuration;
        string connectionString;
        public ResponseAdvisementDAO(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public int Insert(Advisement advisement)
        {
            int result = 0; //Saves 1 or 0 depending on the insertion result
            using (SqlConnection connection = new SqlConnection(connectionString))
                try
                {
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("InsertAdvisement", connection); //todo stored procedure
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        //Agregar AddValue para el procedimiento
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


        public ResponseAdvisement Get(string id)
        {
            ResponseAdvisement response = new ResponseAdvisement();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("GetResponseAdvisementById", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Id", id);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read()) //ask if an user has been found with the given id
                {
                    //Todo parámetros que devuelve el store procedure
                }
                connection.Close();
            }
            return response;
        }
    }
}
