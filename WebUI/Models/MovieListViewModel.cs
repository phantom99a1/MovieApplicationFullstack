namespace WebUI.Models
{
    public class MovieListViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public List<ActorViewModel> Actors { get; set; } = [];

        public string Language { get; set; } = string.Empty;

        public DateTime ReleaseDate { get; set; }

        public string CoverImage { get; set; } = string.Empty;
    }
}
