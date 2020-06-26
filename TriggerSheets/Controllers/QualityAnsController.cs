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
    public class QualityAnsController : Controller
    {
        private QSEntities1 db = new QSEntities1();

        public ActionResult Index(long T_ID)
        {

            long TrigID = T_ID;

            //DateTime date=new DateTime(2018,3,3);
            var Quality_ans_tbl = db.Answers_tbl.Include(q => q.Questions_tbl).Include(q => q.Triggers_tbl);
            var table = Quality_ans_tbl.Where(w => w.triggerID == TrigID && w.TableType == "Quality");

            var tr = db.Triggers_tbl.Find(T_ID);
            ViewBag.id = TrigID;
            ViewBag.line = tr.line;
            ViewBag.shift = tr.shift;
            ViewBag.day = tr.daydate;
            ViewBag.Title = "Quality Trigger Sheet";
            return View(table.ToList());
        }

        public ActionResult Create(long T_ID)
        {

            long TrigId = T_ID;

            var validq = db.Questions_tbl.Where(a => a.Available == true && a.TableType=="Quality");
            ViewBag.Sq = validq.Select(q => q.question).ToArray();
            ViewBag.count = validq.Count();
            ViewBag.SqID = validq.Select(i => i.Q_ID).ToArray();
            //ViewBag.Status = db.Answers_tbl.Select(s => s.Status).ToArray();

            var mymodel = new List<Answers_tbl>();
            for (var i = 0; i < @ViewBag.count; i++)
            {
                mymodel.Add(new Answers_tbl());
                mymodel[i].q_ID = ViewBag.SqID[i];
                mymodel[i].triggerID = TrigId;


                ViewBag.AnsType = validq.Select(n => n.AnsType).ToArray();
            }
            Triggers_tbl t = db.Triggers_tbl.Find(T_ID);
            ViewBag.line = t.line;
            ViewBag.shift = t.shift;
            ViewBag.day = t.daydate;
            ViewBag.id = t.TriggerID;
            return View(mymodel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "A_ID,q_ID,states,action,triggerID")] List<Answers_tbl> Quality_ans_tbls)
        {
            if (ModelState.IsValid)
            {

                foreach (var table in Quality_ans_tbls)
                {
                    table.TableType = "Quality";
                    table.done = !table.states;
                    if (table.states)
                    {

                        string entireString = table.action;
                        if (entireString == null)
                        {
                            entireString = "";
                            db.Answers_tbl.Add(table);
                            db.SaveChanges();

                        }
                        foreach (var myString in entireString.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var record = table;
                            record.action = myString;
                            db.Answers_tbl.Add(record);
                            db.SaveChanges();
                        }

                    }
                    else
                    {
                        table.action = null;
                        db.Answers_tbl.Add(table);
                        db.SaveChanges();
                    }


                }
                long id = Quality_ans_tbls[0].triggerID;
                Triggers_tbl trg =db.Triggers_tbl.Find(id);

                Summary_tbl summ;
                BeforeAfter AB = new BeforeAfter();
                List<Answers_tbl> BeforeList = AB.Before_tr(trg.line,"Quality",trg.shift, DateTime.Now);
                List<Answers_tbl> AfterList = AB.After_tr(trg.line, "Quality", trg.shift, DateTime.Now);


                if (db.Summary_tbl.Any(s => s.TrigID == id))
                {
                    summ = db.Summary_tbl.Where(s => s.TrigID == id).FirstOrDefault();
                    summ.LastEdited = DateTime.Now;
                    summ.QualityBefore = BeforeList.Any();
                    summ.QualityAfter = AfterList.Any();
                    db.Entry(summ).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index", new { T_ID = id });
                }
                else
                {
                    summ = new Summary_tbl();
                    summ.TrigID = id;
                    summ.Inserted = DateTime.Now;
                    summ.QualityBefore = BeforeList.Any();
                    summ.QualityAfter = AfterList.Any();
                    db.Summary_tbl.Add(summ);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Operator", new { TID = id });
                }
            }

            var validq = db.Questions_tbl.Where(a => a.Available == true).Select(q => q.question);
            ViewBag.Sq = validq.ToArray();
            ViewBag.count = validq.Count();

            return View(Quality_ans_tbls);
        }

        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answers_tbl Quality_ans_tbl = db.Answers_tbl.Find(id);
            if (Quality_ans_tbl == null)
            {
                return HttpNotFound();
            }
            ViewBag.SqID = new SelectList(db.Questions_tbl, "q_Id", "Question", Quality_ans_tbl.q_ID);
            ViewBag.TriggerID = new SelectList(db.Triggers_tbl, "triggerID", "shift", Quality_ans_tbl.triggerID);
            return View(Quality_ans_tbl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "A_ID,q_ID,states,action,triggerID")] Answers_tbl Quality_ans_tbl)
        {
            if (ModelState.IsValid)
            {
                Quality_ans_tbl.TableType = "Quality";
                if (!Quality_ans_tbl.states)
                {
                    Quality_ans_tbl.action = null;
                    Quality_ans_tbl.done = true;
                }
                db.Entry(Quality_ans_tbl).State = EntityState.Modified;
                db.SaveChanges();
                long id = Quality_ans_tbl.triggerID;
                Triggers_tbl t = db.Triggers_tbl.Find(id);

                return RedirectToAction("Index", new { T_ID = id });
            }
            ViewBag.SqID = new SelectList(db.Questions_tbl, "q_Id", "question", Quality_ans_tbl.q_ID);
            ViewBag.TriggerID = new SelectList(db.Triggers_tbl, "triggerID", "shift", Quality_ans_tbl.triggerID);
            return View(Quality_ans_tbl);
        }

        
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answers_tbl Quality_ans_tbl = db.Answers_tbl.Find(id);
            long triggerId = Quality_ans_tbl.triggerID;
            if (Quality_ans_tbl == null)
            {
                return HttpNotFound();
            }
            db.Answers_tbl.Remove(Quality_ans_tbl);
            db.SaveChanges();
            return RedirectToAction("Index", new { T_ID = triggerId });
        }

        public ActionResult DeleteAll(long triggerId)
        {
            db.Answers_tbl.RemoveRange(db.Answers_tbl.Where(i => i.triggerID == triggerId && i.TableType == "Quality"));
            db.SaveChanges();

            return RedirectToAction("Index", new { T_ID = triggerId });
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
