using Microsoft.AspNetCore.Mvc;
using BusinessLayer;
using EntityLayer;
using ETicaretSitesi.ExtraClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http.HttpResults;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace ETicaretSitesi.Controllers
{
    public class UserController : Controller
    {
        UserBL user_manager = new UserBL();
        ProductBL product_manager = new ProductBL();
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
                HttpContext.Session.SetString("LoggedUserOnline", "true");

                // Kullanıcı giriş yaptıgında sepetim diye bir cookie ekilyoruz.
                // Cookie var mı diye kontrol ettik. Aşagıda önce cookie'yi seçtik.
                // Sonrasında equals ile seçilen deger boş mu diye baktık.

                var sepetimvarmı = HttpContext.Request.Cookies.Where(c => c.Key == "sepetim").FirstOrDefault();
                if (sepetimvarmı.Equals(new KeyValuePair<string, string>()))
                {
                    CookieOptions cookieOptions = new CookieOptions();
                    cookieOptions.Expires = DateTime.Now.AddMonths(1);
                    cookieOptions.Secure = true;
                    cookieOptions.IsEssential = true;
                    cookieOptions.Path = "/";
                    HttpContext.Response.Cookies.Append("sepetim", $"{userid}:", cookieOptions);
                }
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

        public IActionResult AddToCart(int productid = 1)
        {
            int userid = -1;
            if (HttpContext.Session.GetInt32("LoggedUserID") == null)
            {
                // ip adresiyle sepet tutulmalı
            }
            else
            {
                userid = Convert.ToInt32(HttpContext.Session.GetInt32("LoggedUserID"));
            }

            var sepetimvarmı = HttpContext.Request.Cookies.Where(c => c.Key == "sepetim").FirstOrDefault();
            if (sepetimvarmı.Equals(new KeyValuePair<string, string>()))
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddMonths(1);
                cookieOptions.Secure = true;
                cookieOptions.IsEssential = true;
                cookieOptions.Path = "/";
                // Giriş yapmadan sepete bir şey eklemeye çalışırsa burası çalışacak.
                HttpContext.Response.Cookies.Append("sepetim", $"{userid}:{productid}", cookieOptions);
            }
            else
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddMonths(1);
                cookieOptions.Secure = true;
                cookieOptions.IsEssential = true;
                cookieOptions.Path = "/";
                // Önceki sepeti al
                string öncekisepet = HttpContext.Request.Cookies["sepetim"];
                string yenisepet = öncekisepet += $",{productid}";
                HttpContext.Response.Cookies.Append("sepetim", $"{yenisepet}", cookieOptions);
            }

            return RedirectToAction("MyCart");
        }

        public IActionResult MyCart()
        {
            string öncekisepet = HttpContext.Request.Cookies["sepetim"];
            string urunler = öncekisepet.Split(":")[1];
            string kullanıcı = öncekisepet.Split(":")[0];

            if (HttpContext.Session.GetInt32("LoggedUserID") == Convert.ToInt32(kullanıcı) || urunler != "")
            {
                var tekli_urunler = urunler.Split(",");
                List<Product> urunlistesi = new List<Product>();
                foreach (var urun in tekli_urunler)
                {
                    if (urun != "")
                    {
                        urunlistesi.Add(product_manager.FindProductByID(Convert.ToInt32(urun)));
                    }
                }
                return View(urunlistesi);
            }
            else
            {
                return RedirectToAction("Login");
            }


        }

        [HttpGet]
        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PasswordChange(string oldpassword, string newpassword, string newpassword2)
        {
            int userid = Convert.ToInt32(HttpContext.Session.GetInt32("LoggedUserID"));
            string hashed_old_pass = SHA256Converter.ComputeSha256Hash(oldpassword);
            string hashed_new_pass_1 = SHA256Converter.ComputeSha256Hash(newpassword);
            string hashed_new_pass_2 = SHA256Converter.ComputeSha256Hash(newpassword2);
            bool result = user_manager.UpdatePassword(userid, hashed_old_pass, hashed_new_pass_1, hashed_new_pass_2);
            if (result == true)
            {
                return View();
            }
            else
            {
                return RedirectToAction("usermainpage");
            }
        }

        [HttpGet]
        public IActionResult UserMainPage()
        {
            var products = product_manager.GetAllProducts();
            return View(products);
        }

        [HttpPost]
        public IActionResult UserMainPage(string filterUpperPrice,string filterLowerPrice,string filterText,string filterCategory, string sıralama)
        {
            int? upper_price = 0;
            if (filterUpperPrice !=null)
            {
                upper_price = Convert.ToInt32(filterUpperPrice);
            }
            else
            {
                upper_price = null;
            }

            int? lower_price = 0;
            if (filterLowerPrice != null)
            {
                lower_price = Convert.ToInt32(filterLowerPrice);
            }
            else
            {
                lower_price = null;
            }

            int? category = 0;
            if (filterCategory != null)
            {
                category = Convert.ToInt32(filterCategory);
            }
            else
            {
                category = null;
            }


            string? text = "";
            if (filterText != null)
            {
                text = filterText;
            }
            else
            {
                text = null;
            }

            string sıralama_sekli = "";
            if (sıralama== "Fiyata Göre Artan Sırala")
            {
                sıralama_sekli = "asc";
            }
            else if (sıralama == "Fiyata Göre Azalan Sırala")
            {
                sıralama_sekli = "desc";
            }
            else
            {
               
            }
            //var asc_isaretli_mi = formc.Where(nesne => nesne.Key.Contains("Asc")).FirstOrDefault();
            //if (asc_isaretli_mi.Key!=null)
            //{
            //    sıralama_sekli = "asc";
            //}

            //var desc_isaretli_mi = formc.Where(nesne => nesne.Key.Contains("Desc")).FirstOrDefault();
            //if (desc_isaretli_mi.Key != null)
            //{
            //    sıralama_sekli = "desc";
            //}

            var result = product_manager.GetProductsByPrice(sıralama_sekli, category, text, upper_price, lower_price);
            return View(result);
        }

    }
}