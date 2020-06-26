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
    public class SafetyAnsController : Controller
    {
        private QSEntities1 db = new QSEntities1();
        //a func to find the trigger sheed id or create new one
       /* private long SFindTrigger(int line,string shift, DateTime day)
        {
            bool found = db.SafetyTriggers_tbl.Any(l => l.line == line && l.shift == shift && l.daydate == day);
            if (found)
            {
                return  db.SafetyTriggers_tbl.Where(l => l.line == line && l.shift == shift && l.daydate == day).FirstOrDefault().STriggerId;
                
            }
            else
            {

                SafetyTriggers_tbl record = new SafetyTriggers_tbl();
                record.line = line;
                record.shift = shift;
                record.daydate = day;
                db.SafetyTriggers_tbl.Add(record);
                db.SaveChanges();

                return  db.SafetyTriggers_tbl.Where(l => l.line == line && l.shift == shift && l.daydate == day).FirstOrDefault().STriggerId;
                
            }
        }
        */
     
        // GET: /SafetyAns/
        public ActionResult Index(long T_ID)
        {
           
            long TrigID = T_ID;
           
            //DateTime date=new DateTime(2018,3,3);
            var Safety_ans_tbl = db.Answers_tbl.Include(q => q.Questions_tbl).Include(q => q.Triggers_tbl);
            var table = Safety_ans_tbl.Where(w => w.triggerID == TrigID && w.TableType=="Safety");

            var tr = db.Triggers_tbl.Find(T_ID);
            ViewBag.id = TrigID;
            ViewBag.line = tr.line;
            ViewBag.shift = tr.shift;
            ViewBag.day = tr.daydate;
            ViewBag.Title = "Safety Trigger Sheet";
            return View(table.ToList());
        }

        // GET: /SafetyAns/Details/5
       /* public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Safety_Ans_tbl safety_ans_tbl = db.Safety_Ans_tbl.Find(id);
            if (safety_ans_tbl == null)
            {
                return HttpNotFound();
            }
            return View(safety_ans_tbl);
        }
        */
        // GET: /SafetyAns/Create
        public ActionResult Create(long T_ID)
        {
           
            long TrigId =  T_ID;

            var validq = db.Questions_tbl.Where(a => a.Available == true && a.TableType == "Safety");
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
            //ViewBag.Shift = shift;//db.SafetyTriggers_tbl.Where(t => t.STriggerId == TrigId).Select(s => s.shift).FirstOrDefault();
            return View(mymodel);
        }

        // POST: /SafetyAns/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="A_ID,q_ID,states,action,triggerID")] List<Answers_tbl> safety_ans_tbls)
        {
            if (ModelState.IsValid)
            {
                
                foreach(var table in safety_ans_tbls)
                {
                    table.TableType = "Safety";
                    table.done = !table.states;
                    if (table.states)
                    {

                    string entireString = table.action;
                    if (entireString ==null)
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
                long id = safety_ans_tbls[0].triggerID;
                Triggers_tbl trg = db.Triggers_tbl.Find(id);
                Summary_tbl summ;
                BeforeAfter AB = new BeforeAfter();
                List<Answers_tbl> BeforeList = AB.Before_tr(trg.line, "Safety", trg.shift, DateTime.Now);
                List<Answers_tbl> AfterList = AB.After_tr(trg.line, "Safety", trg.shift, DateTime.Now);

                if (db.Summary_tbl.Any(s => s.TrigID == id))
                {
                    summ = db.Summary_tbl.Where(s => s.TrigID == id).FirstOrDefault();
                    summ.LastEdited = DateTime.Now;
                    summ.SafetyBefore = BeforeList.Any();
                    summ.SafetyAfter = AfterList.Any();
                    db.Entry(summ).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { T_ID = id });
                }
                else
                {

                    summ = new Summary_tbl();
                    summ.TrigID = id;
                    summ.Inserted = DateTime.Now;
                    summ.SafetyBefore = BeforeList.Any();
                    summ.SafetyAfter = AfterList.Any();
                    db.Summary_tbl.Add(summ);
                    db.SaveChanges();
                    return RedirectToAction("Create", "QualityAns", new { T_ID = id });

                }
                
        
            }

            var validq = db.Questions_tbl.Where(a => a.Available == true).Select(q => q.question);
            ViewBag.Sq = validq.ToArray();
            ViewBag.count = validq.Count(); 
 
            return View(safety_ans_tbls);
        }

        // GET: /SafetyAns/Edit/
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answers_tbl safety_ans_tbl = db.Answers_tbl.Find(id);
            if (safety_ans_tbl == null)
            {
                return HttpNotFound();
            }
            ViewBag.SqID = new SelectList(db.Questions_tbl, "q_Id", "question", safety_ans_tbl.q_ID);
            ViewBag.TriggerID = new SelectList(db.Triggers_tbl, "triggerID", "shift", safety_ans_tbl.triggerID);
            return View(safety_ans_tbl);
        }

        // POST: /SafetyAns/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="A_ID,q_ID,states,action,triggerID")] Answers_tbl safety_ans_tbl)
        {
            if (ModelState.IsValid)
            {
                safety_ans_tbl.TableType = "Safety";
                if(!safety_ans_tbl.states)
                {
                    safety_ans_tbl.action = null;
                    safety_ans_tbl.done = true;
                }
                db.Entry(safety_ans_tbl).State = EntityState.Modified;
                db.SaveChanges();
                long id = safety_ans_tbl.triggerID;
                Triggers_tbl t = db.Triggers_tbl.Find(id);

                return RedirectToAction("Index", new { T_ID=id });
            }
            ViewBag.SqID = new SelectList(db.Questions_tbl, "q_Id", "Question", safety_ans_tbl.q_ID);
            ViewBag.TriggerID = new SelectList(db.Triggers_tbl, "triggerID", "shift", safety_ans_tbl.triggerID);
            return View(safety_ans_tbl);
        }

        // GET: /SafetyAns/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answers_tbl safety_ans_tbl = db.Answers_tbl.Find(id);
            long triggerId = safety_ans_tbl.triggerID;
            if (safety_ans_tbl == null)
            {
                return HttpNotFound();
            }
            db.Answers_tbl.Remove(safety_ans_tbl);
            db.SaveChanges();
            return RedirectToAction("Index", new { T_ID = triggerId });
        }

        //// POST: /SafetyAns/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(long id)
        //{
        //    Answers_tbl safety_ans_tbl = db.Answers_tbl.Find(id);
        //    db.Answers_tbl.Remove(safety_ans_tbl);
        //    db.SaveChanges();

        //    long triggerId = safety_ans_tbl.triggerID;
        //    //Triggers_tbl t = db.Triggers_tbl.Find(triggerId);
        //    return RedirectToAction("Index", new { TID=triggerId });
        //}


        public ActionResult DeleteAll(long triggerId)
        {
            db.Answers_tbl.RemoveRange(db.Answers_tbl.Where(i => i.triggerID == triggerId && i.TableType=="Safety"));
            db.SaveChanges();
            
            //SafetyTriggers_tbl t = db.SafetyTriggers_tbl.Find(triggerId);
            return RedirectToAction("Index", new { T_ID=triggerId });
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
