using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Music_Club.Models
{
    public class MusicClubContext: DbContext
    {
        public DbSet<Users> Users { get; set; }

        public DbSet<MusicClip> Clips { get; set; }

        public DbSet<Genre> Genres { get; set; }
        public MusicClubContext(DbContextOptions<MusicClubContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
