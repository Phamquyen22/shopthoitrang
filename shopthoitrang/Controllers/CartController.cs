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
        private string username, userData = null;
        private bool check_login()
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            string user = Session["id_user"] as string;
            bool check = false;
            if (authCookie != null)
            {
                check = true;
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                username = authTicket.Name;
                userData = authTicket.UserData;
            }
            else if (user != null) { check = true; username = Session["id_user"] as string; }
            return check;
        }
        private int id_acc()
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            int user = -1;
            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                user = int.Parse(authTicket.Name);
                userData = authTicket.UserData;
            }
            else { user = int.Parse(Session["id_user"] as string); }
            return user;
        }
        public ActionResult Cart()
        {
            var check = check_login();
            if (check)
            {
                int id = id_acc();
                var data = db.Cart.Where(c => c.id_user == id).ToList();
                return View(data);
            }
            else
            {
                TempData["trangtruoc"] = Request.UrlReferrer?.ToString();
                return RedirectToAction("Login", "Home");
            }
            
        }
       
        [HttpPost]
        public ActionResult addtocart(int id_pro,string color,int quantity,string size)
        {
            bool check= check_login();
            if (check)
            {
                int id_user = id_acc();
                var check_cart = db.Cart.Where(c => c.id_user == id_user && c.id_product == id_pro).FirstOrDefault();
                if (check_cart != null)
                {
                    check_cart.quantity_cart += 1;
                    if (color != null)
                    {
                        check_cart.color_pro = color;
                    }
                    if (size != null)
                    {
                        check_cart.size_pro = size;
                    }
                }
                else
                {
                    var checkid = db.Cart.OrderByDescending(c => c.id_cart).FirstOrDefault();
                    int id = 0;
                    if (checkid != null)
                    {
                        id = checkid.id_cart + 1;
                    }
                    var addcart = new Cart
                    {
                        id_cart = id,
                        id_user = id_user,
                        id_product = id_pro,
                        quantity_cart = quantity,
                        color_pro = color,
                        size_pro = size

                    };
                    db.Cart.Add(addcart);
                }
                
                db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                TempData["trangtruoc"] = Request.UrlReferrer?.ToString();

                return Json(new { success = false , redirectUrl = "/Home/Login"} );
            }
        }
        public JsonResult update_cart(int id_pro,int quantity)
        {
            bool check = check_login();
            if (check)
            {
                int id_user = id_acc();
                var cart = db.Cart.Where(c => c.id_user == id_user && c.id_product == id_pro).FirstOrDefault();
                if (cart != null)
                {
                    cart.quantity_cart = quantity;
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false});
                }
            }
            else
            {
                TempData["trangtruoc"] = Request.UrlReferrer?.ToString();
                return Json(new { success = false, redirectUrl = "/Home/Login" });
            }
        }


    }
}