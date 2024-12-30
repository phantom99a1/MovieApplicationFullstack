using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUI.Context;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MovieController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = 10)
        {
            var movieCount = _context.Movies.Count();
            var movieList = _context.Movies.Include(x => x.Actors)
                .Skip(pageIndex * pageSize).Take(pageSize).ToList();

        }
    }
}
