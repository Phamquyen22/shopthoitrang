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
        public JsonResult voucher(string code_vchr)
        {
            var data = db.Discount_code.Where(d => d.code == code_vchr).FirstOrDefault();
            if (data != null)   
                return Json(new {code_data=data.sale, success =true});
            else
                return Json(new { success = false});
        }
        //thanh toan vnpay
        public ActionResult thanhtoan(string name, string address,string phone, string email,int tongtien, string voucher, string payment)
        {

            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Get payment input
            Order order = new Order();
            int id_order = db.Order.OrderByDescending(c=>c.id_order).Select(p=>p.id_order).FirstOrDefault()+1;

            long giatien = 0;
            int idvoucher;
            if (voucher != null)
            {
                int giamgia = db.Discount_code.Where(c => c.code == voucher).Select(c => c.sale).FirstOrDefault();
                if (giamgia <= 100)
                {
                    giatien = (tongtien) - (tongtien * giamgia / 100);
                }
                else
                {
                    giatien = tongtien- giamgia;
                }
                idvoucher = db.Discount_code.Where(p => p.code == voucher).Select(c=>c.id_discount).FirstOrDefault();
                if (idvoucher!=0)
                {
                    order.id_discount = idvoucher;
                }
                
            }
            order.id_order = id_order;
            order.id_user = id_acc();
            if (giatien < 0)
                giatien = 0 + 16000;
            else
                giatien = giatien + 16000;
            order.total = giatien;
            order.status_order = "Chờ xác nhận";
            order.date_order = DateTime.Now.ToString("yyyy-MM-dd");
            order.payment_status= "Chưa thanh toán";
            long OrderId = id_order; // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
            long Amount = giatien; // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
            
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
                db.SaveChanges();
            }
            DateTime CreatedDate = DateTime.Now;
            var cartItems = db.Cart.Where(c => c.id_user == id).ToList();
            foreach (var cartItem in cartItems)
            {
                db.Cart.Remove(cartItem);
            }
            db.SaveChanges();

            //Build URL for VNPAY
            if (payment == "VNPAY" &&Amount>=5000&&Amount<=1000000000)
            {
                VnPayLibrary vnpay = new VnPayLibrary();

                vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", (Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
                vnpay.AddRequestData("vnp_CreateDate", CreatedDate.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + OrderId);
                vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
                //DateTime expireDateTime = DateTime.Now.AddMinutes(20);
                //string vnp_ExpireDate = expireDateTime.ToString("yyyyMMddHHmmss");
                //vnpay.AddRequestData("vnp_ExpireDate", vnp_ExpireDate);
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", OrderId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
                
                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

                 return Redirect(paymentUrl);
            }
            else
            {
                return RedirectToAction("COD","Cart");
            }

        }
        public ActionResult VNpay()
        {
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; // Chuỗi bí mật
            var vnpayData = Request.QueryString;
            VnPayLibrary vnpay = new VnPayLibrary();

            foreach (string s in vnpayData)
            {
                // Lấy tất cả dữ liệu từ query string
                if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(s, vnpayData[s]);
                }
            }

            // Lấy các thông tin từ phản hồi VNPAY
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
                    // Thanh toán thành công
                    ViewBag.Message = "Giao dịch được thực hiện thành công.";
                    ViewBag.thank = "Quý Khách vui lòng chờ đợi đơn hàng gửi đến bạn.";
                    var order_done = db.Order.Where(c => c.id_order == orderId).FirstOrDefault();
                    order_done.payment_status = "Đã thanh toán";
                    db.SaveChanges();
                }
                else
                {
                    // Thanh toán không thành công
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                }
                ViewBag.TmnCode = "Mã Website (Terminal ID):" + TerminalID;
                ViewBag.TxnRef = "Payment transaction code:" + orderId.ToString();
                ViewBag.VnpayTranNo = "Transaction code at VNPAY:" + vnpayTranId.ToString();
                ViewBag.Amount = "Total Payment (VND):" + vnp_Amount.ToString();
                ViewBag.BankCode = "Bank payment:" + bankCode;
            }
            else
            {
                ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
            }
            var data = db.Order_detail.Where(p => p.id_order == orderId).ToList();
            return View(data);

        }
        public ActionResult COD()
        {
            int id = id_acc();
            int id_order = db.Order.Where(c => c.id_user == id).Select(c=>c.id_order).FirstOrDefault();
            var data = db.Order_detail.Where(p => p.id_order == id_order).ToList();
            ViewBag.Message = "Đơn hàng đã được đặt thành công.";
            ViewBag.thank = "Quý Khách vui lòng chờ đợi đơn hàng gửi đến bạn.";
            return View(data);
        }
    }
}