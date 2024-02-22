using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace shopthoitrang.Areas.admin.Controllers
{
    public class ProductsController : Controller
    {
        // GET: admin/sanpham
        public ActionResult Category()
        {
            return View();
        }
    }
}