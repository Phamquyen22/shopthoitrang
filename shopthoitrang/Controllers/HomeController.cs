﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using shopthoitrang.Models;
namespace shopthoitrang.Controllers
{
    public class HomeController : Controller
    {
        private database db = new database();
        public ActionResult Index()
        {
            
            return View();
        }

       
    }
}