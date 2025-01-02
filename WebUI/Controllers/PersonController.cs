using AutoMapper;
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
        private readonly IMapper _mapper;
        public PersonController(ApplicationDbContext context, IMapper mapper)
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
                var actorCount = _context.Person.Count();
                var actorList = _mapper.Map<List<ActorViewModel>>(_context.Person.Skip(pageIndex * pageSize)
                    .Take(pageSize).ToList());

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

        [HttpGet]
        [Route("Search/{searchText}")]
        public IActionResult Search(string searchText)
        {
            var response = new BaseResponse();
            try
            {
                var personSearch = _context.Person.Where(x => x.Name.Contains(searchText)).Select(x => new
                {
                    x.Id,
                    x.Name
                }).ToList();

                response.Status = true;
                response.Message = "Search Successfully!";
                response.Data = personSearch;
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
        public IActionResult CreateNewPerson(ActorViewModel model)
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

        [HttpPut]
        [Route("Update")]
        public IActionResult UpdatePerson(ActorViewModel model)
        {
            var response = new BaseResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    if(model.Id <= 0)
                    {
                        response.Status = false;
                        response.Message = "Person is not exist!";
                        return BadRequest(response);
                    }
                    var personUpdate = _mapper.Map<Person>(model);
                    var personDetail = _context.Person.Where(m => m.Id == model.Id).AsNoTracking().FirstOrDefault();
                    if(personDetail == null)
                    {
                        response.Status = false;
                        response.Message = "Person is not exist!";
                        return BadRequest(response);
                    }

                    _context.Person.Update(personUpdate);
                    _context.SaveChanges();
                    response.Status = true;
                    response.Message = "Updated Successfully!";
                    response.Data = personUpdate;
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
        public IActionResult DeletePersion(int id)
        {
            var response = new BaseResponse();
            try
            {
                var person = _context.Person.Where(x => x.Id == id).FirstOrDefault();
                if (person == null)
                {
                    response.Status = false;
                    response.Message = "Person is not exist!";
                    return BadRequest(response);
                }
                _context.Person.Remove(person);
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
    }
}
