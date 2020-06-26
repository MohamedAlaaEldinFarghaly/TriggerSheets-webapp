using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;

namespace TriggerSheets.Models
{
    public class BeforeAfter
    {
        private QSEntities1 db = new QSEntities1();

    public BeforeAfter()
        {

        }

    public List<Answers_tbl> Before_tr(int line,string Type, string Shift, DateTime Date_)
        {


            var answers_tbl = db.Answers_tbl.Include(a => a.Triggers_tbl).Include(a => a.Questions_tbl).ToList();
            List<Answers_tbl> Be = answers_tbl.Where(x => x.Triggers_tbl.line == line && x.TableType==Type).ToList();
            List<Answers_tbl> Bef = Be.Where(x => x.states == true).ToList();
            List<Answers_tbl> Befo = Bef.Where(x => x.Triggers_tbl.daydate.Day < Date_.Day || (String.Compare(x.Triggers_tbl.shift, Shift) < 0 && x.Triggers_tbl.daydate.Day == Date_.Day)).ToList();
            List<Answers_tbl> ex = Befo.Where(x => x.done).ToList();
            List<Answers_tbl> exc = ex.Where(x => x.Triggers_tbl1.daydate.Day < Date_.Day || (String.Compare(x.Triggers_tbl1.shift, Shift) < 0 && x.Triggers_tbl1.daydate.Day == Date_.Day)).ToList();
            List<Answers_tbl> Before = Befo.Except(exc).ToList();
            //List<Answers_tbl> Bef = Be.Where(x => x.done == false  ).ToList();// && x.done == false && ((String.Compare(x.Triggers_tbl.shift, Shift) < 0 && x.Triggers_tbl.daydate == Date_) || (x.Triggers_tbl.daydate < Date_))).ToList();
            //List<Answers_tbl> Before = Bef.Where(x => x.Triggers_tbl.daydate.Date < Date_.Date).ToList();

            return Before;
        }


        public List<Answers_tbl> After_tr(int line, string Type,string Shift, DateTime Date_)
        {

            var answers_tbl = db.Answers_tbl.Include(a => a.Triggers_tbl).Include(a => a.Questions_tbl).ToList();
            List<Answers_tbl> Af = answers_tbl.Where(x => x.Triggers_tbl.line == line && x.TableType==Type).ToList();
            List<Answers_tbl> Aft = Af.Where(x => x.states == true).ToList();
            List<Answers_tbl> Afte = Aft.Where(x => x.Triggers_tbl.daydate.Day < Date_.Day || (String.Compare(x.Triggers_tbl.shift, Shift) <= 0 && x.Triggers_tbl.daydate.Day == Date_.Day)).ToList();
            List<Answers_tbl> ex = Afte.Where(x => x.done).ToList();
            List<Answers_tbl> exc = ex.Where(x => x.Triggers_tbl1.daydate.Day < Date_.Day || (String.Compare(x.Triggers_tbl1.shift, Shift) <= 0 && x.Triggers_tbl1.daydate.Day == Date_.Day)).ToList();
            List<Answers_tbl> After = Afte.Except(exc).ToList();

            return After;
        }

    }
}