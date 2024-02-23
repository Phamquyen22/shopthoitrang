using shopthoitrang.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace shopthoitrang.Areas.admin.Controllers
{
    public class ProductsController : Controller
    {
        // GET: admin/sanpham
        private database db = new database();
        private string username;
        private string userData;
        private bool check_login()
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            bool check = false;
            if (authCookie != null)
            {
                check = true;
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                username = authTicket.Name;
                userData = authTicket.UserData;
            }
            return check;
        }
        private string file_upload(string url, HttpPostedFileBase file)
        {
            string photo=null;
            if (file != null && file.ContentLength > 0)
            {
                photo = Path.GetFileName(file.FileName);
                string file_path = Path.Combine(Server.MapPath(url), photo);
                file.SaveAs(file_path);
            }
            return photo;
        }
        public ActionResult Category()
        {
            bool check = check_login();
            if (check)
            {
                var cate = db.Category.ToList();
                return View(cate);
            }
            else
                return RedirectToAction("login", "admin");
        }
        [HttpPost]
        public ActionResult add_cate(string name_cateName,HttpPostedFileBase f_file)
        {
            var checkid = db.Category.Select(c => c.id_cate).ToList();
            int dem = 1;
            if (checkid != null)
            {
                foreach(var i in checkid)
                {
                    if (i == dem)
                    {
                        dem += 1;
                    }
                }
            }
            var cate = new Category {
                id_cate = dem,
                name_cate=name_cateName,
                image_cate= file_upload("~/public/img/cate/", f_file)
            };
            db.Category.Add(cate);
            db.SaveChanges();
            ViewBag.thanhcong = "them thanh cong danh muc san pham";
            var cate_v = db.Category.ToList();
            return View("Category", cate_v);
        }
        [HttpPost]
        public ActionResult sua_cate(string cateID, string cateName, HttpPostedFileBase file)
        {
            int id = int.Parse(cateID);
            var cate = db.Category.Where(c => c.id_cate == id).FirstOrDefault();
            if (cate != null)
            {
                cate.name_cate = cateName;
                if (file != null && file.ContentLength > 0)
                {
                cate.image_cate= file_upload("~/public/img/cate/", file);
                }
                db.SaveChanges();
                ViewBag.thanhcong = "sua thanh cong";
            }
            else
            {
                ViewBag.error = "khong ton tai danh muc nay";
            }

           
            var cate_v = db.Category.ToList();
            return View("Category",cate_v);
        }
        
        public ActionResult xoa_cate(string cateID)
        {
            int id = int.Parse(cateID);
            var cate = db.Category.Where(c => c.id_cate == id).FirstOrDefault();
            if (cate != null)
            {
                db.Category.Remove(cate);
                db.SaveChanges();
                ViewBag.thanhcong = "xoa thanh cong";
            }
            else
            {
                ViewBag.error = "loai san pham khong ton tai";
            }
            var cate_v = db.Category.ToList();
            return View("Category",cate_v);
        }

        public ActionResult ProductType()
        {
            bool check = check_login();
            if (check)
            {
                var type = db.product_type.ToList();
                return View(type);
            }
            else
                return RedirectToAction("login", "admin");
        }
        [HttpPost]
        public ActionResult add_type(int idcatename,string typeName, HttpPostedFileBase f_file)
        {

            var checkid = db.product_type.ToList();
            int dem = 1;
            if (checkid != null)
            {
                foreach(var id in checkid)
                {
                    if (dem == id.id_protype)
                    {
                        dem += 1;
                    }
                }
            }
            var type_add = new product_type
            {
                id_protype = dem,
                id_cate = idcatename,
                name_type = typeName,
                image_type = file_upload("~/public/img/type/", f_file),
            };
            db.product_type.Add(type_add);
            db.SaveChanges();
            ViewBag.thanhcong = "them thanh cong loai san pham";
            var type = db.product_type.ToList();
            return View("ProductType",type);
        }
        [HttpPost]
        public ActionResult sua_type(int typeID, int idcate, string typeName, HttpPostedFileBase file)
        {

            var pro = db.product_type.Where(p => p.id_protype == typeID).FirstOrDefault();
            if (pro != null)
            {
                pro.id_cate = idcate;
                pro.name_type = typeName;
                if (file.ContentLength > 0 && file != null)
                {
                    pro.image_type = file_upload("~/public/img/type/", file);
                }
                ViewBag.thanhcong = "Sua thanh cong loai san pham";
            }
            else
            {
                ViewBag.error = "Khong tim thay san pham";
            }
           
            var type = db.product_type.ToList();
            return View("ProductType", type);
        }
        public ActionResult xoa_type(int typeID)
        {
            var xoa = db.product_type.Where(t => t.id_protype == typeID).FirstOrDefault();
            if (xoa != null)
            {
                db.product_type.Remove(xoa);
                db.SaveChanges();
                ViewBag.thanhcong = "xoa thanh cong";
            }
            else
            {
                ViewBag.error = "loai san pham khong ton tai";
            }
            var type = db.product_type.ToList();
            return View("ProductType", type);
        }

        public ActionResult Producer()
        {
            bool login = check_login();
            if (login)
            {
                var producer = db.producer.ToList();
                return View(producer);
            }
            else
            {
                return RedirectToAction("login", "admin");
            }
        }
        [HttpPost]
        public ActionResult add_producer(string pdcName,string pdcPhone,string pdcEmail, string pdcAddress,HttpPostedFileBase file, string pdcInfo)
        {
            var id = db.producer.Select(p => p.id_producer).ToList();
            int dem = 1;
            if (id != null)
            {
                foreach (var i in id)
                {
                    if (dem == i) dem += 1;
                }
            }
            var nhasx = new producer
            {
                id_producer=dem,
                name=pdcName,
                phone=pdcPhone,
                email=pdcEmail,
                address=pdcAddress,
                photo=file_upload("~/public/img/type/producers/", file),
                info=pdcInfo
            };
            db.producer.Add(nhasx);
            db.SaveChanges();
            var producer = db.producer.ToList();
            return View("Producer", producer);
        }
        [HttpPost]
        public ActionResult sua_producer(int id_pdcID, string N_pdcName, string P_pdcPhone, string E_pdcEmail, string A_pdcAddress, HttpPostedFileBase f_file, string I_pdcInfo)
        {
            var tim = db.producer.Where(id => id.id_producer == id_pdcID).FirstOrDefault();
            if (tim != null)
            {
                tim.name = N_pdcName;
                tim.phone = P_pdcPhone;
                tim.email = E_pdcEmail;
                tim.address = A_pdcAddress;
                if (f_file != null && f_file.ContentLength > 0) tim.photo = file_upload("~/public/img/type/producers", f_file);
                tim.info = I_pdcInfo;
                db.SaveChanges();
                ViewBag.thanhcong = "sua thanh cong";
            }
            else
            {
                ViewBag.error = "nha san xuan khong ton tai";
            }
            
            var producer = db.producer.ToList();
            return View("Producer", producer);
        }

        public ActionResult product()
        {
            return View();
        }
    }
}