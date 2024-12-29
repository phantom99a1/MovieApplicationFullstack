namespace WebUI.Entities
{
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public ICollection<Movie> Movies { get; set; } = [];

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }
    }
}
