using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using shopthoitrang.Models;
namespace shopthoitrang.Areas.admin.Controllers
{
    public class OrderController : Controller
    {
        // GET: admin/donhang
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
        public ActionResult donhang()
        {
            var don = db.Order.ToList();
            return View(don);
        }
        public ActionResult orderdetail(int id)
        {
            var data = db.Order_detail.Where(c => c.id_order == id).ToList();
            return View(data);
        }
        public JsonResult xacnhan(int id)
        {
            var order = db.Order.Where(c => c.id_order == id).FirstOrDefault();
            if (order != null)
            {
                order.status_order = "Chờ vận chuyển";
                db.SaveChanges();
                return Json(new { sucsses = true });
            }
            else
            {
                return Json(new { sucsses = false });
            }
        }

        public JsonResult xoadonhang(int id)
        {
            var order = db.Order.Where(c => c.id_order == id).FirstOrDefault();
            if (order != null)
            {
                var details = db.Order_detail.Where(o => o.id_order == order.id_order).ToList();
                
                foreach (var detail in details)
                {
                    db.Order_detail.Remove(detail);
                }
                db.Order.Remove(order);
                db.SaveChanges();
                return Json(new { sucsses = true });
            }
            else
            {
                return Json(new { sucsses = false });
            }
        }

        public ActionResult choxacnhan()
        {
            var don = db.Order.Where(c=>c.status_order=="Chờ xác nhận").ToList();
            return View(don);
        }

        public ActionResult delivery()
        {
            var don = db.Order.Where(c => c.status_order == "Chờ vận chuyển").ToList();
            return View(don);
        }
        public ActionResult ordercompleted()
        {
            var don = db.Order.Where(c => c.status_order == "Hoàn thành").ToList();
            return View(don);
        }


        public JsonResult thanhcong(int id)
        {
            var order = db.Order.Where(c => c.id_order == id).FirstOrDefault();
            if (order != null)
            {
                order.status_order = "Hoàn thành";
                db.SaveChanges();
                return Json(new { sucsses = true });
            }
            else
            {
                return Json(new { sucsses = false });
            }
        }

       
    }
}