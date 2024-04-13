using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using shopthoitrang.Models;
namespace shopthoitrang.Areas.admin.Controllers
{
    public class DiscountCodeController : Controller
    {
        // GET: admin/DiscountCode
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
        public ActionResult Discount()
        {
            var code = db.Discount_code.ToList();
            return View(code);
        }
        public ActionResult searchDiscount(string key)
        {
            bool check = check_login();
            if (check)
            {
                int id;
                int.TryParse(key, out id);
                var don = db.Discount_code.Where(c => c.id_discount == id || c.name_code.Contains(key)||c.code.Contains(key)||c.end_date.Contains(key)||c.rank_user.Contains(key)).ToList();
                return View("Discount", don);
            }
            else
                return RedirectToAction("login", "admin");
        }
        [HttpPost]
        public ActionResult add_discount(string namecodeq,string codeq,int saleq, int stockq,string startdateq,string enddateq,string typediscountq, string rankuserq, string infoq,string an_spq)
        {
            var id_code = db.Discount_code.Select(i => i.id_discount).ToList();
            int dem = 1;
            if (id_code != null)
            {
                foreach(var a in id_code)
                {
                    if (a == dem) dem += 1;
                }
            }
            string code = codeq;

            code = code.Trim();
            if (code == null || code == "")
            {
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                StringBuilder result = new StringBuilder();

                Random random = new Random();
                for (int i = 0; i < 8; i++)
                {
                    int index = random.Next(chars.Length);
                    result.Append(chars[index]);

                }
                code = result.ToString();
            }
            var dis_code = new Discount_code
            {
                id_discount = dem,
                name_code = namecodeq,
                code = code,
                sale = saleq,
                stock = stockq,
                start_date = startdateq,
                end_date = enddateq,
                type_voucher = typediscountq,
                rank_user = rankuserq,
                info = infoq,
                photo = "gam.png",
                hide=an_spq
            };
            db.Discount_code.Add(dis_code);
            db.SaveChanges();
            var code_view = db.Discount_code.ToList();
            return View("Discount", code_view);
        }
        [HttpPost]
        public ActionResult sua_discount(int id_discount, string namecode, string code, int sale, int stock, string startdate, string enddate, string typediscount, string rankuser, string info, string anmagiam)
        {
            var id_code = db.Discount_code.Where(i => i.id_discount==id_discount).FirstOrDefault();
            if (id_code != null)
            {
                id_code.name_code = namecode;

                code = code.Trim();
                if (code == null|| code=="" )
                {
                    string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                    StringBuilder result = new StringBuilder();
                    
                    Random random = new Random();
                    for (int i = 0; i < 8; i++)
                    {
                        int index = random.Next(chars.Length);
                        result.Append(chars[index]);
                        
                    }
                    code = result.ToString();
                }
                id_code.code = code;
                id_code.sale = sale;
                id_code.stock = stock;
                id_code.start_date = startdate;
                id_code.end_date = enddate;
                id_code.type_voucher = typediscount;
                id_code.rank_user = rankuser;
                id_code.info = info;
                id_code.hide = anmagiam;
                db.SaveChanges();
                ViewBag.thanhcong = "sua thanh cong ma giam gia";
            }
            else
            {
                ViewBag.error = "ma giam gia khong ton tai";
            }
            
                
            var code_view = db.Discount_code.ToList();
            return View("Discount", code_view);
        }
        
        public ActionResult xoa_discount(int disID)
        {
            var id_code = db.Discount_code.Where(i => i.id_discount == disID).FirstOrDefault();
            if (id_code != null)
            {
                db.Discount_code.Remove(id_code);
                db.SaveChanges();
                ViewBag.thanhcong = "xoa thanh cong ma giam gia";
            }
            else
            {
                ViewBag.error = "ma giam gia khong ton tai";
            }


            var code_view = db.Discount_code.ToList();
            return View("Discount", code_view);
        }
    }
}