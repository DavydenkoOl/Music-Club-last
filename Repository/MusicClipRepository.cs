using Microsoft.EntityFrameworkCore;
using Music_Club.Models;

namespace Music_Club.Repository
{
    public class MusicClipRepository : IRepository<MusicClip>
    {
        private readonly MusicClubContext _context;
        public MusicClipRepository(MusicClubContext context) 
        {
            _context = context;
        }
        public async Task Create(MusicClip item)
        {
            await _context.Clips.AddAsync(item);
        }

        public async Task Delete(int? id)
        {
           MusicClip clip = await _context.Clips.FindAsync(id);
            if (clip != null)
            {
                _context.Clips.Remove(clip);    
            }
        }

        public async Task<List<MusicClip>> GetList()
        {
           return await _context.Clips.ToListAsync();
        }

        public async Task<MusicClip> GetObject(int? id)
        {
           return await _context.Clips.FindAsync(id);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(MusicClip item)
        {
            _context.Entry(item).State = EntityState.Modified;
        }
    }
}
