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
    public class QualityController : Controller
    {
        private QSEntities1 db = new QSEntities1();

// GET: /Quality/
        public ActionResult Index()
        {
            return View(db.Questions_tbl.SqlQuery("Select * from Questions_tbl where Available=1 and TableType='Quality' ").ToList());
        }

        // GET: /Quality/Details/5
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
            return View(db.Questions_tbl.SqlQuery("Select * from Questions_tbl where TableType='Quality'").ToList());
        }

        // GET: /Quality/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Quality/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Q_ID,question,AnsType,Available,TableType")] List<Questions_tbl> quality_qs_tbl)
        {
            if (ModelState.IsValid)
            {
                foreach (var Quality_tbl in quality_qs_tbl)
                {
                    Quality_tbl.TableType = "Quality";
                    db.Questions_tbl.Add(Quality_tbl);
                    db.SaveChanges();

                }
         
                return RedirectToAction("Index");
            }

            return View(quality_qs_tbl);
        }

        // GET: /Quality/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Questions_tbl quality_qs_tbl = db.Questions_tbl.Find(id);
            if (quality_qs_tbl == null)
            {
                return HttpNotFound();
            }
            return View(quality_qs_tbl);
        }

        // POST: /Quality/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Q_ID,Question,AnsType,Available,TableType")] Questions_tbl quality_qs_tbl)
        {
            if (ModelState.IsValid)
            {
                quality_qs_tbl.TableType = "Quality";
                db.Entry(quality_qs_tbl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(quality_qs_tbl);
        }

        // GET: /Quality/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Questions_tbl quality_qs_tbl = db.Questions_tbl.Find(id);
            if (quality_qs_tbl == null)
            {
                return HttpNotFound();
            }
            return View(quality_qs_tbl);
        }

        // POST: /Quality/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Questions_tbl quality_qs_tbl = db.Questions_tbl.Find(id);
            db.Questions_tbl.Remove(quality_qs_tbl);
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
