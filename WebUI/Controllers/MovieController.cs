using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using WebUI.Context;
using WebUI.Entities;
using WebUI.Models;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MovieController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("Get")]
        public IActionResult Get(int pageIndex = 0, int pageSize = 10)
        {
            var response = new BaseResponse();

            try
            {
                var movieCount = _context.Movies.Count();
                var movieList = _mapper.Map<List<MovieListViewModel>>(_context.Movies.Include(x => x.Actors)
                    .Skip(pageIndex * pageSize).Take(pageSize).ToList());               

                response.Status = true;
                response.Message = "Success";
                response.Data = new {Movies = movieList, Count = movieCount};
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet]
        [Route("GetById")]
        public IActionResult GetMovieById(int id)
        {
            var response = new BaseResponse();

            try
            {
                var movie = _context.Movies.Include(x => x.Actors).Where(m => m.Id == id).FirstOrDefault();

                if(movie == null)
                {
                    response.Status = false;
                    response.Message = "Movie is not exist!";
                    return BadRequest(response);
                }

                var movieDetail = _mapper.Map<MovieDetailViewModel>(movie);

                response.Status = true;
                response.Message = "Success";
                response.Data = movieDetail;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult CreateNewMovie(CreateMovieViewModel model)
        {
            var response = new BaseResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    var actors = _context.Person.Where(x => model.Actors.Contains(x.Id)).ToList();
                    if(actors.Count != model.Actors.Count)
                    {
                        response.Status = false;
                        response.Message = "Invalid Actor assigned!";
                        return BadRequest(response);
                    }

                    var newMovie = _mapper.Map<Movie>(model);
                    newMovie.Actors = actors;
                    _context.Movies.Add(newMovie);
                    _context.SaveChanges();

                    var responseData = _mapper.Map<MovieDetailViewModel>(newMovie);

                    response.Status = true;
                    response.Message = "Created Successfully!";
                    response.Data = responseData;
                    return Ok(response);
                }
                else
                {
                    response.Status = false;
                    response.Message = "Validation Failed!";
                    response.Data = ModelState;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPut]
        [Route("Update")]
        public IActionResult UpdateMovie(CreateMovieViewModel model)
        {
            var response = new BaseResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    if(model.Id <= 0)
                    {
                        response.Status = false;
                        response.Message = "Invalid Movie Record!";
                        return BadRequest(response);
                    }
                    var actors = _context.Person.Where(x => model.Actors.Contains(x.Id)).ToList();
                    if (actors.Count != model.Actors.Count)
                    {
                        response.Status = false;
                        response.Message = "Invalid Actor assigned!";
                        return BadRequest(response);
                    }

                    var movieDetail = _context.Movies.Include(x => x.Actors).Where(x => x.Id == model.Id).FirstOrDefault();
                    if(movieDetail == null)
                    {
                        response.Status = false;
                        response.Message = "Invalid Movie Record!";
                        return BadRequest(response);
                    }

                    movieDetail.CoverImage = model.CoverImage;
                    movieDetail.Description = model.Description;
                    movieDetail.ReleaseDate = model.ReleaseDate;
                    movieDetail.Title = model.Title;
                    movieDetail.Language = model.Language;                    

                    //Find removed actor
                    var removedActors = movieDetail.Actors.Where(x => !model.Actors.Contains(x.Id)).ToList();
                    foreach(var actor in removedActors)
                    {
                        movieDetail.Actors.Remove(actor);
                    }

                    //Find added actors
                    var addActors = actors.Except(movieDetail.Actors).ToList();
                    foreach (var actor in addActors)
                    {
                        movieDetail.Actors.Add(actor);
                    }
                    _context.SaveChanges();

                    var responseData = new MovieDetailViewModel
                    {
                        Id = movieDetail.Id,
                        Title = movieDetail.Title,
                        Actors = movieDetail.Actors.Select(m => new ActorViewModel
                        {
                            Id = m.Id,
                            Name = m.Name,
                            DateOfBirth = m.DateOfBirth,
                        }).ToList(),
                        Language = movieDetail.Language,
                        CoverImage = movieDetail.CoverImage,
                        ReleaseDate = movieDetail.ReleaseDate,
                        Description = movieDetail.Description
                    };

                    response.Status = true;
                    response.Message = "Updated Successfully!";
                    response.Data = responseData;
                    return Ok(response);
                }
                else
                {
                    response.Status = false;
                    response.Message = "Validation Failed!";
                    response.Data = ModelState;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public IActionResult DeleteMovie(int id)
        {
            var response = new BaseResponse();
            try
            {
                var movie = _context.Movies.Where(x => x.Id == id).FirstOrDefault();
                if(movie == null)
                {
                    response.Status = false;
                    response.Message = "Movie is not exist!";
                    return BadRequest(response);
                }
                _context.Movies.Remove(movie);
                _context.SaveChanges();

                response.Status = true;
                response.Message = "Deleted Successfully!";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost]
        [Route("UploadMoviePoster")]
        public async Task<IActionResult> UploadMoviePosterAsync(IFormFile imageFile)
        {
            try
            {
                var fileName = ContentDispositionHeaderValue.Parse(imageFile.ContentDisposition).FileName.Trim('\"');
                string newPath = @"D:..\Delete";
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                string[] allowedInmageExtensions = [".jpg", ".jpeg", ".png"];
                if (!allowedInmageExtensions.Contains(Path.GetExtension(fileName)))
                {
                    return BadRequest(new BaseResponse
                    {
                        Status = false,
                        Message = "Only .jpg, .jpeg, .png type files are allowed!"
                    });
                }
                string newFileName = Guid.NewGuid() + Path.GetExtension(fileName);
                string fullFilePath = Path.Combine(newPath, newFileName);
                using var stream = new FileStream(fullFilePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                return Ok(new { ProfileImage = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/StaticFiles/{newFileName}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }
    }
}
