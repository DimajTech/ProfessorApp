using Microsoft.Data.SqlClient;
using StudentApp.Models.Entity;

namespace StudentApp.Models.DAO
{
    public class CommentNewsDAO
    {
        private readonly IConfiguration _configuration;
        string connectionString;
        CommentNewsResponseDAO responseDAO;

        public CommentNewsDAO(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
            responseDAO= new CommentNewsResponseDAO(_configuration);

        }
        public int Insert(CommentNews comment)
        {
            int result = 0; //Saves 1 or 0 depending on the insertion result
            using (SqlConnection connection = new SqlConnection(connectionString))
                try
                {
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("InsertNewsComment", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@PieceOfNewsId", comment.PieceOfNews.Id);
						command.Parameters.AddWithValue("@AuthorID", comment.User.Id);
						command.Parameters.AddWithValue("@Text", comment.Text);


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

		public List<CommentNews> GetCommentsByPieceOfNewsId(string id)
		{
			List<CommentNews> comments = new List<CommentNews>();

            using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				SqlCommand command = new SqlCommand("GetCommentByPieceOfNewsId", connection);
				command.CommandType = System.Data.CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@Id", id);

				SqlDataReader reader = command.ExecuteReader();

				while (reader.Read())
				{
                    comments.Add(new CommentNews
                    {
                        Id = reader["Id"].ToString(),
                        Text = reader["Text"].ToString(),
                        DateTime = DateTime.Parse(reader["Date"].ToString()),
                        User = new User(null, reader["Name"].ToString(), null, null, reader["Role"].ToString()),
                        TotalResponses = Convert.ToInt32(reader["TotalResponses"])
                }) ;
				}
				connection.Close();
			}
			return comments;
		}
		
		//Possible Method: Bring all the comments by PieceOfNewsId
	}
}
