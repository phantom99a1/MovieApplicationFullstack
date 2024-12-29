namespace WebUI.Entities
{
    public class Movie
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ICollection<Person> Actors { get; set; } = [];

        public string Language { get; set; } = string.Empty;

        public DateTime ReleaseDate { get; set; }

        public string CoverImage { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }
    }
}
