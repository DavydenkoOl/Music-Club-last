using System.ComponentModel.DataAnnotations;

namespace Music_Club.Models
{
    public class GenreView
    {
        public int Id { get; set; }

        [Required]
        public string Genre_name { get; set; }

        public IEnumerable<Genre> GenreList { get; set; }
    }
}
