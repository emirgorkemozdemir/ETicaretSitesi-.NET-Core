using Microsoft.AspNetCore.Mvc;
using BusinessLayer;
using EntityLayer;
using ETicaretSitesi.ExtraClasses;
using Microsoft.AspNetCore.Http;

namespace ETicaretSitesi.Controllers
{
    public class UserController : Controller
    {
        UserBL user_manager = new UserBL();
        [HttpGet]
        public IActionResult register()
        { 
            return View();  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult register(User kullanıcı)
        {
            if (ModelState.IsValid)
            {
                kullanıcı.Password = SHA256Converter.ComputeSha256Hash(kullanıcı.Password);
                kullanıcı.UserRegisterDate = DateTime.Now;
                kullanıcı.UserOnlineDate = DateTime.Now;
                user_manager.AddBl(kullanıcı);
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
           
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User kullanıcı)
        {
            string hashed_pass = SHA256Converter.ComputeSha256Hash(kullanıcı.Password);
            bool result = user_manager.Login(kullanıcı.UserName, hashed_pass);
            if (result == true)
            {
                int userid = user_manager.FindUserIDByUserName(kullanıcı.UserName);
                HttpContext.Session.SetInt32("LoggedUserID", userid);
                HttpContext.Session.SetString("LoggedUserOnline","true");
                return RedirectToAction("UserMainPage");
            }
            else
            {
                ViewBag.errmsg = "Kullanıcı adı veya şifre hatalı";
                return View();
            }
        }
        public IActionResult MyProfile()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetInt32("LoggedUserID"));
            return View(user_manager.MyProfile(id));
        }
    }
}
