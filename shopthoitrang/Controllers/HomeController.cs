using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using shopthoitrang.Models;
namespace shopthoitrang.Controllers
{
    public class HomeController : Controller
    {
        private database db = new database();
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
        public ActionResult Index()
        {
            Uri currentUrl = Request.Url;
            TempData["trangtruoc"] = currentUrl.ToString();
            return View();
        }
        public ActionResult Login()
        {
            FormsAuthentication.SignOut();
            return View();
        }
        [HttpPost]
        public ActionResult Login(string taikhoan,string matkhau,string luuthongtin)
        {
            var check = db.Account.Where(c => c.taikhoan == taikhoan && c.matkhau == matkhau).FirstOrDefault();
            if (check != null)
            {
                if (luuthongtin == "on")
                {
                    var authTicket = new FormsAuthenticationTicket(
                      1,
                      check.id_user.ToString(),
                      DateTime.Now,
                      DateTime.Now.AddDays(30),
                      false,
                      userData: check.role
                   );
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    authCookie.Expires = DateTime.Now.AddDays(30);
                    Response.Cookies.Add(authCookie);
                }
                else
                {
                    Session.Add("id_user", check.id_user);
                }
                string trang_truoc = TempData["trangtruoc"] as string;
                if( trang_truoc!=null)
                    return Redirect(trang_truoc);
                else
                    return RedirectToAction("Index","Home");
            }
            else
            {
                ViewBag.error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }
        }

    }
}