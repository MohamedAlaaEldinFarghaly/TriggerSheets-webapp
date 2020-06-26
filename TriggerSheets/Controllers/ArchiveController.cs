//using NPOI.HSSF.UserModel;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TriggerSheets.Models;
using Syncfusion.XlsIO;
using System.Drawing;

namespace TriggerSheets.Controllers
{
    public class ArchiveController : Controller
    {
        private QSEntities1 db = new QSEntities1();

        public ActionResult Choose()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Choose(string command, string Start, string End)
        {
            string s = command;
            if (s=="Shifts")
            {
                return RedirectToAction("Index", new {Start =Start, End = End});
            }
            else if (s=="Late")
            {
                return RedirectToAction("Notify", new { Start = Start, End = End });
            }

            return View();
        }

        // GET: /Archive/
        public ActionResult Index(string Start, string End)
        {
            DateTime startdate = Convert.ToDateTime(Start).AddDays(-1);
            DateTime Enddate = Convert.ToDateTime(End).AddDays(1);
            var summary_tbl = db.Summary_tbl.Include(s => s.Triggers_tbl).Where(u => u.Triggers_tbl.daydate > startdate && u.Triggers_tbl.daydate < Enddate);
            var sum = summary_tbl.ToList().OrderBy(a => a.Inserted).ThenBy(b => b.Triggers_tbl.line).ThenBy(c => c.Triggers_tbl.shift);
            
            return View(sum);
        }
      
        // GET :/Archive/Notify
        public ActionResult Notify(string Start, string End)
        {
            DateTime startdate = Convert.ToDateTime(Start).AddDays(-1);
            DateTime Enddate = Convert.ToDateTime(End).AddDays(1);
            
            var summary_tbl = db.Summary_tbl.Include(s => s.Triggers_tbl).Where(u => u.Triggers_tbl.daydate > startdate && u.Triggers_tbl.daydate < Enddate);
            var sum = summary_tbl.ToList().OrderBy(a => a.Inserted).ThenBy(b => b.Triggers_tbl.line).ThenBy(c => c.Triggers_tbl.shift);

            return View(sum);
        }
        [HttpPost]

        public ActionResult Excelpost()
        {
            var summary_tbl = db.Summary_tbl.Include(s => s.Triggers_tbl);
            var sum = summary_tbl.ToList().OrderBy(a => a.Inserted).ThenBy(b => b.Triggers_tbl.line).ThenBy(c => c.Triggers_tbl.shift);
            string name = "Archive.xlsx";
            String ext = "xlsx";

            //Create an instance of ExcelEngine.
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                //Set the default application version as Excel 2016.
                excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2016;

                //Create a workbook with a worksheet.
                IWorkbook workbook = excelEngine.Excel.Workbooks.Create(1);

                //Access first worksheet from the workbook instance.
                IWorksheet sheet1 = workbook.Worksheets[0];
               

                //            //make a header row

                sheet1[1, 1].Text = "Date";
                sheet1[1, 2].Text = "Line";
                sheet1[1, 3].Text = "Shift";
                sheet1[1, 4].Text = "Safety_Before";
                sheet1[1, 5].Text = "Safety_After";
                sheet1[1, 6].Text = "Quality_Before";
                sheet1[1, 7].Text = "Quality_After";
                sheet1[1, 8].Text = "Inserted_Time";
                sheet1[1, 9].Text = "Last_Edited";


                            //loops through data
                            
                            int i = 1;
                            foreach (var item in sum)
                            {

                                i++;
                                sheet1[i, 1].ColumnWidth = 11;
                                sheet1[i, 1].NumberFormat = "mm/dd/yyyy";
                                sheet1[i, 1].DateTime = item.Triggers_tbl.daydate.Date;
                                sheet1[i, 2].Number = item.Triggers_tbl.line;
                                sheet1[i, 3].Text = item.Triggers_tbl.shift;

                                bool x = item.SafetyBefore.HasValue ? item.SafetyBefore.Value : false;
                                string Safety_Before = x ? "HIGH" : "LOW";
                                sheet1[i, 4].Text = Safety_Before;
                                Color c = x ? Color.Red : Color.Green;
                                sheet1[i, 4].CellStyle.Font.RGBColor = c;
                                sheet1[i, 4].ColumnWidth = 13;

                                x = item.SafetyAfter.HasValue ? item.SafetyAfter.Value : false;
                                string Safety_After = x ? "HIGH" : "LOW";
                                sheet1[i, 5].Text = Safety_After;
                                c = x ? Color.Red : Color.Green;
                                sheet1[i, 5].CellStyle.Font.RGBColor = c;
                                sheet1[i, 5].ColumnWidth = 11;

                                x = item.QualityBefore.HasValue ? item.QualityBefore.Value : false;
                                string Quality_Before = x ? "HIGH" : "LOW";
                                sheet1[i, 6].Text = Quality_Before;
                                c = x ? Color.Red : Color.Green;
                                sheet1[i, 6].CellStyle.Font.RGBColor = c;
                                sheet1[i, 6].ColumnWidth = 14;

                                x = item.QualityAfter.HasValue ? item.QualityAfter.Value : false;
                                string Quality_After = x ? "HIGH" : "LOW";
                                sheet1[i, 7].Text = Quality_After;
                                c = x ? Color.Red : Color.Green;
                                sheet1[i, 7].CellStyle.Font.RGBColor = c;
                                sheet1[i, 7].ColumnWidth = 12;

                                sheet1[i, 8].ColumnWidth = 20;
                                sheet1[i, 8].Text = item.Inserted.ToString();
                                sheet1[i, 9].ColumnWidth= 20;
                                sheet1[i, 9].Text = item.LastEdited.ToString();
                            }
                //Insert sample text into cell “A1”.
                //worksheet.Range["A1"].Text = "Hello World";
                
                //Save the workbook to disk in xlsx format.
                workbook.SaveAs("Archive.xlsx", HttpContext.ApplicationInstance.Response, ExcelDownloadType.Open);
            }
            return View();
        }

    }
}

