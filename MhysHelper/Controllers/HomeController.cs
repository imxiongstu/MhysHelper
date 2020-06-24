using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MhysHelper.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Play(string courseId, string chapterId, string uid)
        {
            ViewBag.chapterId = chapterId;
            ViewBag.courseId = courseId;
            ViewBag.uid = uid;
            return View();
        }

        public ActionResult Rank()
        {
            return View();
        }
    }
}