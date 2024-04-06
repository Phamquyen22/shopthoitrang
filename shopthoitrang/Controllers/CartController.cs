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
            return View();
        }


        public ActionResult checkout()
        {

            return View();
        }

        //thanh toan vnpay
        public void thanhtoan()
        {

            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Get payment input

            long OrderId = DateTime.Now.Ticks; // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
            long Amount = 10000; // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
            string Status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending" khởi tạo giao dịch chưa có IPN
            DateTime CreatedDate = DateTime.Now;
            //Save order to db

            //Build URL for VNPAY
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

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", OrderId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày


            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            Response.Redirect(paymentUrl);
        }
        public ActionResult order()
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
                    ViewBag.Message = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                }
                else
                {
                    // Thanh toán không thành công
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                }
                ViewBag.TmnCode = "Mã Website (Terminal ID):" + TerminalID;
                ViewBag.TxnRef = "Mã giao dịch thanh toán:" + orderId.ToString();
                ViewBag.VnpayTranNo = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                ViewBag.Amount = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                ViewBag.BankCode = "Ngân hàng thanh toán:" + bankCode;
            }
            else
            {
                ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
            }

            return View();

        }

    }
}