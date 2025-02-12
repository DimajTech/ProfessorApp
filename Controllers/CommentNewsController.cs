using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StudentApp.Models.DAO;
using StudentApp.Models.Entity;

namespace StudentApp.Controllers
{
	public class CommentNewsController : Controller
	{
		private readonly ILogger<CommentNewsController> _logger;
		private readonly IConfiguration _configuration;
        CommentNewsDAO commentNewsDAO;
        CommentNewsResponseDAO commentNewsResponseDAO;
        PieceOfNewsDAO newsDAO;
        public CommentNewsController(ILogger<CommentNewsController> logger, IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;

            commentNewsDAO = new CommentNewsDAO(_configuration);
            newsDAO = new PieceOfNewsDAO(_configuration);
            commentNewsResponseDAO = new CommentNewsResponseDAO(_configuration);

        }


        public IEnumerable<CommentNews> GetCommentNewsByPieceOfNewsId(string id)
        {
            IEnumerable<CommentNews> commentNews = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7039/api/CommentNews/GetCommentsByPieceOfNewsId/" + id);
                    var responseTask = client.GetAsync(client.BaseAddress);
                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<List<CommentNews>>();
                        readTask.Wait();
                        //lee el estudiante provenientes de la API
                        commentNews = readTask.Result;

                    }
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact an administrator");

            }

            return commentNews;
        }
        public IEnumerable<CommentNewsResponse> GetCommentNewsResponsesByCommentId(string id)
        {

            IEnumerable<CommentNewsResponse> commentNewsResponses = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7039/api/CommentNews/GetResponsesByCommentNewsId/" + id);
                    var responseTask = client.GetAsync(client.BaseAddress);
                    responseTask.Wait();

                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<List<CommentNewsResponse>>();
                        readTask.Wait();
                        //lee el estudiante provenientes de la API
                        commentNewsResponses = readTask.Result;

                    }
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact an administrator");

            }

            return commentNewsResponses;
        }


        //TODO YA
        public IActionResult AddNewsComment([FromBody] CommentNews comment)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7039/api/CommentNews/AddNewsComment");

                var postTask = client.PostAsJsonAsync("AddNewsComment", new
                {
                    PieceOfNewsId = comment.PieceOfNews?.Id,
                    AuthorId = comment.User?.Id,
                    Text = comment.Text
                });
                postTask.Wait();

                var result = postTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    return Ok(new { Message = "Comment added successfully" });
                }
                else
                {
                    var errorMessage = result.Content.ReadAsStringAsync().Result; 
                    return StatusCode((int)result.StatusCode, new { Message = "Failed to add comment", Error = errorMessage });
                }
            }
        }


        public IActionResult AddNewsCommentResponse([FromBody] CommentNewsResponse commentResponse)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7039/api/CommentNews/AddNewsCommentResponse");

                var postTask = client.PostAsJsonAsync("AddNewsCommentResponse", new
                {
                    CommentNewsId = commentResponse.CommentNews?.Id,
                    AuthorId = commentResponse.User?.Id,
                    Text = commentResponse.Text
                });
                postTask.Wait();

                var result = postTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    return Ok(new { Message = "Response added successfully" });
                }
                else
                {
                    var errorMessage = result.Content.ReadAsStringAsync().Result;
                    return StatusCode((int)result.StatusCode, new { Message = "Failed to add response", Error = errorMessage });
                }
            }
        }



    }
}
