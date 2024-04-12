using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using shopthoitrang.Models;

namespace shopthoitrang.Areas.admin.Controllers
{
    public class ContactController : Controller
    {
        // GET: admin/Contact
        private database db = new database();
        private string username, userData = null;
        private bool check_login()
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            string user = Session["id_user"] as string;
            bool check = false;
            if (authCookie != null)
            {

                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                username = authTicket.Name;
                userData = authTicket.UserData;
                if (userData == "admin")
                    check = true;
            }
            else if (user != null)
            {
                username = Session["id_user"] as string;
                int id = int.Parse(user);
                var tk = db.Account.Where(c => c.id_user == id).FirstOrDefault();
                if (tk.role == "admin") check = true;
            }
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
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult chat(int id)
        {
            var listchat = db.Conversations.Where(c => c.ConversationID == id).FirstOrDefault();
            var read = db.Messages.Where(c => c.ConversationID == listchat.ConversationID&&c.UserID==listchat.UserID1).ToList();
            if (read != null)
            {
                foreach(var item in read)
                {
                    item.status = "yes";
                }
                db.SaveChanges();
            }
            return View(listchat);
            
        }
        public JsonResult post_chat(int id, string message)
        {
            bool check = check_login();
            if (check)
            {
                if (message != null || message != "")
                {
                    var chat = db.Conversations.Where(c => c.ConversationID == id).OrderBy(c => c.ConversationID).FirstOrDefault();
                    if (chat != null)
                    {
                        Messages messages = new Messages();
                        int id_chat = db.Messages.Select(c => c.id_chat).Count() + 1;
                        messages.id_chat = id_chat;
                        messages.message = message;
                        messages.ConversationID = chat.ConversationID;
                        messages.UserID = chat.UserID2;
                        messages.status = "no";
                        string date = DateTime.Now.ToString("yyyy-MM-dd");
                        messages.Timestamp = DateTime.Now;
                        db.Messages.Add(messages);
                        db.SaveChanges();
                    }
                    
                }
               
                return Json(new { result=true });
            }
            else
            {
                TempData["trangtruoc"] = Request.UrlReferrer?.ToString();
                return Json(new { success = false, redirectUrl = "/admin/Login" });
            }
            
        }
    }
}