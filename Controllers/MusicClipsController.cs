using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Music_Club.Filters;
using Music_Club.Models;
using Music_Club.Notifications;
using Music_Club.Repository;

namespace Music_Club.Controllers
{
    [Culture]
    public class MusicClipsController : Controller
    {
        IRepository<MusicClip> _context;
        IRepository<Users> _context_user;
        IWebHostEnvironment _appEnvironment;
        IRepository<Genre> _context_genre;
        IHubContext<NotificationHub> _hub_context;
        public MusicClipsController(IRepository<MusicClip> context, IWebHostEnvironment appEnvironment, IRepository<Genre> context_genre, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _context_genre = context_genre;
            _hub_context = hub;
        }

        // GET: MusicClips
        public async Task<IActionResult> Index(string? searchClip, string? filterArtist, string? filterGenre, 
           SortState sortState, int page = 1)
        { 

            var model = new IndexModel();
            int sizePage = 12;

            model.sortViewModel = new SortViewModel(sortState);
            model.musicClips = await _context.GetList();
            model.filterViewModel = new FilterViewModel(filterArtist, filterGenre, searchClip);
            if (searchClip != null)
                model.musicClips = model.musicClips.Where(c => c.Title.ToLower().Contains(searchClip.ToLower())).ToList();
            if (filterArtist != null)
                model.musicClips = model.musicClips.Where(c => c.Artist.ToLower().Contains(filterArtist.ToLower())).ToList();
            if (filterGenre != null)
                model.musicClips = model.musicClips.Where(c => c.Genre.ToLower().Contains(filterGenre.ToLower())).ToList();

            model.musicClips = sortState switch
            {
                SortState.TitleDesc => model.musicClips.OrderByDescending(s => s.Title).ToList(),
                _ => model.musicClips.OrderBy(s => s.Title).ToList(),
            };


            var count = model.musicClips.Count();
            model.musicClips = model.musicClips.Skip((page - 1) * sizePage).Take(sizePage).ToList();
            model.pageViewModel = new PageViewModel(count, page, sizePage);


            return View(model);
        }
        public ActionResult ChangeCulture(string lang)
        {
            string? returnUrl = HttpContext.Session.GetString("path") ?? "/Club/Index";

            // Список культур
            List<string> cultures = new List<string>() { "ru", "en", "uk", "de", "fr" };
            if (!cultures.Contains(lang))
            {
                lang = "ru";
            }

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(10); // срок хранения куки - 10 дней
            Response.Cookies.Append("lang", lang, option); // создание куки
            return Redirect(returnUrl);
        }

        // GET: MusicClips/Create
        public async Task< IActionResult> Create()
        {
            MusicClipView model = new MusicClipView();
            model.GenreList = await _context_genre.GetList();
            return View(model);
        }

        public async Task<IActionResult> SelectedVideo(int id)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(30);
            var tmp=await _context.GetList();
            int selected_index=0;
            for (int i = 0; i < tmp.Count; i++)
            { 
                if (tmp[i].Id == id)
                    selected_index = i; 
            }
            int previous_video, next_video=0;
            if (selected_index != 0)
            {
                previous_video = tmp[selected_index - 1].Id;
            }
            else
            {
                previous_video = tmp[tmp.Count - 1].Id;
            }
            if(selected_index == tmp.Count - 1 )
            {
                next_video = tmp[0].Id;
            }
            else if(selected_index == 0 &&  tmp.Count == 1) { next_video = id; }
            else if(selected_index >= 0 && selected_index < tmp.Count - 1) { next_video = tmp[selected_index + 1].Id; }
            Response.Cookies.Append("Selected_video", id.ToString(), option);
            Response.Cookies.Append("previous_video", previous_video.ToString(), option);
            Response.Cookies.Append("next_video", next_video.ToString(), option);
            var model = new IndexModel
            {
                activeClip = _context.GetList().Result.Where(m => m.Id == id).FirstOrDefault(),
                musicClips = await CreateRecommendation.createRecomendation(await _context.GetList(), 5),
                filterViewModel = new FilterViewModel(null, null, null),
                pageViewModel = new PageViewModel(0, 0, 0),
                sortViewModel = new SortViewModel(SortState.TitleAsc)
            };
            return View("/Views/MusicClips/Index.cshtml", model);
        }
        // POST: MusicClips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(1000000000)]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ReleaseDate,Artist,Genre,Id_user")] MusicClip musicClip, IFormFile? uploadedFile)
        {
            if (uploadedFile != null)
            {
                // Путь к папке Files
                string path = "/video/" + uploadedFile.FileName; // имя файла

                // Сохраняем файл в папку images в каталоге wwwroot
                // Для получения полного пути к каталогу wwwroot
                // применяется свойство WebRootPath объекта IWebHostEnvironment
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream); // копируем файл в поток
                }




                musicClip.Path_Video = uploadedFile.FileName;
            }
            if (ModelState.IsValid)
            {
                musicClip.Genre = musicClip.Genre.Trim();
               await _context.Create(musicClip);
                await _context.Save();
                await SendMessage($"Трек {musicClip.Title} был добавлен");
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Create", "MusicClips", musicClip);
        }

        // GET: MusicClips/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var musicClip = await _context.Clips.FindAsync(id);
        //    if (musicClip == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(musicClip);
        //}

        // POST: MusicClips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ReleaseDate,Artist,Genre,Id_user")] MusicClip musicClip)
        //{
        //    if (id != musicClip.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(musicClip);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!MusicClipExists(musicClip.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(musicClip);
        //}

       
        public async Task<IActionResult> Delete(int? id)
        {
            var musicClip = await _context.GetObject(id);
            if (musicClip != null)
            {
                await _context.Delete(id);
            }

            await _context.Save();
            await SendMessage($"Трек {musicClip.Title} был удален");
            return RedirectToAction(nameof(Index));
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var musicClip = await _context.GetObject(id);
            if (musicClip != null)
            {
                await _context.Delete(id);
            }

            await _context.Save();
            return RedirectToAction(nameof(Index));
        }

        //private bool MusicClipExists(int id)
        //{
        //    return _context.Clips.Any(e => e.Id == id);
        //}

        private async Task SendMessage(string message)
        {
            // Вызов метода displayMessage на всех клиентах
            await _hub_context.Clients.All.SendAsync("displayMessage", message);
        }
    }
}
