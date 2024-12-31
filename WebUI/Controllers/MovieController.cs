using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
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

        public MovieController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Get")]
        public IActionResult Get(int pageIndex = 0, int pageSize = 10)
        {
            var response = new BaseResponse();

            try
            {
                var movieCount = _context.Movies.Count();
                var movieList = _context.Movies.Include(x => x.Actors)
                    .Skip(pageIndex * pageSize).Take(pageSize)
                    .Select(x => new MovieListViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Actors = x.Actors.Select(m => new ActorViewModel
                        {
                            Id = m.Id,
                            Name = m.Name,
                            DateOfBirth = m.DateOfBirth,
                        }).ToList(),
                        Language = x.Language,
                        CoverImage = x.CoverImage,
                        ReleaseDate = x.ReleaseDate
                    })
                    .ToList();

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
                var movie = _context.Movies.Include(x => x.Actors)
                    .Where(m => m.Id == id)
                    .Select(x => new MovieDetailViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Actors = x.Actors.Select(m => new ActorViewModel
                        {
                            Id = m.Id,
                            Name = m.Name,
                            DateOfBirth = m.DateOfBirth,
                        }).ToList(),
                        Language = x.Language,
                        CoverImage = x.CoverImage,
                        ReleaseDate = x.ReleaseDate,
                        Description = x.Description
                    })
                    .FirstOrDefault();

                if(movie == null)
                {
                    response.Status = false;
                    response.Message = "Movie is not exist!";
                    return BadRequest(response);
                }

                response.Status = true;
                response.Message = "Success";
                response.Data = movie;
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

                    var newMovie = new Movie()
                    {
                        Title = model.Title,
                        ReleaseDate = model.ReleaseDate,
                        Language = model.Language,
                        CoverImage = model.CoverImage,
                        Description = model.Description,
                        Actors = actors,
                    };
                    _context.Movies.Add(newMovie);
                    _context.SaveChanges();

                    var responseData = new MovieDetailViewModel
                    {
                        Id = newMovie.Id,
                        Title = newMovie.Title,
                        Actors = newMovie.Actors.Select(m => new ActorViewModel
                        {
                            Id = m.Id,
                            Name = m.Name,
                            DateOfBirth = m.DateOfBirth,
                        }).ToList(),
                        Language = newMovie.Language,
                        CoverImage = newMovie.CoverImage,
                        ReleaseDate = newMovie.ReleaseDate,
                        Description = newMovie.Description
                    };

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
    }
}
