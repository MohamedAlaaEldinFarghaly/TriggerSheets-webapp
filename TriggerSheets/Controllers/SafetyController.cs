using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TriggerSheets.Models;

namespace TriggerSheets.Controllers
{
    public class SafetyController : Controller
    {
        private QSEntities1 db = new QSEntities1();

        // GET: /Safety/
        public ActionResult Index()
        {
            return View(db.Questions_tbl.SqlQuery("Select * from Questions_tbl where Available=1 and TableType='Safety' ").ToList());
        }

        // GET: /Safety/Details/5
        public ActionResult Details()
        {
            /*if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quality_QS_tbl quality_qs_tbl = db.Quality_QS_tbl.Find(id);
            if (quality_qs_tbl == null)
            {
                return HttpNotFound();
            }
            return View(quality_qs_tbl);*/
            return View(db.Questions_tbl.SqlQuery("Select * from Questions_tbl where TableType='Safety'").ToList());
        }

        // GET: /Safety/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Safety/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Q_ID,question,AnsType,Available,TableType")] List<Questions_tbl> safety_qs_tbl)
        {
            if (ModelState.IsValid)
            {
                foreach (var Safety_tbl in safety_qs_tbl)
                {
                    Safety_tbl.TableType = "Safety";
                    db.Questions_tbl.Add(Safety_tbl);
                    db.SaveChanges();

                }

                return RedirectToAction("Index");
            }

            return View(safety_qs_tbl);
        }

        // GET: /Safety/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Questions_tbl safety_qs_tbl = db.Questions_tbl.Find(id);
            if (safety_qs_tbl == null)
            {
                return HttpNotFound();
            }
            return View(safety_qs_tbl);
        }

        // POST: /Safety/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Q_ID,Question,AnsType,Available,TableType")] Questions_tbl safety_qs_tbl)
        {
            if (ModelState.IsValid)
            {

                db.Entry(safety_qs_tbl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(safety_qs_tbl);
        }

        // GET: /Safety/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Questions_tbl safety_qs_tbl = db.Questions_tbl.Find(id);
            if (safety_qs_tbl == null)
            {
                return HttpNotFound();
            }
            return View(safety_qs_tbl);
        }

        // POST: /Safety/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Questions_tbl safety_qs_tbl = db.Questions_tbl.Find(id);
            db.Questions_tbl.Remove(safety_qs_tbl);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
