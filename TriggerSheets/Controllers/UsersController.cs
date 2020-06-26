using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using TriggerSheets.Models;

namespace TriggerSheets.Controllers
{
    public class UsersController : Controller
    {
        private QSEntities1 db = new QSEntities1();

        // GET: /Line/
        public ActionResult Index(string searchBy, string search)
        {
            if (searchBy == "Name")
            {
                return View(db.User_Line.Where(x => x.Name.StartsWith(search) || search == null).ToList());
            }
            else
            {


                if (searchBy == "User_num")
                {
                    return View(db.User_Line.Where(y => y.User_num == search || search == null).ToList());

                }
                else
                {
                    int line = Convert.ToInt16(search);
                    return View(db.User_Line.Where(z => z.Line == line || search == null).ToList());

                }

                //return View(db.User_Line.ToList());
            }
        }

        // GET: /Line/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Line user_line = db.User_Line.Find(id);
            if (user_line == null)
            {
                return HttpNotFound();
            }
            return View(user_line);
        }

        // GET: /Line/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Line/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,User_num,Name,Line,Type")] User_Line user_line)
        {
            if (ModelState.IsValid)
            {
                db.User_Line.Add(user_line);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user_line);
        }

        // GET: /Line/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Line user_line = db.User_Line.Find(id);
            if (user_line == null)
            {
                return HttpNotFound();
            }
            return View(user_line);
        }

        // POST: /Line/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,User_num,Name,Line,Type")] User_Line user_line)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user_line).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user_line);
        }

        // GET: /Line/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Line user_line = db.User_Line.Find(id);
            if (user_line == null)
            {
                return HttpNotFound();
            }
            return View(user_line);
        }

        // POST: /Line/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User_Line user_line = db.User_Line.Find(id);
            db.User_Line.Remove(user_line);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult DeleteAll()
        {
            string username =Session["username"].ToString();
            long AdminID = db.User_Line.Where(u => u.User_num == username).FirstOrDefault().ID;
            db.User_Line.RemoveRange(db.User_Line.Where(i => i.ID != AdminID));
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

        public ActionResult FromExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FromExcel(HttpPostedFileBase file)
        {
            DataSet ds = new DataSet();
            if (Request.Files["file"].ContentLength > 0)
            {
                string fileExtension =
                                     System.IO.Path.GetExtension(Request.Files["file"].FileName);
                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["file"].SaveAs(fileLocation);
                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //connection String for xls file format.
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        @ViewBag.Message = "Error";
                        return View();
                    }
                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);

                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection);
                    
                        
                        dataAdapter.Fill(ds);

                        excelConnection.Close();
                    }
                if (fileExtension.ToString().ToLower().Equals(".xml"))
                {
                    string fileLocation = Server.MapPath("~/Content/") + Request.Files["FileUpload"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }

                    Request.Files["FileUpload"].SaveAs(fileLocation);
                    XmlTextReader xmlreader = new XmlTextReader(fileLocation);
                    ds.ReadXml(xmlreader);
                    xmlreader.Close();
                    System.IO.File.Delete(fileLocation);
                }

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string conn = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
                        SqlConnection con = new SqlConnection(conn);
                        string Query = "Insert into User_Line(User_num,Name,Line,Type) Values('" + ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() + "','" + ds.Tables[0].Rows[i][2].ToString() + "','" + ds.Tables[0].Rows[i][3].ToString() + "')";
                        con.Open();
                        SqlCommand cmd = new SqlCommand(Query, con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return RedirectToAction("Index", "Users");
            }

        }
    }
