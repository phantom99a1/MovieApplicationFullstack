using AutoMapper;
using WebUI.Entities;
using WebUI.Models;

namespace WebUI.Mapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Movie, MovieListViewModel>();
            CreateMap<Movie, MovieDetailViewModel>();
            CreateMap<MovieListViewModel, Movie>();
            CreateMap<CreateMovieViewModel, Movie>()
                .ForMember(x => x.Actors, y => y.Ignore());

            CreateMap<Person, ActorViewModel>();
            CreateMap<Person, ActorDetailViewModel>();
            CreateMap<ActorViewModel, Person>();
        }
    }
}
