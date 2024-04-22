using System;

using System.Configuration;
using System.Linq;

using System.Web.Mvc;
using System.Web.Security;
using shopthoitrang.Models;
using shopthoitrang.Models.payment;
namespace shopthoitrang.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        database db = new database();
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
        public ActionResult Cart()
        {
            var check = check_login();
            if (check)
            {
                int id = id_acc();
                var data = db.Cart.Where(c => c.id_user == id).ToList();
                return View(data);
            }
            else
            {
                TempData["trangtruoc"] = Request.UrlReferrer?.ToString();
                return RedirectToAction("Login", "Home");
            }
            
        }
       
        [HttpPost]
        public ActionResult addtocart(int id_pro,string color,int quantity,string size)
        {
            bool check= check_login();
            if (check)
            {
                int id_user = id_acc();
                var check_cart = db.Cart.Where(c => c.id_user == id_user && c.id_product == id_pro).FirstOrDefault();
                if (check_cart != null)
                {
                    check_cart.quantity_cart += 1;
                    if (color != null)
                    {
                        check_cart.color_pro = color;
                    }
                    if (size != null)
                    {
                        check_cart.size_pro = size;
                    }
                }
                else
                {
                    var checkid = db.Cart.OrderByDescending(c => c.id_cart).FirstOrDefault();
                    int id = 0;
                    if (checkid != null)
                    {
                        id = checkid.id_cart + 1;
                    }
                    var addcart = new Cart
                    {
                        id_cart = id,
                        id_user = id_user,
                        id_product = id_pro,
                        quantity_cart = quantity,
                        color_pro = color,
                        size_pro = size

                    };
                    db.Cart.Add(addcart);
                }
                
                db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                TempData["trangtruoc"] = Request.UrlReferrer?.ToString();

                return Json(new { success = false , redirectUrl = "/Home/Login"} );
            }
        }

        [HttpPost]
        public JsonResult update_cart(int id_pro,int quantity)
        {
            bool check = check_login();
            if (check)
            {
                int id_user = id_acc();
                var cart = db.Cart.Where(c => c.id_user == id_user && c.id_product == id_pro).FirstOrDefault();
                if (cart != null)
                {
                    cart.quantity_cart = quantity;
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false});
                }
            }
            else
            {
                TempData["trangtruoc"] = Request.UrlReferrer?.ToString();
                return Json(new { success = false, redirectUrl = "/Home/Login" });
            }
        }

        public ActionResult xoa_cart (int id_pro)
        {
            bool check = check_login();
            if (check)
            {
                int id = id_acc();
                var item = db.Cart.Where(c => c.id_product == id_pro&&c.id_user==id).FirstOrDefault();
                if (item != null)
                {
                    db.Cart.Remove(item);
                    db.SaveChanges();
                }
                return RedirectToAction("Cart", "Cart");
            }
            else
            {
                TempData["trangtruoc"] = Request.UrlReferrer?.ToString();
                return RedirectToAction("Login", "Home");
            }
        }
        public JsonResult resulttotal()
        {
            bool check = check_login();
            var tientotal = "0";
            if (check)
            {
                int id = id_acc();
                var giohang = db.Cart.Where(c => c.id_user == id).ToList();
                int tongtien = 0;
                foreach (var it in giohang)
                {
                    var item = db.product.Where(c => c.id_product == it.id_product).FirstOrDefault();

                    int tien = ((item.price_pro - (item.price_pro * item.discount_pro / 100)) * it.quantity_cart)??0;
                   
                    tongtien += tien;
                    

                }
                 tientotal = string.Format("{0:#,##0} ₫", tongtien);
            }
           
            return Json(new { giatien= tientotal, success=true });
        }

        public ActionResult checkout()
        {
            bool check = check_login();
            if (check)
            {
                int id = id_acc();
                var data = db.Cart.Where(c => c.id_user == id).ToList();
                return View(data);
            }
            else
            {
                TempData["trangtruoc"] = Request.UrlReferrer?.ToString();
                return RedirectToAction("Login", "Home");
            }
        }
        public JsonResult voucher(int code_vchr)
        {
            var value = db.Voucher_user.Where(d => d.id_voucher == code_vchr).FirstOrDefault();
            if (value != null)
            {
                var data = db.Discount_code.Where(c => c.id_discount == value.id_discount).FirstOrDefault();
                return Json(new { code_data = data.sale, success = true });
            }
            else
                return Json(new { success = false });
        }
        //thanh toan vnpay
        public ActionResult thanhtoan(int id_user,string name, string address,string phone, string email, int voucher, string payment)
        {

            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];

            Order order = new Order();
            int id_order = db.Order.OrderByDescending(c=>c.id_order).Select(p=>p.id_order).FirstOrDefault()+1;

            long giatien = 0;
            int idvoucher;
            int tongtien = 0;
            foreach(var item in db.Cart.Where(c => c.id_user == id_user).ToList())
            {
                tongtien += ((item.product.price_pro - (item.product.discount_pro * item.product.price_pro) / 100)  * item.quantity_cart)??0;
            }
            if (voucher != 0)
            {

                var giamgia = db.Discount_code.Where(c => c.id_discount == c.Voucher_user.Where(p=>p.id_voucher==voucher).Select(p=>p.id_discount).FirstOrDefault()).FirstOrDefault();
                if (giamgia.sale <= 100)
                {
                    giatien = (tongtien) - (tongtien * giamgia.sale / 100);
                }
                else
                {
                    giatien = tongtien- giamgia.sale;
                }
                idvoucher = db.Discount_code.Where(c => c.id_discount == c.Voucher_user.Where(p => p.id_voucher == voucher).Select(p => p.id_discount).FirstOrDefault()).Select(c => c.id_discount).FirstOrDefault();
                if (idvoucher!=0)
                {
                    order.id_discount = idvoucher;
                }
                var vchr = db.Voucher_user.Where(p => p.id_voucher == voucher).FirstOrDefault();
                vchr.status_use = "true";
                giamgia.stock -= 1;
                if (giatien < 0)
                    giatien = 0 + 16000;
                else
                    giatien = giatien + 16000;
                order.total = giatien;
            }
            else
            {
                order.total = tongtien+16000;
                giatien = tongtien + 16000;
            }
            var user = db.User_info.Where(c => c.id_user == id_user).FirstOrDefault();
            user.name = name;
            user.phone = phone;
            user.address = address;
            user.email = email;
            order.id_order = id_order;
            order.id_user = id_acc();
            
            order.status_order = "Chờ xác nhận";
            order.date_order = DateTime.Now.ToString("yyyy-MM-dd");
            order.payment_status= "Chưa thanh toán";
            long OrderId = id_order; 
            long Amount = giatien; 
            int id = id_acc();
            var data = db.Cart.Where(c => c.id_user == id).ToList();
            
            db.Order.Add(order);
            db.SaveChanges();
            
            foreach (var item in data)
            {
                Order_detail odr_detail = new Order_detail();
                int id_detail = db.Order_detail.OrderByDescending(c=>c.id_orderdetail).Select(c => c.id_orderdetail).FirstOrDefault()+1;
                odr_detail.id_orderdetail = id_detail;
                odr_detail.id_order = id_order;
                odr_detail.id_product = item.id_product;
                odr_detail.size_pro = item.size_pro;
                odr_detail.color_pro = item.color_pro;
                odr_detail.quantity_pro = item.quantity_cart;
                db.Order_detail.Add(odr_detail);
                var pro = db.product.Where(c => c.id_product == item.id_product).FirstOrDefault();
                pro.buy_count += 1;
                pro.quantity_pro -= 1;
                db.SaveChanges();
            }
            DateTime CreatedDate = DateTime.Now;
            var cartItems = db.Cart.Where(c => c.id_user == id).ToList();
            foreach (var cartItem in cartItems)
            {
                db.Cart.Remove(cartItem);
            }
            db.SaveChanges();
            
            if (payment == "VNPAY" &&Amount>=5000&&Amount<=1000000000)
            {
                VnPayLibrary vnpay = new VnPayLibrary();
                vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", (Amount * 100).ToString()); 
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
                vnpay.AddRequestData("vnp_CreateDate", CreatedDate.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + OrderId);
                vnpay.AddRequestData("vnp_OrderType", "other");
                DateTime expireDateTime = DateTime.Now.AddMinutes(20);
                string vnp_ExpireDate = expireDateTime.ToString("yyyyMMddHHmmss");
                vnpay.AddRequestData("vnp_ExpireDate", vnp_ExpireDate);
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", OrderId.ToString());
                
                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

                 return Redirect(paymentUrl);
            }
            else
            {
                return RedirectToAction("COD","Cart",new {id= id_order });
            }

        }
        public ActionResult VNpay()
        {
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
            var vnpayData = Request.QueryString;
            VnPayLibrary vnpay = new VnPayLibrary();

            foreach (string s in vnpayData)
            {
                
                if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(s, vnpayData[s]);
                }
            }
            
            long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
            string TerminalID = Request.QueryString["vnp_TmnCode"];
            long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
            string bankCode = Request.QueryString["vnp_BankCode"];

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
            if (checkSignature)
            {
                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    var order_done = db.Order.Where(c => c.id_order == orderId).FirstOrDefault();
                    order_done.payment_status = "Đã thanh toán";
                    db.SaveChanges();
                }
                else
                {
                    
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                }
               
            }
            else
            {
                ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
            }
            var data = db.Order.Where(p => p.id_order == orderId).FirstOrDefault();
            ViewBag.payment = "VNPAY";
            return View(data);

        }
        public ActionResult COD(int id)
        {
            ViewBag.payment = "COD";
            var data = db.Order.Where(c => c.id_order==id).FirstOrDefault();
            return View(data);
        }
    }
}