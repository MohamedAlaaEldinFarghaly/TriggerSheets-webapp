using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using TriggerSheets.Models;
using System.Runtime.Serialization.Diagnostics;
using System.ComponentModel;

namespace TriggerSheets.Controllers
{
    public class OperatorController : Controller
    {
        QSEntities1 db = new QSEntities1(); // context database connection


        //
        // GET: /Operator/
        public ActionResult Index( long TID)
        {
            Triggers_tbl tr = db.Triggers_tbl.Find(TID);
            int LINE =tr.line;
            var answers = db.Answers_tbl.Include(q => q.Triggers_tbl);
            var actions = answers.Where(a => a.Triggers_tbl.line == LINE && a.done == false);
            ViewBag.ids = TID;
            ViewBag.idq = TID;

            
            
            string shift = tr.shift;
            DateTime date = tr.daydate;
      
            ViewBag.line = LINE;
            ViewBag.shift = shift;
            ViewBag.day = date;

            return View(actions.ToList());

            
        }

        // POST: /Operator/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "A_ID,q_ID,states,action,done,triggerID,TableType")] List<Answers_tbl> Actions_tbl,long TID)
    {
        if (ModelState.IsValid)
        {
            foreach (var table in Actions_tbl)
            {
                if (table.done)
                {
                    table.DoneDate = DateTime.Now;
                    table.DoneTrigger = TID;
                }
                
                db.Entry(table).State = EntityState.Modified;
                db.SaveChanges();
                
            }

            long id = TID;
            
            Triggers_tbl trg = db.Triggers_tbl.Find(id);
            Summary_tbl summ;
            BeforeAfter AB = new BeforeAfter();
            List<Answers_tbl> BeforeList = AB.Before_tr(trg.line, Actions_tbl[0].TableType, trg.shift, DateTime.Now);
            List<Answers_tbl> AfterList = AB.After_tr(trg.line, Actions_tbl[0].TableType, trg.shift, DateTime.Now);

            if (db.Summary_tbl.Any(s => s.TrigID == id))
            {
                summ = db.Summary_tbl.Where(s => s.TrigID == id).FirstOrDefault();
                summ.LastEdited = DateTime.Now;
                summ.SafetyBefore = BeforeList.Any();
                summ.SafetyAfter = AfterList.Any();
                db.Entry(summ).State = EntityState.Modified;
                db.SaveChanges();

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

            }

            return RedirectToAction("Index", new { TID = id });
        }
            ViewBag.message = "Not Saved";
            return View(Actions_tbl);
}
        
        
        // GET: /Operator/Create
        public ActionResult Create()
        {
            string username =Session["Username"].ToString();
            User_Line user = db.User_Line.Where(u => u.User_num == username ).FirstOrDefault();
            Triggers_tbl tr = new Triggers_tbl();
            tr.line = Convert.ToInt16( user.Line);
            tr.usernum = user.ID;
            return View(tr);
        }

        // POST: /Operator/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TriggerID,line,shift,daydate,usernum")] Triggers_tbl trigger)
        {
            long T_ID;
            if (ModelState.IsValid)
            {
                if (db.Triggers_tbl.Any(t => t.line == trigger.line && t.shift == trigger.shift && DbFunctions.TruncateTime(t.daydate) == DbFunctions.TruncateTime(trigger.daydate)))
                {
                    Triggers_tbl tabl = db.Triggers_tbl.Where(t => t.line == trigger.line && t.shift == trigger.shift && DbFunctions.TruncateTime(t.daydate) == DbFunctions.TruncateTime(trigger.daydate)).FirstOrDefault();
                    T_ID = tabl.TriggerID;

                }
                else
                {

                    db.Triggers_tbl.Add(trigger);
                    db.SaveChanges();
                    T_ID = trigger.TriggerID;
                    
                }

                if (db.Summary_tbl.Any(s => s.TrigID == T_ID))
                {
                   return RedirectToAction("Index", new { TID =T_ID });
                }
                else{
                    return RedirectToAction("Create", "SafetyAns", new { T_ID = T_ID });
                    
                }
                
                
            }
            return View(trigger);

        }
        
