using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using shopthoitrang.Models;
namespace shopthoitrang.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
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
       
        public ActionResult product()
        {
            Uri currentUrl = Request.Url;
            TempData["trangtruoc"] = currentUrl.ToString();
            ViewBag.page_index = db.product.Select(c=>c.id_product).Count();
            Session.Add("controller", "Product");
            var data = db.product.OrderByDescending(c => c.id_product).Skip(0).Take(9).ToList();
            return View(data);
        }
        
        public ActionResult detail(int id_pro)
        {
            var pro = db.product.Where(p => p.id_product == id_pro).FirstOrDefault();
            return View(pro);
        }

        
        public JsonResult favourite(int id_pro)
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
                            them.favourite_pro = them.favourite_pro.Replace(id_pro.ToString()+ ",", "").Trim();
                            db.SaveChanges();
                            break;
                        }
                        else
                        {
                            var them = db.User_info.Where(c => c.id_user == id).FirstOrDefault();
                            them.favourite_pro += id_pro.ToString()+",";
                            db.SaveChanges();
                            break;
                        }
                    }
                    return Json(new { success = true });
                }
                else
                {
                    var them = db.User_info.Where(c => c.id_user == id).FirstOrDefault();
                    them.favourite_pro += "," + id_pro.ToString();
                    db.SaveChanges();
                    return Json(new { success=true });
                }

            }
            else
            {
                TempData["trangtruoc"] = Request.UrlReferrer?.ToString();
                return Json(new { success = true, redirectUrl = "/Home/Login" });
            }
           
        }
        public JsonResult getpro()
        {
            var check = check_login();
            if (check)
            {
               int id = int.Parse(username); 
               var giohang = db.Cart.Where(c => c.id_user == id).ToList();
               string tong = (giohang.Count()).ToString();
               int tongtien = 0;
               foreach (var it in giohang)
               {
                   var item = db.product.Where(c => c.id_product == it.id_product).FirstOrDefault();

                   var tien = (item.price_pro - (item.price_pro * item.discount_pro / 100)) * it.quantity_cart;

                   tongtien += tien ?? 0;

                }
                string gia = string.Format("{0:#,##0}₫", tongtien);
                return Json(new { success = true ,tongsohang = tong,giatien=gia});
                }
            return Json(new { success = false });
        }
        public ActionResult nextpage(int page = 0,int search=0)
        {
            int so = (page-1)*9;
            var data = db.product.OrderByDescending(c => c.id_product).Skip(so).Take(so + 9).ToList();
            return View(data);
        }
        public ActionResult barnding(int page, int id = 0)
        {
            int so = (page - 1) * 9;
            Uri currentUrl = Request.Url;
            TempData["trangtruoc"] = currentUrl.ToString();
            int tim = 0;
            if (id == 0)
            {
                string key = Session["timkiem"] as string;
                if (key != null)
                {
                    tim = int.Parse(key);
                    var data = db.product.Where(c => c.id_producer == tim).OrderByDescending(c => c.id_product).Skip(so).Take(so + 9).ToList();
                    ViewBag.page_index = db.product.Where(c => c.id_protype == tim).Select(c => c.id_product).Count();
                    return View(data);
                }
                else
                {
                    var data = db.product.OrderByDescending(c => c.id_product).Skip(so).Take(so + 9).ToList();
                    ViewBag.page_index = db.product.Select(c => c.id_product).Count();
                    return View(data);
                }

            }
            else
            {
                Session.Add("timkiem", id);
                var data = db.product.Where(c => c.id_producer == id).OrderByDescending(c => c.id_product).Skip(so).Take(so + 9).ToList();
                ViewBag.page_index = db.product.Where(c => c.id_producer == id).Select(c => c.id_product).Count();
                return View(data);
            }
        }
        public ActionResult category(int page,int id=0)
        {
            int so = (page - 1) * 9;
            Uri currentUrl = Request.Url;
            TempData["trangtruoc"] = currentUrl.ToString();
            int tim = 0;
            if (id == 0)
            {
                string key = Session["timkiem"] as string;
                if (key != null)
                {
                    tim = int.Parse(key);
                    var type = db.product_type.Where(c => c.id_cate == tim).Select(c => c.id_protype).FirstOrDefault();
                    var data = db.product.Where(c => c.id_protype == type).OrderByDescending(c => c.id_product).Skip(so).Take(so + 9).ToList();
                    ViewBag.page_index = db.product.Where(c => c.id_protype == type).Select(c => c.id_product).Count();
                    return View(data);
                }
                else
                {
                   
                    var data = db.product.OrderByDescending(c => c.id_product).Skip(so).Take(so + 9).ToList();
                    ViewBag.page_index = db.product.Select(c => c.id_product).Count();
                    return View(data);
                }
                
            }
            else
            {
                Session.Add("timkiem", id);
                var type = db.product_type.Where(c => c.id_cate == id).Select(c => c.id_protype).FirstOrDefault();
                var data = db.product.Where(c => c.id_protype == type).OrderByDescending(c => c.id_product).Skip(so).Take(so + 9).ToList();
                ViewBag.page_index = db.product.Where(c => c.id_protype == type).Select(c => c.id_product).Count();
                return View(data);
            }
            
        }
       
    }
}