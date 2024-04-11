using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using shopthoitrang.Models;
namespace shopthoitrang.Controllers
{
    public class UserController : Controller
    {
        // GET: User
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
        public ActionResult profile()
        {
            bool check = check_login();
            if(check)
            return View();
            else
            {
                Uri currentUrl = Request.Url;
                TempData["trangtruoc"] = currentUrl.ToString();
                return RedirectToAction("Login", "Home");
            }
        }
        [HttpPost]
        public JsonResult update(int id_user, string name ,string email, string phone,string address,string birthday,string oldpass,string newpass,string confirmpass)
        {

            var data = db.User_info.Where(c => c.id_user == id_user).FirstOrDefault();
            data.name = name;
            data.email = email;
            data.phone = phone;
            data.address = address;
            data.birthday = birthday;
            db.SaveChanges();
            if (oldpass != null&& oldpass !="" || newpass != null &&newpass != "" || confirmpass != null&&confirmpass != "")
            {
                var user = db.Account.Where(c => c.id_user == id_user).FirstOrDefault();
                if (oldpass == user.matkhau)
                {
                    if (newpass == confirmpass)
                    {
                        user.matkhau = newpass;
                        db.SaveChanges();
                        return Json(new { result = true });
                    }
                    else
                    {
                        ViewBag.error = "Nhập lại mật khẩu không chính xác";
                        return Json(new { result = false });
                    }
                }
                else
                {
                    ViewBag.error = "Mật khẩu cũ không chính xác";
                    return Json(new { result = false });
                }
            }
           
            return Json(new { result = true });
        }
        [HttpPost]
        public ActionResult file (HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") +Path.GetFileName(file.FileName);
                string filePath = Path.Combine(Server.MapPath("~/public/img/user/"), fileName);
                file.SaveAs(filePath);
                int id = id_acc();
                var thayanh = db.User_info.Where(c => c.id_user == id).FirstOrDefault();
                thayanh.img = fileName;
                db.SaveChanges();
            }

            return View("profile");
        }

        public ActionResult Orderlist()
        {

            return View();
        }

        public ActionResult order_list(int order_id)
        {
            var data = db.Order_detail.Where(c => c.id_order == order_id).ToList();
            return View(data);
        }

        public ActionResult Favourite()
        {
            return View();
        }
        
        public ActionResult yeuthich(string id_pro)
        {
            bool check = check_login();
            if (check)
            {
                int id = int.Parse(username);
                var check_favourite = db.User_info.Where(c => c.id_user == id).Select(a => a.favourite_pro).FirstOrDefault();
                if (check_favourite != null)
                {
                    var check_favourite_list = check_favourite.Split(',').ToList();
                    foreach (var item in check_favourite_list)
                    {
                        if (item == id_pro.ToString())
                        {
                            var them = db.User_info.Where(c => c.id_user == id).FirstOrDefault();
                            them.favourite_pro = them.favourite_pro.Replace(id_pro.ToString() + ",", "").Trim();
                            db.SaveChanges();
                            break;
                        }
                        else
                        {
                            var them = db.User_info.Where(c => c.id_user == id).FirstOrDefault();
                            them.favourite_pro += id_pro.ToString() + ",";
                            db.SaveChanges();
                            break;
                        }
                    }
                    return RedirectToAction("Favourite", "User");
                }
                else
                {
                    var them = db.User_info.Where(c => c.id_user == id).FirstOrDefault();
                    them.favourite_pro += "," + id_pro.ToString();
                    db.SaveChanges();
                    return RedirectToAction("Favourite","User");
                }

            }
            else
            {
                TempData["trangtruoc"] = Request.UrlReferrer?.ToString();
                return RedirectToAction("Login","Home");
            }
            
        }


        public ActionResult Contact()
        {

            return View();
        }
        public JsonResult post_chat(string message)
        {
            bool check = check_login();
            if (check)
            {
                int acc = id_acc();
                
                if (message != null || message != "")
                {
                    var chat = db.Conversations.Where(c => c.UserID1 == acc).OrderBy(c => c.ConversationID).FirstOrDefault();
                    if (chat != null)
                    {
                        Messages messages = new Messages();
                        int id_chat = db.Messages.Select(c => c.id_chat).Count() + 1;
                        messages.id_chat = id_chat;
                        messages.message = message;
                        messages.ConversationID = chat.ConversationID;
                        messages.UserID = acc;
                        messages.status = "no";
                        string date = DateTime.Now.ToString("yyyy-MM-dd");
                        messages.Timestamp = DateTime.Now;
                        db.Messages.Add(messages);
                        db.SaveChanges();
                    }
                    else
                    {
                        int id = db.Conversations.Where(c => c.UserID1 == acc).Select(c => c.ConversationID).Count() + 1;
                        Conversations conversations = new Conversations();
                        conversations.ConversationID = id;
                        conversations.UserID1 = acc;
                        int id_admin = db.Account.Where(i => i.role == "admin").Select(c => c.id_user).FirstOrDefault();
                        conversations.UserID2 = id_admin;
                        db.Conversations.Add(conversations);
                        db.SaveChanges();
                        //message
                        Messages messages = new Messages();
                        int id_chat = db.Messages.Select(c => c.id_chat).Count() + 1;
                        messages.id_chat = id_chat;
                        messages.ConversationID = id;
                        messages.message = message;
                        messages.UserID = acc;
                        messages.status = "no";
                        string date = DateTime.Now.ToString("yyyy-MM-dd");
                        messages.Timestamp = DateTime.Now;
                        db.Messages.Add(messages);
                        db.SaveChanges();
                    }
                }
                return Json(new { result = true });
            }
            else
            {
                TempData["trangtruoc"] = Request.UrlReferrer?.ToString();
                return Json(new { success = false, redirectUrl = "/Home/Login" });
            }
           
        }
        public  ActionResult message()
        {
            bool check = check_login();
            if (check) {
                int acc = id_acc();
                
                var chat = db.Conversations.Where(c => c.UserID1 == acc).FirstOrDefault();
                if (chat != null)
                {
                    var read = db.Messages.Where(c => c.ConversationID == chat.ConversationID && c.UserID == chat.UserID2).ToList();
                    foreach (var item in read)
                    {
                        item.status = "ok";
                        db.SaveChanges();
                    }
                }
                return View();
            }
            else
            {
                TempData["trangtruoc"] = Request.UrlReferrer?.ToString();
                return RedirectToAction("Login", "Home");
            }
        }



    }
}