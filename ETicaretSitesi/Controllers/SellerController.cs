using BusinessLayer;
using EntityLayer;
using ETicaretSitesi.ExtraClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ETicaretSitesi.Controllers
{
    public class SellerController : Controller
    {
        SellerBL seller_manager = new SellerBL();
        ProductBL product_manager = new ProductBL();
        CategoryBL category_manager = new CategoryBL();

        [HttpGet]
        public IActionResult register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult register(Seller kullanıcı)
        {
            if (ModelState.IsValid)
            {
                kullanıcı.Password = SHA256Converter.ComputeSha256Hash(kullanıcı.Password);

                seller_manager.AddBl(kullanıcı);
                return RedirectToAction("LoginSeller", "Seller");
            }
            else
            {
                return View();
            }

        }

        [HttpGet]
        public IActionResult LoginSeller()
        {
            return View();
        }
        [HttpPost]

        [ValidateAntiForgeryToken]
        public IActionResult LoginSeller(Seller seller)
        {
            string hashed_pass = SHA256Converter.ComputeSha256Hash(seller.Password);
            Seller result = seller_manager.Login(seller.TaxNo, hashed_pass);

            if (result!=null)
            {
                HttpContext.Session.SetInt32("LoggedSellerID", result.SellerId);
                HttpContext.Session.SetString("IsSellerOnline", "true");

                return RedirectToAction("SellerMainPage", "Seller");
            }
            else
            {
                return RedirectToAction("register","Seller");
            }

        }

        public List<SelectListItem> GetCategory()
        {
            var list = new List<SelectListItem>();
            var listt = category_manager.GetCategories();
            foreach (var item in listt)
            {
                SelectListItem p = new SelectListItem();
                p.Text = item.CategoryName;
                p.Value = item.CategoryId.ToString();
                list.Add(p);
            }
            return list;
        }
        [HttpGet]
        public IActionResult AddProduct()
        {
            if (HttpContext.Session.GetString("IsSellerOnline")=="true")
            {
				ViewBag.categori = GetCategory();
				return View();
			}
            else
            {
                return RedirectToAction("LoginSeller");
            }
          
        }
        [HttpPost]
        public IActionResult AddProduct(Product input)
        {
			if (HttpContext.Session.GetString("IsSellerOnline") == "true")
			{
				input.Sellerid = Convert.ToInt32(HttpContext.Session.GetInt32("LoggedSellerID"));
				ViewBag.categori = GetCategory();
				product_manager.AddProduct(input);
				return View();
			}
			else
			{
				return RedirectToAction("LoginSeller");
			}	
        }
    }
}
