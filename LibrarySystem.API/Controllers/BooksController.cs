using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
       

        private readonly ILogger<BooksController> _logger;      

        public BooksController(ILogger<BooksController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            throw new NotImplementedException();
        }


    }
}
