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
        private string file_upload(string url, HttpPostedFileBase file)
        {
            string photo=null;
            if (file != null && file.ContentLength > 0)
            {
                photo = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + Path.GetFileName(file.FileName);
                string file_path = Path.Combine(Server.MapPath(url), photo);
                file.SaveAs(file_path);
            }
            return photo;
        }
        private void xoa_file(string url,string filename)
        {
            if (filename != "demo.jpg" && filename != "avatar-1.png")
            {
                string file = Server.MapPath(url + filename);

                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);

                }
            }
               
           
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
            string anh = "demo.jpg";
            if (f_file != null && f_file.ContentLength > 0)
            {
                anh = file_upload("~/public/img/cate/", f_file);
            }
            var cate = new Category {
                id_cate = dem,
                name_cate=name_cateName,
                image_cate= anh,
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
                    xoa_file("~/public/img/cate/" , cate.image_cate);
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
                xoa_file("~/public/img/cate/" , cate.image_cate);
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
            string anh = "demo.jpg";
            if (f_file != null && f_file.ContentLength > 0)
            {
                anh = file_upload("~/public/img/type/", f_file);
            }
            var type_add = new product_type
            {
                id_protype = dem,
                id_cate = idcatename,
                name_type = typeName,
                image_type = anh,
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
                if (file != null)
                {
                    xoa_file("~/public/img/type/" , pro.image_type);
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
                xoa_file("~/public/img/type/" , xoa.image_type);
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
            string anh = "avatar-1.png";
            if (file != null && file.ContentLength > 0)
            {
                anh = file_upload("~/public/img/producers/", file);
            }
                var nhasx = new producer
            {
                id_producer=dem,
                name=pdcName,
                phone=pdcPhone,
                email=pdcEmail,
                address=pdcAddress,
  
                photo = anh,
                
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
                if (f_file != null && f_file.ContentLength > 0) {
                    xoa_file("~/public/img/producers/" , tim.photo);
                    tim.photo = file_upload("~/public/img/producers/", f_file);
                }
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
        public ActionResult xoa_producer(int prdc)
        {
            bool check = check_login();
            if (check)
            {
                var id = db.producer.Where(u => u.id_producer == prdc).FirstOrDefault();
                if (id != null)
                {
                    xoa_file("~/public/img/producers/", id.photo);
                    db.producer.Remove(id);
                    db.SaveChanges();
                    ViewBag.thanhcong = "xoa thanh cong nha san xuat";
                }
                else
                {
                    ViewBag.error = "nhà sản xuất không tồn tại";
                }
                var producer = db.producer.ToList();
                return View("Producer", producer);
            }
            else
            {
                return RedirectToAction("Login", "admin");
            }
        }
        

        public ActionResult product()
        {
            bool check = check_login();
            if (check)
            {
                var prod = db.product.ToList();
                return View(prod);
            }
            else
                return RedirectToAction("login", "admin");
            
        }
        [HttpPost]
        public ActionResult add_product(int nameprod,int nametype, string proName, string proSize, string proColor,int proPrice, int proDiscount, HttpPostedFileBase file, HttpPostedFileBase V_file, string proTag, string proDescription, int proQuantity,string an_sp)
        {
            var id = db.product.Select(p => p.id_product).ToList();
            int dem = 1;
            if (id != null)
            {
                foreach (var i in id)
                {
                    if (dem == i) dem += 1;
                }
            }
            string anh = "demo.jpg";
            if (file != null && file.ContentLength > 0)
            {
                anh = file_upload("~/public/img/product/", file);
            }
            var pro = new product
            {
                id_product = dem,
                id_producer = nameprod,
                id_protype = nametype,
                name_pro = proName,
                size_pro = proSize,
                color_pro = proColor,
                price_pro = proPrice,
                discount_pro = proDiscount,
                photo_pro = anh,
                video_pro = file_upload("~/public/img/product/", V_file),
                date_update = DateTime.Now.ToString("yyyy-MM-dd"),
                tag = proTag,
                info_pro = proDescription,
                quantity_pro = proQuantity,
                hide = an_sp,
            };
            db.product.Add(pro);
            db.SaveChanges();
            var prod = db.product.ToList();
            return View("product", prod);
        }
        [HttpPost]
        public ActionResult sua_product(int id_proID, int id_nameprod, int id_nametype, string N_proName, string N_proSize,string proColor,int N_proPrice,int N_proDiscount, HttpPostedFileBase file, HttpPostedFileBase V_file,string proTag, string N_proDescription,int N_proQuantity,string an_sp)
        {
            var tim = db.product.Where(id => id.id_product == id_proID).FirstOrDefault();
            if (tim != null)
            {
                tim.id_producer = id_nameprod;
                tim.id_protype = id_nametype;
                tim.name_pro = N_proName;
                tim.size_pro = N_proSize;
                tim.color_pro = proColor;
                tim.price_pro = N_proPrice;
                tim.discount_pro = N_proDiscount;
                if (file != null)
                {
                    xoa_file("~/public/img/product/" , tim.photo_pro);
                    tim.photo_pro = file_upload("~/public/img/product/", file);
                }
                if (V_file != null)
                {
                    xoa_file("~/public/img/product/video", tim.video_pro);
                    tim.photo_pro = file_upload("~/public/img/product/video", V_file);
                }
                tim.tag = proTag;
                tim.quantity_pro = N_proQuantity;
                tim.hide = an_sp;
                tim.info_pro = N_proDescription;
                db.SaveChanges();
                ViewBag.thanhcong = "sua thanh cong";
            }
            else
            {
                ViewBag.error = "nha san xuan khong ton tai";
            }

            var product = db.product.ToList();
            return View("product", product);
        }
        public ActionResult xoa_product(int proID)
        {
            var id = db.product.Where(u => u.id_product == proID).FirstOrDefault();
            if (id != null)
            {
                xoa_file("~/public/img/product/", id.photo_pro);
                db.product.Remove(id);
                db.SaveChanges();
                ViewBag.thanhcong = "xoa thanh cong nha san xuat";
            }
            else
            {
                ViewBag.error = "nhà sản xuất không tồn tại";
            }
            var prod = db.product.ToList();
            return View("product", prod);
        }

    }
}