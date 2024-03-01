using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Music_Club.Models;
using Music_Club.Repository;
namespace Music_Club.Controllers
{
    public class GenreController : Controller
    {
        
        IRepository<Genre> _context;
        IRepository<MusicClip> _clips;
        public GenreController(IRepository<Genre> context, IRepository<MusicClip> clips)
        {
            _context = context;
            _clips = clips;
        }
        public async  Task<IActionResult> CreateGenre()
        {
            GenreView genreView = new GenreView();
            genreView.GenreList = await _context.GetList();
            return View(genreView);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreateGenre([Bind("Id,Genre_name")] Genre genre)
        {
            
            if (ModelState.IsValid)
            {
                foreach (var item in await _context.GetList()) 
                {
                    if (genre.Genre_name == item.Genre_name)
                    {
                        ModelState.AddModelError("", "Такой жанр уже существует!");
                        return View(genre);
                    }
                        
                }
                
                _context.Create(genre);
                await _context.Save();
                //return RedirectToAction(nameof(Index));
                var clip_models = await _clips.GetList();
                return View("~/Views/MusicClips/Index.cshtml", clip_models);
            }
            return RedirectToAction("CreateGenre", "Genre", genre);
            
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGenre(int id, [Bind("Id,Genre_name")] Genre genre,string original_name)
        {
            

            if (ModelState.IsValid)
            {
                try
                {
                    Genre edit_genre = GetGenre(original_name);
                    edit_genre.Genre_name = genre.Genre_name;
                    _context.Update(edit_genre);
                    await _context.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                       return NotFound();
                    
                    
                   
                }
                return RedirectToAction(nameof(CreateGenre));
            }
            return View(genre);
        }
       
        [HttpPost, ActionName("DeleteGenre")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string original_name)
        {
            var ganre_ = GetGenre(original_name);
            if (ganre_ != null)
            {
               await _context.Delete(ganre_.Id);
            }

            await _context.Save();
            return RedirectToAction(nameof(CreateGenre));
        }
        public Genre GetGenre(string name)
        {
            Genre genre = null;
            foreach (var item in _context.GetList().Result)
            {
                if(name.Contains(item.Genre_name))
                return item;
            }return null;
        }
    }

}
