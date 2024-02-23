using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using shopthoitrang.Models;
using System.IO;
using System.Web.Security;

namespace shopthoitrang.Areas.admin.Controllers
{
    public class adminController : Controller
    {

        // GET: admin/admin
        private database db = new database();
        private bool check_login()
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            bool check=false;
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
            bool check = check_login();
            if (check)
                return View();
            else
            {
                return RedirectToAction("login", "admin");
            }
        }
        public ActionResult acc_management()
        {
            bool check = check_login();
            if (check)
                return View();
            else
            {
                return RedirectToAction("login", "admin");
            }
        }
        public ActionResult add_account()
        {
            bool check = check_login();
            if (check)
                return View();
            else
            {
                return RedirectToAction("login", "admin");
            }
        }

        [HttpPost]
        public ActionResult add_account(string tk,string mk,string role, string username, string phone, string address, string email, string ngaysinh ,HttpPostedFileBase file)
        {
            var check_inf = db.User_info.Select(u => u.id_user).ToList();
            int ID_user=1;
            if (check_inf != null)
            {
                foreach(var a in check_inf)
                {
                    if (a == ID_user)
                    {
                        ID_user += 1;
                    }
                }
                
            }
            string photo = "avatar-1.png";
            if(file !=null && file.ContentLength > 0){
                photo = Path.GetFileName(file.FileName);
                string file_path = Path.Combine(Server.MapPath("~/public/img/user/"), photo);
                file.SaveAs(file_path);
            }
            var info_acc = new User_info
            {
                id_user= ID_user,
                name =username,
                phone=phone,
                email=email,
                address=address,
                birthday=ngaysinh,
                img= photo,
                rank_user="Đồng"
            };
            var add_acc_tk = new Account
            {
                taikhoan=tk,
                matkhau=mk,
                id_user=ID_user,
                role=role,
                acc_lock="false",

            };
            db.User_info.Add(info_acc);
            db.SaveChanges();
            db.Account.Add(add_acc_tk);
            db.SaveChanges();

            return View();
        }


        public ActionResult login()
        {
            FormsAuthentication.SignOut();
            return View();
        }
        [HttpPost]
        public ActionResult login(string taikhoan, string matkhau)
        {
            var check = db.Account.Where(c => c.taikhoan == taikhoan && c.matkhau == matkhau).FirstOrDefault();
            if (check != null)
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
                return RedirectToAction("index", "admin");
            }
            return View();
        }
    }
}