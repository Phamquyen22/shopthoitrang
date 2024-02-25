using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using shopthoitrang.Models;
namespace shopthoitrang.Areas.admin.Controllers
{
    public class OrderController : Controller
    {
        // GET: admin/donhang
        database db = new database();
        public ActionResult donhang()
        {
            var don = db.Order.ToList();
            return View(don);
        }
    }
}