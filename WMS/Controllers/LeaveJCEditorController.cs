using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.CustomClass;
using WMS.HelperClass;
using WMS.Models;

namespace WMS.Controllers
{
    public class LeaveJCEditorController : Controller
    {
        //
        // GET: /LeaveJCEditor/
        TAS2013Entities db = new TAS2013Entities();
        public ActionResult Index()
        {
            VMLeaveEditor vm = new VMLeaveEditor();
            return View(vm);
        }
        public ActionResult JCLeaveEditor(VMLeaveEditor vmdata)
        {
            List<string> dropDownValues = new List<string>();
            dropDownValues.Add("None");
            dropDownValues.AddRange(db.JobCards.Select(aa => aa.WorkCardName).ToList());
            dropDownValues.AddRange(db.LvTypes.Select(aa => aa.LvDesc).ToList());
            dropDownValues.Add("HLV:Casual");
            dropDownValues.Add("HLV:Annual");
            vmdata.dropdowns = dropDownValues;
            User LoggedInUser = Session["LoggedUser"] as User;
            List<EmpView> emps = new List<EmpView>();
            emps = db.EmpViews.Where(aa => aa.EmpNo == vmdata.EmpNo && aa.Status == true).ToList();
            emps = AssistantQuery.GetFilteredEmps(emps, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());
            try
            {
                if (emps.Count()>0 && vmdata.FromDate != null && vmdata.ToDate != null)
                {
                    List<ViewAttData> attdata = new List<ViewAttData>();
                    attdata = db.ViewAttDatas.Where(aa => aa.EmpNo == vmdata.EmpNo && aa.AttDate >= vmdata.FromDate && aa.AttDate <= vmdata.ToDate && aa.StatusAB == true).ToList();
                    if (attdata.Count() == 0)
                    {
                        ViewBag.message = "No entry found";
                    }
                    vmdata.dbViewAttData = attdata;
                }
                else
                {
                    ViewBag.Messages = "There is no employee found";
                }
                return View("Index", vmdata);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult UpdateEditor(VMLeaveEditor datas)
        {
            List<string> dropDownValues = new List<string>();
            dropDownValues.Add("None");
            dropDownValues.AddRange(db.JobCards.Select(aa => aa.WorkCardName).ToList());
            dropDownValues.AddRange(db.LvTypes.Select(aa => aa.LvDesc).ToList());
            dropDownValues.Add("HLV:Casual");
            dropDownValues.Add("HLV:Annual");
            datas.dropdowns = dropDownValues;
            if (datas.EmpNo != "" && datas.FromDate != null && datas.ToDate != null)
            {
                List<string> empDates = db.ViewAttDatas.Where(aa => aa.EmpNo == datas.EmpNo && aa.AttDate >= datas.FromDate && aa.AttDate <= datas.ToDate && aa.StatusAB == true).Select(aa => aa.EmpDate).ToList();
                foreach (var empDate in empDates)
                {
                    if (Request.Form["EmpDate" + empDate].ToString() != null && Request.Form["EmpDate" + empDate].ToString() != "" && Request.Form["EmpDate" + empDate].ToString() != "None")
                    {
                        AttData attData = db.AttDatas.First(aa => aa.EmpDate == empDate);
                        string date = attData.AttDate.ToString();
                        var Name = Request.Form["EmpDate" + empDate].ToString();
                        if (Name.Count() > 0)
                        {
                            switch (Name)
                            {
                                case "JC:Official Duty":
                                    AddJobCardsIntoDatabse(empDate, datas, Name, date);
                                    break;
                                case "JC:Field Visit":
                                    AddJobCardsIntoDatabse(empDate, datas, Name, date);
                                    break;
                                case "JC:Training":
                                    AddJobCardsIntoDatabse(empDate, datas, Name, date);
                                    break;
                                case "LV:Casual":
                                    AddLeaveIntoDatabase(empDate, datas, Name, date, false);
                                    break;
                                case "LV:Annual":
                                    AddLeaveIntoDatabase(empDate, datas, Name, date, false);
                                    break;
                                case "HLV:Casual":
                                    AddLeaveIntoDatabase(empDate, datas, Name, date, true);
                                    break;
                                case "HLV:Annual":
                                    AddLeaveIntoDatabase(empDate, datas, Name, date, true);
                                    break;
                                case "LV:Sick":
                                    AddLeaveIntoDatabase(empDate, datas, Name, date, false);
                                    break;
                                case "LV:Ex Pakistan Leave":
                                    AddLeaveIntoDatabase(empDate, datas, Name, date, false);
                                    break;
                                case "LV:Study Leave":
                                    AddLeaveIntoDatabase(empDate, datas, Name, date, false);
                                    break;
                                case "LV:Optional Leave":
                                    AddLeaveIntoDatabase(empDate, datas, Name, date, false);
                                    break;
                                case "LV:Maternity Leave":
                                    AddLeaveIntoDatabase(empDate, datas, Name, date, false);
                                    break;
                                case "LV:Compensatory":
                                    AddLeaveIntoDatabase(empDate, datas, Name, date, false);
                                    break;
                                case "LV:Special Leave":
                                    AddLeaveIntoDatabase(empDate, datas, Name, date, false);
                                    break;
                                case "LV:Recreation Leave":
                                    AddLeaveIntoDatabase(empDate, datas, Name, date, false);
                                    break;
                                case "LV:Extraordinary Leave":
                                    AddLeaveIntoDatabase(empDate, datas, Name, date, false);
                                    break;
                            }
                        }
                        db.SaveChanges();
                    }
                }
            }
            datas.dbViewAttData = db.ViewAttDatas.Where(aa => aa.EmpNo == datas.EmpNo && aa.AttDate >= datas.FromDate && aa.AttDate <= datas.ToDate && aa.StatusAB == true).ToList();
            return View("Index", datas);
        }

        private void AddJobCardsIntoDatabse(string empDate, VMLeaveEditor datas, string Name, string date)
        {
            User LoggedInUser = Session["LoggedUser"] as User;
            int empid = db.Emps.First(aa => aa.EmpNo == datas.EmpNo).EmpID;
            int cardtypeid = db.JobCards.First(aa => aa.WorkCardName == Name).WorkCardID;
            JobCardApp jc = new JobCardApp();
            jc.DateStarted = Convert.ToDateTime(date);
            jc.DateEnded = Convert.ToDateTime(date);
            jc.CardType = (short)cardtypeid;
            jc.JobCardCriteria = "E";
            jc.CriteriaDate = empid;
            jc.Status = false;
            jc.UserID = LoggedInUser.UserID;
            List<Emp> emptemp = db.Emps.Where(aa => aa.EmpNo == datas.EmpNo && aa.Deleted == false).ToList();
            if (emptemp.Count > 0)
            {
                if (ValidateJobCard(jc))
                {
                    db.JobCardApps.Add(jc);
                    if (db.SaveChanges() > 0)
                        AddJobCardAppToJobCardData(datas);
                }
                else
                    ViewBag.validtonmessageforJC = "Job Card Validation failed";
            }
            else
                ViewBag.validtonmessageforJC = "There is no employee found";
        }

        private void AddJobCardAppToJobCardData(VMLeaveEditor datas)
        {
            List<JobCardApp> _jobCardApp = new List<JobCardApp>();
            _jobCardApp = db.JobCardApps.Where(aa => aa.Status == false).ToList();
            List<Emp> _Emp = new List<Emp>();
            foreach (var jcApp in _jobCardApp)
            {
                jcApp.Status = true;
                AddJobCardData(datas, jcApp);
            }
        }


        private void AddJobCardData(VMLeaveEditor datas, JobCardApp jcApp)
        {

            int _empID = db.Emps.First(aa => aa.EmpNo == datas.EmpNo).EmpID;
            string _empDate = "";
            int _userID = (int)jcApp.UserID;
            DateTime _Date = (DateTime)jcApp.DateStarted;
            while (_Date <= jcApp.DateEnded)
            {
                _empDate = _empID + _Date.ToString("yyMMdd");
                AddJobCardDataToDatabase(_empDate, _empID, _Date, _userID, jcApp);
                db.SaveChanges();
                if (db.AttDatas.Where(aa => aa.EmpDate == _empDate).Count() > 0)
                {
                    //1	Official Duty
                    //2	Present
                    //3	Absent
                    //5	Special Holiday
                    switch (jcApp.CardType)
                    {

                        case 1:// Official Duty
                            AddJCToAttData(_empDate, _empID, _Date, "O", false, false, false, false, true, "Offical Duty");
                            break;
                        case 5:// Field Visit
                            AddJCToAttData(_empDate, _empID, _Date, "O", false, false, false, false, true, "Field Visit");
                            break;
                        case 7:// Training
                            AddJCToAttData(_empDate, _empID, _Date, "O", false, false, false, false, true, "Training");
                            break;
                    }
                }
                _Date = _Date.AddDays(1);
            }
        }
        private bool AddJCToAttData(string _empDate, int _empID, DateTime _Date, string dutyCode, bool statusAB, bool statusDO,
            bool statusLeave, bool statusGZ, bool statusP, string Remarks)
        {
            bool check = false;
            try
            {
                using (var context = new TAS2013Entities())
                {
                    AttData _attdata = context.AttDatas.FirstOrDefault(aa => aa.EmpDate == _empDate);
                    if (_attdata != null)
                    {
                        if (statusAB == true)
                        {
                            _attdata.PDays = 0;
                            _attdata.LeaveDays = 0;
                            _attdata.ABDays = 1;
                        }
                        if (statusP == true)
                        {
                            _attdata.PDays = 1;
                            _attdata.LeaveDays = 0;
                            _attdata.ABDays = 0;
                        }
                        _attdata.DutyCode = dutyCode;
                        _attdata.StatusAB = statusAB;
                        _attdata.StatusDO = statusDO;
                        _attdata.StatusLeave = statusLeave;
                        _attdata.StatusGZ = statusGZ;
                        _attdata.StatusP = statusP;
                        if (_attdata.StatusAB != true)
                            _attdata.WorkMin = _attdata.ShifMin;
                        else
                            _attdata.WorkMin = 0;
                        if (_attdata.StatusDO == true)
                            _attdata.WorkMin = 0;
                        _attdata.Remarks = Remarks;
                    }
                    context.SaveChanges();
                    if (context.SaveChanges() > 0)
                        check = true;
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
            }
            return check;
        }
        private bool AddJobCardDataToDatabase(string _empDate, int _empID, DateTime _currentDate, int _userID, JobCardApp jcApp)
        {
            bool check = false;
            try
            {
                JobCardDetail _jobCardEmp = new JobCardDetail();
                _jobCardEmp.EmpDate = _empDate;
                _jobCardEmp.EmpID = _empID;
                _jobCardEmp.Dated = _currentDate;
                _jobCardEmp.WrkCardID = jcApp.CardType;
                _jobCardEmp.JCAppID = jcApp.JobCardID;
                _jobCardEmp.Remarks = jcApp.Remarks;
                db.JobCardDetails.Add(_jobCardEmp);
                if (db.SaveChanges() > 0)
                {
                    check = true;
                }
            }
            catch (Exception ex)
            {
                check = false;
            }
            return check;
        }

        private bool ValidateJobCard(JobCardApp jc)
        {
            List<JobCardApp> _Lc = new List<JobCardApp>();
            DateTime _DTime = new DateTime();
            DateTime _DTimeLV = new DateTime();
            _Lc = db.JobCardApps.Where(aa => aa.CriteriaDate == jc.CriteriaDate).ToList();
            foreach (var item in _Lc)
            {
                _DTime = (DateTime)item.DateStarted;
                _DTimeLV = (DateTime)jc.DateStarted;
                while (_DTime <= item.DateEnded)
                {
                    while (_DTimeLV <= jc.DateEnded)
                    {
                        if (_DTime == _DTimeLV)
                            return false;
                        _DTimeLV = _DTimeLV.AddDays(1);
                    }
                    _DTime = _DTime.AddDays(1);
                }
            }
            return true;
        }

        private void AddLeaveIntoDatabase(string empDate, VMLeaveEditor datas, string Name, string date, bool IsHalf)
        {
            int empid = db.Emps.First(aa => aa.EmpNo == datas.EmpNo).EmpID;
            LvType lvtype = new LvType();
            if (Name == "HLV:Casual")
            {
                lvtype = db.LvTypes.First(aa => aa.LvDesc == "LV:Casual");
            }
            else if (Name == "HLV:Annual")
            {
                lvtype = db.LvTypes.First(aa => aa.LvDesc == "LV:Annual");
            }
            else
            {
                lvtype = db.LvTypes.First(aa => aa.LvDesc == Name);
            }
            User LoggedInUser = Session["LoggedUser"] as User;
            LvApplication lv = new LvApplication();
            lv.EmpID = empid;
            lv.LeaveTypeID = lvtype.LvTypeID;
            lv.LvDate = DateTime.Today;
            if (IsHalf == true)
            {
                lv.NoOfDays = (float)0.5;
                lv.IsHalf = true;
            }
            else
            {
                lv.NoOfDays = 1;
            }
            lv.CreatedBy = LoggedInUser.UserID;
            lv.FromDate = Convert.ToDateTime(date);
            lv.ToDate = Convert.ToDateTime(date);
            if (lvtype.UpdateBalance == true)
            {
                if (HasLeaveQuota(empid, lvtype.LvTypeID, lvtype))
                {
                    if (CheckLeaveBalance(lv, lvtype))
                    {
                        CreateLeave(lv, lvtype);
                    }
                    else
                        ViewBag.validtonmessage = "Leave Balance Exceeds, Please check the balance";
                }
                else
                    ViewBag.validtonmessage = "Leave Quota does not exist";
            }
            else
            {
                CreateLeave(lv, lvtype);
            }
        }

        private void CreateHalfLeave()
        {
            throw new NotImplementedException();
        }

        private void CreateLeave(LvApplication lv, LvType lvTypes)
        {
            LeaveController LvProcessController = new LeaveController();
            lv.LvDate = DateTime.Today;
            int _userID = Convert.ToInt32(Session["LogedUserID"].ToString());
            lv.CreatedBy = _userID;
            lv.Active = true;
            db.LvApplications.Add(lv);
            if (db.SaveChanges() > 0)
            {
                HelperClass.MyHelper.SaveAuditLog(_userID, (byte)MyEnums.FormName.Leave, (byte)MyEnums.Operation.Add, DateTime.Now);
                LvProcessController.AddLeaveToLeaveData(lv, lvTypes);
                LvProcessController.AddLeaveToLeaveAttData(lv, lvTypes);
            }
            else
            {
                ViewBag.validtonmessage = "There is an error while creating leave.";
            }
        }

        private bool CheckLeaveBalance(LvApplication lv, LvType lvTypes)
        {
            bool balance = false;
            decimal RemainingLeaves;
            if (lvTypes.UpdateBalance == true)
            {
                using (var ctx = new TAS2013Entities())
                {
                    List<LvConsumed> _lvConsumed = new List<LvConsumed>();
                    string empLvType = lv.EmpID.ToString() + lv.LeaveTypeID + lv.FromDate.Year.ToString("0000");
                    _lvConsumed = ctx.LvConsumeds.Where(aa => aa.EmpLvTypeYear == empLvType).ToList();
                    if (_lvConsumed.Count > 0)
                    {
                        RemainingLeaves = (decimal)_lvConsumed.FirstOrDefault().YearRemaining;
                        if ((RemainingLeaves - Convert.ToDecimal(lv.NoOfDays)) >= 0)
                        {
                            balance = true;
                        }
                        else
                            balance = false;
                    }
                    else
                        balance = false;

                }
            }
            else
                balance = true;

            return balance;
        }
        public bool HasLeaveQuota(int empID, string lvType, LvType leaveType)
        {
            bool check = false;
            using (var ctx = new TAS2013Entities())
            {
                List<LvConsumed> lv = new List<LvConsumed>();
                lv = ctx.LvConsumeds.Where(aa => aa.EmpID == empID && aa.LeaveTypeID == lvType).ToList();
                if (lv.Count > 0)
                    check = true;
            }
            return check;
        }
    }
    public class VMLeaveEditor
    {
        public int EmpID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string EmpNo { get; set; }
        public string SectionName { get; set; }
        public int Count { get; set; }
        public List<ViewAttData> dbViewAttData;
        public List<string> dropdowns;
    }
}