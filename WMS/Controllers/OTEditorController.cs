using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.CustomClass;
using WMS.Models;

namespace WMS.Controllers
{
    public class OTEditorController : Controller
    {
        //
        // GET: /OTEditor/
        TAS2013Entities db = new TAS2013Entities();
        public ActionResult Index()
        {
            VMOTEntry vm = new VMOTEntry();
            return View(vm);
        }
        [HttpGet]
        public ActionResult EditorOT(VMOTEntry vmdata)
        {
            try
            {
                User LoggedInUser = Session["LoggedUser"] as User;
                List<EmpView> emps = new List<EmpView>();
                emps = db.EmpViews.Where(aa => aa.EmpNo == vmdata.EmpNo && aa.Status == true).ToList();
                emps = AssistantQuery.GetFilteredEmps(emps, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());

                if (emps.Count()>0 && vmdata.FromDate != null && vmdata.ToDate != null)
                {
                    List<ViewAttData> attdata = new List<ViewAttData>();
                    attdata = db.ViewAttDatas.Where(aa => aa.EmpNo == vmdata.EmpNo && aa.AttDate >= vmdata.FromDate && aa.AttDate <= vmdata.ToDate).ToList();
                    vmdata.dbViewAttData = attdata;
                    vmdata.Count = attdata.Count;
                }
                else
                {
                    ViewBag.Message = "There is no employee found";
                }
                return View("Index",vmdata);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public ActionResult UpdateApprovedOT(VMOTEntry vmdata)
        {
            if (vmdata.EmpNo != "" && vmdata.FromDate != null && vmdata.ToDate != null)
            {
                List<string> empDates = db.ViewAttDatas.Where(aa => aa.EmpNo == vmdata.EmpNo && aa.AttDate >= vmdata.FromDate && aa.AttDate <= vmdata.ToDate).Select(aa=>aa.EmpDate).ToList();
                foreach (var empDate in empDates)
                {
                    if (Request.Form["EmpDate-" + empDate].ToString() != null && Request.Form["EmpDate-" + empDate].ToString() !="")
                    {
                        AttData attData = db.AttDatas.First(aa => aa.EmpDate == empDate);
                        attData.ApprovedOT = Convert.ToInt16(Request.Form["EmpDate-" + empDate].ToString());
                        db.SaveChanges();
                    }
                }
            }
            vmdata.dbViewAttData = db.ViewAttDatas.Where(aa => aa.EmpNo == vmdata.EmpNo && aa.AttDate >= vmdata.FromDate && aa.AttDate <= vmdata.ToDate).ToList();
            return View("Index",vmdata);
        }
    }
    public class VMOTEntry
    {
        public int EmpID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string EmpNo { get; set; }
        public int Count { get; set; }
        public List<ViewAttData> dbViewAttData;
    }
}