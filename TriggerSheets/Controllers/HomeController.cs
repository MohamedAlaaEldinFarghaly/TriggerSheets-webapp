using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TriggerSheets.Models;
namespace TriggerSheets.Controllers
{
    public class HomeController : Controller
    {
        private QSEntities1 db = new QSEntities1();

        public ActionResult Index()
        {
            string user = Session["Username"].ToString();
            User_Line userdata = db.User_Line.Where(a => a.User_num == user).FirstOrDefault();
            if (userdata==null)
            {
                ViewBag.Message = "User is Not Found";
            }
             
            return View(userdata);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}