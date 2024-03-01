using System.ComponentModel.DataAnnotations;

namespace Music_Club.Models
{
    public class MusicClip
    {
        public int Id { get; set; }
       
        public string? Title { get; set; }
       
        public string? Description { get; set; }
       
        public DateTime? ReleaseDate { get; set; }
       
        public string? Artist { get; set; }
        
        public string? Genre { get; set; }

        public string? Path_Video { get; set; }

        public int? Id_user { get; set; }
    }
}
