using MhysHelper.AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MhysHelper.AspNetCore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Play(string courseId, string chapterId, string uid, string mode)
        {
            ViewBag.chapterId = chapterId;
            ViewBag.courseId = courseId;
            ViewBag.uid = uid;
            ViewBag.mode = mode;
            return View();
        }
    }
}
