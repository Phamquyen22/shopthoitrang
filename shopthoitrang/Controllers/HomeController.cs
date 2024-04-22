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
            var check = db.Account.Where(c => c.taikhoan == taikhoan && c.matkhau == matkhau&&c.acc_lock!="false").FirstOrDefault();
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

        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(string name,string phone, string address, string email, string username, string password)
        {
            User_info user = new User_info();
            Account acc = new Account();
            int id_user = 0;
            var dem =db.User_info.Select(c => c.id_user).ToList();
            foreach(var item in dem)
            {
                if (id_user == item) id_user += 1;
            }
            var check = db.Account.Where(c => c.taikhoan == username.Trim()).FirstOrDefault();
            if (check == null)
            {
                acc.taikhoan = username.Trim();
                acc.matkhau = password.Trim();
                acc.id_user = id_user;
                acc.role = "user";
                acc.acc_lock = "true";
                user.name = name;
                user.phone = phone;
                user.address = address;
                user.email = email;
                user.rank_user = "Đồng";
                user.img = "avatar-1.png";
                db.User_info.Add(user);
                db.SaveChanges();
                db.Account.Add(acc);
                db.SaveChanges();
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ViewBag.error = "User name đã tồn tại";
                return View();
            }
           
        }

        public ActionResult error()
        {
            return View();
        }
    }
}