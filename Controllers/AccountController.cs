using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Music_Club.Models;
using Music_Club.Repository;
using System.Security.Cryptography;
using System.Text;

namespace Music_Club.Controllers
{
    public class AccountController : Controller
    {
        IRepository<Users> _repository;
        IRepository<MusicClip> _clips;

        public AccountController(IRepository<Users> context, IRepository<MusicClip> clips)
        {
            _repository = context;
            _clips = clips;
        }

        public async Task<IActionResult> Users()
        {

            return View(await _repository.GetList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UsersConfirm(int id)
        {
            var us = await _repository.GetObject(id);
            if(us!= null)
            {
                us.IsСonfirm = true;
                _repository.Update(us);
               await _repository.Save();
                return RedirectToAction(nameof(Users));
            }
            return RedirectToAction(nameof(Users));
        }
        public ActionResult Login()

        {
            
            return View();
        }
        public ActionResult LogOut()

        {
            Response.Cookies.Delete("Login");
            Response.Cookies.Delete("FirstName");
            Response.Cookies.Delete("LastName");
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< IActionResult> Login(LoginModel logon)
        {

            if (ModelState.IsValid)
            {
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(30);
                if (_repository.GetList().Result.Count == 0)
                {
                    ModelState.AddModelError("", "Wrong login or password!");
                    return View(logon);
                }
                var users = _repository.GetList().Result.Where(a => a.Login == logon.Login).FirstOrDefault();
                if (users == null)
                {
                    ModelState.AddModelError("", "Wrong login or password!");
                    return View(logon);
                }
                //var user = users.First();
                string? salt = users.Salt;

                //переводим пароль в байт-массив  
                byte[] password = Encoding.Unicode.GetBytes(salt + logon.Password);

                //создаем объект для получения средств шифрования  
                var md5 = MD5.Create();

                //вычисляем хеш-представление в байтах  
                byte[] byteHash = md5.ComputeHash(password);

                StringBuilder hash = new StringBuilder(byteHash.Length);
                for (int i = 0; i < byteHash.Length; i++)
                    hash.Append(string.Format("{0:X2}", byteHash[i]));
                if (!users.IsСonfirm)
                {
                    ModelState.AddModelError("", "Ваша регистрация ещё не подтверждена, попробуйте позже!");
                    return View(logon);
                }
                if (users.Password != hash.ToString())
                {
                    ModelState.AddModelError("", "Wrong login or password!");
                    return View(logon);
                }
                HttpContext.Session.SetString("UserID", users.Id.ToString());
                HttpContext.Session.SetString("FirstName", users.FirstName);
                HttpContext.Session.SetString("LastName", users.LastName);
                HttpContext.Session.SetString("Login", users.Login);
                
                Response.Cookies.Append("UserID", users.Id.ToString(), option);
                Response.Cookies.Append("FirstName", users.FirstName, option);
                Response.Cookies.Append("LastName", users.LastName, option);
                Response.Cookies.Append("Login", users.Login, option);
                //return View("~/Views/MusicClips/Create.cshtml");
                var clip_models = await _clips.GetList();
                
                return RedirectToAction("Index", "MusicClips", clip_models);
            }
            return View(logon);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistratModel reg)
        {
            if (ModelState.IsValid)
            {
                Users user = new Users();
                user.FirstName = reg.FirstName;
                user.LastName = reg.LastName;
                user.Login = reg.Login;
                user.IsСonfirm = false;

                byte[] saltbuf = new byte[16];

                RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
                randomNumberGenerator.GetBytes(saltbuf);

                StringBuilder sb = new StringBuilder(16);
                for (int i = 0; i < 16; i++)
                    sb.Append(string.Format("{0:X2}", saltbuf[i]));
                string salt = sb.ToString();

                //переводим пароль в байт-массив  
                byte[] password = Encoding.Unicode.GetBytes(salt + reg.Password);

                //создаем объект для получения средств шифрования  
                var md5 = MD5.Create();

                //вычисляем хеш-представление в байтах  
                byte[] byteHash = md5.ComputeHash(password);

                StringBuilder hash = new StringBuilder(byteHash.Length);
                for (int i = 0; i < byteHash.Length; i++)
                    hash.Append(string.Format("{0:X2}", byteHash[i]));

                user.Password = hash.ToString();
                user.Salt = salt;
               await _repository.Create(user);
               await _repository.Save();
                return RedirectToAction("Login");
            }

            return View(reg);
        }
    }
}
