using Microsoft.EntityFrameworkCore;
using Music_Club.Models;

namespace Music_Club.Repository
{
    
    public class GenreRepository : IRepository<Genre>
    {
        private readonly MusicClubContext _context;
        public GenreRepository(MusicClubContext context)
        {
            _context = context;
        }

        public async Task Create(Genre item)
        {
            await _context.Genres.AddAsync(item);
        }

        public async Task Delete(int? id)
        {
            Genre tmp = await _context.Genres.FindAsync(id);
            if (tmp != null)
            {
                _context.Genres.Remove(tmp);
            }
        }

        public async Task<List<Genre>> GetList()
        {
           return await _context.Genres.ToListAsync();
        }

        public async Task<Genre> GetObject(int? id)
        {
            return await _context.Genres.FindAsync(id);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Genre item)
        {
            _context.Entry(item).State = EntityState.Modified;
        }
        public bool Genrexists(int id)
        {
            return _context.Genres.Any(e => e.Id == id);
        }
        
    }
}
