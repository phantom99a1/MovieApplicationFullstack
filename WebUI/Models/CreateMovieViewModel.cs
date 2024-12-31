using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class CreateMovieViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name of the movie is required!")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<int> Actors { get; set; } = [];

        [Required(ErrorMessage = "Language of the movie is required!")]
        public string Language { get; set; } = string.Empty;

        public DateTime ReleaseDate { get; set; }

        public string CoverImage { get; set; } = string.Empty;        
    }
}
