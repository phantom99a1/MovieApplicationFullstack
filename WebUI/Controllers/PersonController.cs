using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUI.Context;
using WebUI.Entities;
using WebUI.Models;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PersonController(ApplicationDbContext context)
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
                var actorCount = _context.Person.Count();
                var actorList = _context.Person.Skip(pageIndex * pageSize).Take(pageSize)
                    .Select(x => new ActorViewModel
                    {
                        Id = x.Id,
                        Name = x.Name, 
                        DateOfBirth = x.DateOfBirth
                    })
                    .ToList();

                response.Status = true;
                response.Message = "Success";
                response.Data = new { Person = actorList, Count = actorCount };
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
        public IActionResult GetPersonById(int id)
        {
            var response = new BaseResponse();

            try
            {
                var person = _context.Person.Where(m => m.Id == id).FirstOrDefault();

                if (person == null)
                {
                    response.Status = false;
                    response.Message = "Movie is not exist!";
                    return BadRequest(response);
                }

                var movies = _context.Movies.Where(x => x.Actors.Contains(person))
                    .Select(m => m.Title).ToArray();

                var personData = new ActorDetailViewModel
                {
                    Id = person.Id,
                    Name = person.Name,
                    DateOfBirth = person.DateOfBirth,
                    Movies = movies
                };

                response.Status = true;
                response.Message = "Success";
                response.Data = personData;
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
        public IActionResult CreateNewMovie(ActorViewModel model)
        {
            var response = new BaseResponse();
            try
            {
                if (ModelState.IsValid)
                {                  
                    var newPerson = new Person()
                    {
                       Name = model.Name,
                       DateOfBirth = model.DateOfBirth,

                    };
                    _context.Person.Add(newPerson);
                    _context.SaveChanges();

                    model.Id = newPerson.Id;

                    response.Status = true;
                    response.Message = "Created Successfully!";
                    response.Data = model;
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
