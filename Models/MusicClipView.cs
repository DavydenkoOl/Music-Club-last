using System.ComponentModel.DataAnnotations;

namespace Music_Club.Models
{
    public class MusicClipView
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [StringLength(500, MinimumLength = 20, ErrorMessage = "Длина строки должна быть от 20 до 500 символов")]
        public string? Description { get; set; }
        [Required]
        public DateTime? ReleaseDate { get; set; }
        [Required]
        public string? Artist { get; set; }
        [Required]
        public string? Genre { get; set; }

        public string? Path_Video { get; set; }

        public int? Id_user { get; set; }
        public IEnumerable<Genre> GenreList { get; set; }
    }
}
