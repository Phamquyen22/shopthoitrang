using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using shopthoitrang.Models;
namespace shopthoitrang.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        database db = new database();
        public ActionResult Cart()
        {
            return View();
        }
        private bool check_login()
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            bool check = false;
            if (authCookie != null)
            {
                check = true;
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                string username = authTicket.Name;
                string userData = authTicket.UserData;
            }
            return check;
        }
        public void addtocart()
        {
            bool check= check_login();
            if (check)
            {
                var checkid = db.Cart.OrderByDescending(c => c.id_cart).FirstOrDefault();
                int id = 0;
                if (checkid!=null)
                {
                    id = checkid.id_cart + 1;
                }
                var addcart = new Cart
                {
                    id_cart = id,
                    
                };
            }
            else
            {
                TempData["trangtruoc"] = Request.UrlReferrer?.ToString();

                RedirectToAction("Login", "Home");
            }
        }
    }
}