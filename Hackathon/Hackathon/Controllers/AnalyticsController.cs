﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hackathon.Controllers
{
    public class AnalyticsController : Controller
    {
        // GET: Analytics
        public ActionResult ByProjectsView()
        {
            return View();
        }
        public ActionResult ByFundsView()
        {
            return View();
        }
    }
}