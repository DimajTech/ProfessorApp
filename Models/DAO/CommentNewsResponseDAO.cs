using Microsoft.Data.SqlClient;
using StudentApp.Models.Entity;

namespace StudentApp.Models.DAO
{
    public class CommentNewsResponseDAO
    {
        private readonly IConfiguration _configuration;
        string connectionString;

        public CommentNewsResponseDAO(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public int Insert(CommentNewsResponse comment)
        {
            int result = 0; //Saves 1 or 0 depending on the insertion result
            using (SqlConnection connection = new SqlConnection(connectionString))
                try
                {
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("InsertNewsCommentResponse", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@commentNewsId", comment.CommentNews.Id);
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

        public List<CommentNewsResponse> GetResponsesByCommentNewsId(string id)
        {
            List<CommentNewsResponse> reponses = new List<CommentNewsResponse>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("GetResponsesByCommentNewsId", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    reponses.Add(new CommentNewsResponse
                    {
                        Id = reader["Id"].ToString(),
                        Text = reader["Text"].ToString(),
                        DateTime = DateTime.Parse(reader["Date"].ToString()),
                        User = new User(null, reader["Name"].ToString(), null, null, reader["Role"].ToString()),
                    });
                }
                connection.Close();
            }
            return reponses;
        }

        //Possible Method: Bring all the comments by PieceOfNewsId
    }
}