        //GET: /Operator/AddAction
        public ActionResult AddAction(bool RL, long TID)
        {
            long IID = TID;
            var model = new Answers_tbl();
            if (RL)
            {
                 model.TableType = "Safety";
                 ViewBag.q_ID = new SelectList(db.Questions_tbl.Where(x =>x.TableType=="Safety"), "Q_Id", "question");
            }
            else
            {
                 model.TableType = "Quality";
                 ViewBag.q_ID = new SelectList(db.Questions_tbl.Where(x => x.TableType == "Quality"), "Q_Id", "question");

            }

            model.states = true;
            model.triggerID = IID;
            model.done = false;
            
            ViewBag.ids = TID;
            ViewBag.idq = TID;

            ViewBag.q_ID = new SelectList(db.Questions_tbl, "Q_Id", "question");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAction([Bind(Include="q_ID,states,action,done,triggerID,TableType")] Answers_tbl answer)
        {

            if (ModelState.IsValid)
            {
                db.Answers_tbl.Add(answer);
                db.SaveChanges();

                long id =answer.triggerID;
                Triggers_tbl trg = db.Triggers_tbl.Find(id);
                Summary_tbl summ;
                BeforeAfter AB = new BeforeAfter();
                List<Answers_tbl> BeforeList = AB.Before_tr(trg.line, answer.TableType, trg.shift, DateTime.Now);
                List<Answers_tbl> AfterList = AB.After_tr(trg.line, answer.TableType, trg.shift, DateTime.Now);

                if (db.Summary_tbl.Any(s => s.TrigID == id))
                {
                    summ = db.Summary_tbl.Where(s => s.TrigID == id).FirstOrDefault();
                    summ.LastEdited = DateTime.Now;
                    summ.SafetyBefore = BeforeList.Any();
                    summ.SafetyAfter = AfterList.Any();
                    db.Entry(summ).State = EntityState.Modified;
                    db.SaveChanges();
                
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

                }
                
                return RedirectToAction("Index", new { TID = id  });
            }
            return View();
        }

        public ActionResult EditAction(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answers_tbl ans_tbl = db.Answers_tbl.Find(id);
            if (ans_tbl == null)
            {
                return HttpNotFound();
            }
            ViewBag.SqID = new SelectList(db.Questions_tbl, "q_Id", "Question", ans_tbl.q_ID);
            ViewBag.TriggerID = new SelectList(db.Triggers_tbl, "triggerID", "shift", ans_tbl.triggerID);
            return View(ans_tbl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAction([Bind(Include = "A_ID,q_ID,states,action,triggerID,TableType,done,DoneDate")] Answers_tbl ans_tbl)
        {
            if (ModelState.IsValid)
            {
                if (ans_tbl.done)
                {
                    ans_tbl.DoneDate = DateTime.Now;
                }
                if (!ans_tbl.states)
                {
                    ans_tbl.action = null;
                    ans_tbl.done = true;
                }
              
                db.Entry(ans_tbl).State = EntityState.Modified;
                db.SaveChanges();
                long id = ans_tbl.triggerID;
                
                Triggers_tbl trg = db.Triggers_tbl.Find(id);
                Summary_tbl summ;
                BeforeAfter AB = new BeforeAfter();
                List<Answers_tbl> BeforeList = AB.Before_tr(trg.line, ans_tbl.TableType, trg.shift, DateTime.Now);
                List<Answers_tbl> AfterList = AB.After_tr(trg.line, ans_tbl.TableType, trg.shift, DateTime.Now);

                if (db.Summary_tbl.Any(s => s.TrigID == id))
                {
                    summ = db.Summary_tbl.Where(s => s.TrigID == id).FirstOrDefault();
                    summ.LastEdited = DateTime.Now;
                    summ.SafetyBefore = BeforeList.Any();
                    summ.SafetyAfter = AfterList.Any();
                    db.Entry(summ).State = EntityState.Modified;
                    db.SaveChanges();

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

                }
                return RedirectToAction("Index", new { TID = id });
            }
            ViewBag.SqID = new SelectList(db.Questions_tbl, "q_Id", "question", ans_tbl.q_ID);
            ViewBag.TriggerID = new SelectList(db.Triggers_tbl, "triggerID", "shift", ans_tbl.triggerID);
            return View(ans_tbl);
        }
    }
}