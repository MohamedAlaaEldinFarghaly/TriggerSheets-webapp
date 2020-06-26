using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TriggerSheets.Models
{
    [MetadataType(typeof(Triggers_tbl))]
    public partial class Triggers_tbl
    {
         public Triggers_tbl()
        {
            this.Answers_tbl = new HashSet<Answers_tbl>();
            this.Answers_tbl1 = new HashSet<Answers_tbl>();
            this.Summary_tbl = new HashSet<Summary_tbl>();

            this.daydate = DateTime.Now;
            int hour = this.daydate.Hour;
            if (hour>= 15 && hour<23 )
            {
                this.shift = "C";
            }else if(hour >=7 && hour<15)
            {
                this.shift="B";
            }
            else
            {
                this.shift = "A";
            }
        }

         //public virtual ICollection<Answers_tbl> Answers_tbl { get; set; }
    }
    public class trig
    {
        
    
        public long TriggerID { get; set; }

        [Display(Name="Line Number")]
        public int line { get; set; }
        public string shift { get; set; }

        [Display(Name="Time")]
        public System.DateTime daydate { get; set; }
        public Nullable<long> usernum { get; set; }

        [Display(Name="Trigger Type")]
        public string TableType { get; set; }
    
        public virtual ICollection<Answers_tbl> Answers_tbl { get; set; }
    }
}