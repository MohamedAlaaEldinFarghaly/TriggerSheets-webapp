using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TriggerSheets.Models;
using System.Data.Entity;

namespace TriggerSheets.Controllers
{
    public class LineController : Controller
    {

        private QSEntities1 db = new QSEntities1();

        // GET: /Line/
        public ActionResult Index()
        {
            
            var answers_tbl = db.Answers_tbl.Include(a => a.Triggers_tbl).Include(a => a.Triggers_tbl1).Include(a => a.Questions_tbl);
            return View(answers_tbl.ToList());
        }
	}
}