using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using shopthoitrang.Models;
namespace shopthoitrang.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        private database db = new database();
        public ActionResult product()
        {
            Uri currentUrl = Request.Url;
            TempData["trangtruoc"] = currentUrl.ToString();
            return View();
        }
        [HttpGet]
        public ActionResult getpro()
        {
            var chao = "xin chao banj";
            return Content(chao);
        }
    }
}