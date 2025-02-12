using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StudentApp.Models.Entity;

namespace StudentApp.Models.DAO
{
	public class PieceOfNewsDAO
	{
		private readonly IConfiguration _configuration;
		string connectionString;

		public PieceOfNewsDAO(IConfiguration configuration)
		{
			_configuration = configuration;
			connectionString = _configuration.GetConnectionString("DefaultConnection");
		}

        public int Insert(PieceOfNews news)
        {
            int result = 0; //Saves 1 or 0 depending on the insertion result

            if (news.User.Role =="President")
			{
                using (SqlConnection connection = new SqlConnection(connectionString))
                    try
                    {
                        {
                            connection.Open();


                            SqlCommand command = new SqlCommand("InsertNews", connection);
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Title", news.Title);
                            //command.Parameters.AddWithValue("@File", null);

                            string base64String = news.Picture;
                            base64String = Regex.Replace(base64String, "^data:.+;base64,", "");

                            // Convertir la cadena base64 a un arreglo de bytes
                            byte[] imageBytes = Convert.FromBase64String(base64String);


                            command.Parameters.AddWithValue("@Picture", imageBytes);
                            command.Parameters.AddWithValue("@AuthorID", news.User.Id);
                            command.Parameters.AddWithValue("@Description", news.Description);



                            result = command.ExecuteNonQuery();
                            connection.Close();

                        }
                    }
                    catch (SqlException e)
                    {
                        throw;
                    }
            }
            

            return result;

        }
        public List<PieceOfNews> Get()
		{
			List<PieceOfNews> news = new List<PieceOfNews>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				SqlCommand command = new SqlCommand("GetAllNews", connection);
				command.CommandType = System.Data.CommandType.StoredProcedure;
				SqlDataReader reader = command.ExecuteReader();

				while (reader.Read())
				{
                    byte[] pictureBytes = reader["Picture"] as byte[]; // Obtener los bytes de la imagen

                    string pictureBase64 = pictureBytes != null ? Convert.ToBase64String(pictureBytes) : null; // Convertir a base64

                    news.Add(new PieceOfNews
                    {
                        Id = reader["Id"].ToString(),
                        Title = reader["Title"].ToString(),
                        Description = reader["Description"].ToString(),
                        Picture = "data:image/jpeg;base64,"+pictureBase64, // Asignar la cadena base64
                        Date = DateOnly.FromDateTime((DateTime)reader["Date"]),
                        User = new User(reader["AuthorID"].ToString(), reader["AuthorName"].ToString(), null, null, reader["AuthorRole"].ToString()),
                    });
                }
				connection.Close();
			}
			return news;
		}
		public PieceOfNews Get(string id)
		{
			PieceOfNews news = new PieceOfNews();

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				SqlCommand command = new SqlCommand("GetNewsById", connection);

				command.CommandType = System.Data.CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@Id", id);
				SqlDataReader reader = command.ExecuteReader();

				if (reader.Read()) 
				{
                    byte[] pictureBytes = reader["Picture"] as byte[]; 

                    string pictureBase64 = pictureBytes != null ? Convert.ToBase64String(pictureBytes) : null; 

                    news.Title = reader["Title"].ToString();
					news.Description = reader["Description"].ToString();
                    news.Picture = "data:image/jpeg;base64," + pictureBase64; 
					news.Date = DateOnly.FromDateTime((DateTime)reader["Date"]);
					news.User = new User(reader["AuthorID"].ToString(), reader["AuthorName"].ToString(), null, null, reader["AuthorRole"].ToString());
				}
				connection.Close();
			}
			return news;
		}

	}
}
