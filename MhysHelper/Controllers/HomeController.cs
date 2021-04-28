using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MhysHelper.Models;
namespace MhysHelper.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var db = new Common.DBContext().Instance;
                int? useCount = db.Queryable<MhysUserInfo>().Sum(x => x.LoginCount);
                int? totalCount = db.Queryable<MhysUserInfo>().Count();
                ViewBag.useCount = useCount;
                ViewBag.totalCount = totalCount;

            }
            catch (Exception)
            {
            }

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