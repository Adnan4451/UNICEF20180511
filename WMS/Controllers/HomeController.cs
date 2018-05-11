using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using WMS.Controllers.Filters;
using WMS.HelperClass;
using WMS.Models;
using System.DirectoryServices;
using System.Linq.Dynamic;
using System.DirectoryServices.AccountManagement;
using System.Data;
using System.Drawing;
using System.Management;
using System.Net.NetworkInformation;
using WMS.CustomClass;
namespace WMS.Controllers
{
    public class HomeController : Controller
    {
        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }
        //
        // GET: /Home/
        TAS2013Entities db = new TAS2013Entities();
        public ActionResult Index()
        {
            ViewBag.Location = new SelectList(db.Locations.OrderBy(s => s.LocName), "LocID", "LocName");
            try
            {
                if (db.Options.ToList().Count > 0)
                {

                    SetGlobalVaribale();
                    Session["CompanyName"] = db.Options.FirstOrDefault().CompanyName;
                    if (CheckForValidLicense("Server"))
                    {
                        if (Session["LogedUserID"] == null)
                        {
                            Session["LogedUserID"] = "";
                            Session["Role"] = "";
                            Session["MHR"] = "0";
                            Session["MDevice"] = "0";
                            Session["MLeave"] = "0";
                            Session["MEditAtt"] = "0";
                            Session["MUser"] = "0";
                            Session["LogedUserFullname"] = "";
                            Session["UserCompany"] = "";
                            Session["MRDailyAtt"] = "0";
                            Session["MRLeave"] = "0";
                            Session["MRMonthly"] = "0";
                            Session["MRAudit"] = "0";
                            Session["MRManualEditAtt"] = "0";
                            Session["MREmployee"] = "0";
                            Session["MRoster"] = "0";
                            Session["MRDetail"] = "0";
                            Session["MRSummary"] = "0";
                            Session["MProcess"] = "0";
                            return View();
                        }
                        else if (Session["LogedUserID"].ToString() == "")
                        {
                            return View();
                        }
                        else
                        {
                            return View("AfterLogin");
                        }
                    }
                    else
                    {
                        return View("LoadLicense");
                    }
                }
                else
                {
                    Session["CompanyName"] = "No Company";
                    return View("CompanyInfo");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult SaveCompanyInfo(HttpPostedFileBase uploadFile)
        {
            string CompanyName = Request.Form["CompanyName"].ToString();
            string EPath = Request.Form["Path"].ToString();
            if (uploadFile.ContentLength > 0)
            {
                string filePath = Path.GetFileName(uploadFile.FileName);
                uploadFile.SaveAs(EPath + filePath);
                //SaveImage("E:\\air.png");
                SaveImage(EPath + filePath, CompanyName, EPath);
            }
            return RedirectToAction("Index");
        }
        private void SaveImage(string fileaddress, string ComName, string EPath)
        {
            //image to byteArray
            Image img = Image.FromFile(fileaddress);
            byte[] bArr = imgToByteArray(img);
            //byte[] bArr = imgToByteConverter(img);
            //Again convert byteArray to image and displayed in a picturebox
            TAS2013Entities ctx = new TAS2013Entities();
            Option oo = new Option();
            oo.CompanyLogo = bArr;
            oo.CompanyName = ComName;
            oo.ServerFilePath = EPath;
            oo.ID = 1;
            ctx.Options.Add(oo);
            ctx.SaveChanges();
        }
        private void AdjustLeaves()
        {
            using (var mydb = new TAS2013Entities())
            {
                List<LvApplication> lvApps = new List<LvApplication>();
                lvApps = mydb.LvApplications.ToList();
                foreach (var lv in lvApps)
                {
                    string empLvYear = lv.EmpID.ToString() + lv.LeaveTypeID.ToString() + "2016";
                    List<LvConsumed> lvcon = new List<LvConsumed>();
                    lvcon = mydb.LvConsumeds.Where(aa => aa.EmpID == lv.EmpID && aa.EmpLvTypeYear == empLvYear).ToList();
                    if (lvcon.Count > 0)
                    {
                        lvcon.FirstOrDefault().YearRemaining = lvcon.FirstOrDefault().YearRemaining - lv.NoOfDays;
                        lvcon.FirstOrDefault().GrandTotalRemaining = lvcon.FirstOrDefault().GrandTotalRemaining - lv.NoOfDays;
                        mydb.SaveChanges();
                    }
                }
            }
        }

        private void SetGlobalVaribale()
        {
            using (var db = new TAS2013Entities())
            {
                GlobalVaribales.ServerPath = db.Options.FirstOrDefault().ServerFilePath;
                if (db.LicenseInfoes.Count() > 0)
                {
                    LicenseInfo li = new LicenseInfo();
                    li = db.LicenseInfoes.FirstOrDefault();
                    GlobalVaribales.NoOfDevices = StringCipher.Decrypt(li.NoOfDevices, "1234");
                    GlobalVaribales.NoOfEmps = StringCipher.Decrypt(li.NoOfEmps, "1234");
                    GlobalVaribales.NoOfUsers = StringCipher.Decrypt(li.NoOfUsers, "1234");
                    GlobalVaribales.DeviceType = StringCipher.Decrypt(li.DeviceType, "1234");
                    GlobalVaribales.LicenseType = StringCipher.Decrypt(li.LicenseType, "1234");
                }
                db.Dispose();
            }
        }

        #region --License--
        [HttpPost]
        public ActionResult LoadLicense(HttpPostedFileBase uploadFile)
        {
            if (uploadFile.ContentLength > 0)
            {
                string filePath = Path.GetFileName(uploadFile.FileName);
                uploadFile.SaveAs(GlobalVaribales.ServerPath + filePath);
                ReadFile(GlobalVaribales.ServerPath + filePath);
            }
            return RedirectToAction("Index");
        }

        private void ReadFile(string LicensePath)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(LicensePath);
            string line;
            int CurrentlineNo = 0;
            string InvoiceNo = "";
            string CustomerName = "";
            string LicenseType = "";
            string DeviceType = "";
            string ClientMac = "";
            string NoOfEmps = "";
            string NoOfUsers = "";
            string NoOfDevices = "";
            List<string> DeviceMacs = new List<string>();
            var fileLines = new List<string>();
            while ((line = file.ReadLine()) != null)
            {
                if (CurrentlineNo == 0)
                    InvoiceNo = line;
                if (CurrentlineNo == 1)
                    CustomerName = line;
                if (CurrentlineNo == 2)
                    LicenseType = line;
                if (CurrentlineNo == 3)
                    DeviceType = line;
                if (CurrentlineNo == 4)
                    ClientMac = line;
                if (CurrentlineNo == 5)
                    NoOfEmps = line;
                if (CurrentlineNo == 6)
                    NoOfUsers = line;
                if (CurrentlineNo == 7)
                    NoOfDevices = line;
                if (CurrentlineNo > 7)
                    DeviceMacs.Add(line);
                CurrentlineNo++;
            }
            TAS2013Entities db = new TAS2013Entities();
            string DBMACAddress = StringCipher.Decrypt(ClientMac, "1234");
            string SystemMACAdress = GetClientMacAddress();
            if (DBMACAddress == SystemMACAdress)
            {
                LicenseInfo li = new LicenseInfo();
                if (db.LicenseInfoes.Count() > 0)
                {
                    li = db.LicenseInfoes.FirstOrDefault();
                }
                li.ID = 1;
                li.InvoiceNo = InvoiceNo;
                li.LicenseType = LicenseType;
                li.NoOfDevices = NoOfDevices;
                li.NoOfEmps = NoOfEmps;
                li.NoOfUsers = NoOfUsers;
                li.CustomerName = CustomerName;
                li.ClientMAC = ClientMac;
                li.DeviceType = DeviceType;
                li.ValidLicense = StringCipher.Encrypt("1", "1234");
                if (db.LicenseInfoes.Count() == 0)
                {
                    db.LicenseInfoes.Add(li);
                }
                db.SaveChanges();
                foreach (var item in db.LicenseDeviceInfoes.ToList())
                {
                    db.LicenseDeviceInfoes.Remove(item);
                }
                db.SaveChanges();
                byte count = 1;
                foreach (var item in DeviceMacs)
                {
                    LicenseDeviceInfo ldi = new LicenseDeviceInfo();
                    ldi.DeviceID = count;
                    ldi.DeviceMAC = item;
                    db.LicenseDeviceInfoes.Add(ldi);
                    count++;
                }
                db.SaveChanges();
            }
        }

        private bool CheckForValidLicense(string DevUser)
        {
            bool valid = false;
            if (DevUser != "Server")
            {
                try
                {
                    using (var db = new TAS2013Entities())
                    {
                        if (db.LicenseInfoes.ToList().Count > 0)
                        {
                            LicenseInfo li = new LicenseInfo();
                            li = db.LicenseInfoes.FirstOrDefault();
                            string val = StringCipher.Decrypt(li.ValidLicense, "1234");
                            if (val == "1")
                            {
                                string ClientMAC = GetClientMacAddress();
                                string DatabaseMac = StringCipher.Decrypt(li.ClientMAC, "1234");
                                if (ClientMAC == DatabaseMac)
                                    valid = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    valid = false;
                }
            }
            else
                return true;
            return valid;
        }
        //private bool CheckForValidLicense()
        //{
        //    bool valid;

        //    return true;
        //}
        public static string GetClientMacAddress()
        {
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            string mac = "";
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {

                    mac = adapter.GetPhysicalAddress().ToString();

                }
            }
            return mac;
        }
        #endregion
        private void SaveImage()
        {
            //image to byteArray
            Image img = Image.FromFile("E:\\air.png");
            byte[] bArr = imgToByteArray(img);
            //byte[] bArr = imgToByteConverter(img);
            //Again convert byteArray to image and displayed in a picturebox
            TAS2013Entities ctx = new TAS2013Entities();
            Option oo = new Option();
            oo = ctx.Options.First(aa => aa.ID == 1);
            oo.CompanyLogo = bArr;
            ctx.SaveChanges();
        }
        //convert image to bytearray
        public byte[] imgToByteArray(Image img)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                img.Save(mStream, img.RawFormat);
                return mStream.ToArray();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User u)
        {
            ViewBag.Location = new SelectList(db.Locations.OrderBy(s => s.LocName), "LocID", "LocName");
            try
            {
                if (ModelState.IsValid) // this is check validity
                {
                    using (TAS2013Entities dc = new TAS2013Entities())
                    {
                        List<User> users = new List<Models.User>();
                        //int NoOfUsres = Convert.ToInt32(GlobalVaribales.NoOfUsers);
                        int NoOfUsres = 200;
                        users = dc.Users.Where(aa => aa.Deleted == false).ToList();
                        var usr = users.Take(NoOfUsres);
                        var v = usr.Where(a => a.UserName.ToUpper().Equals(u.UserName.ToUpper()) && a.Password.ToUpper() == u.Password.ToUpper()).FirstOrDefault();
                        //login for emplioyee
                        if (v != null)
                        {

                            Session["MDevice"] = "0";
                            Session["MHR"] = "0";
                            Session["MDevice"] = "0";
                            Session["MLeave"] = "0";
                            Session["MEditAtt"] = "0";
                            Session["MUser"] = "0";
                            Session["LogedUserFullname"] = "";
                            Session["UserCompany"] = "";
                            Session["MRDailyAtt"] = "0";
                            Session["MRLeave"] = "0";
                            Session["MRMonthly"] = "0";
                            Session["MRAudit"] = "0";
                            Session["MRManualEditAtt"] = "0";
                            Session["MREmployee"] = "0";
                            Session["MRDetail"] = "0";
                            Session["MRSummary"] = "0";
                            Session["LogedUserID"] = v.UserID.ToString();
                            Session["LogedUserFullname"] = v.UserName;
                            Session["LoggedUser"] = v;
                            Session["UserLocations"] = dc.UserSections.Where(aa => aa.UserID == v.UserID).ToList();
                            if (v.MHR == true)
                                Session["MHR"] = "1";
                            if (v.MDevice == true)
                                Session["MDevice"] = "1";
                            if (v.MLeave == true)
                                Session["MLeave"] = "1";
                            if (v.MEditAtt == true)
                                Session["MEditAtt"] = "1";
                            if (v.MUser == true)
                                Session["MUser"] = "1";
                            if (v.MRDailyAtt == true)
                                Session["MRDailyAtt"] = "1";
                            if (v.MRLeave == true)
                                Session["MRLeave"] = "1";
                            if (v.MRMonthly == true)
                                Session["MRMonthly"] = "1";
                            if (v.MRAudit == true)
                                Session["MRAudit"] = "1";
                            if (v.MRManualEditAtt == true)
                                Session["MRManualEditAtt"] = "1";
                            if (v.MProcess == true)
                                Session["MProcess"] = "1";
                            if (v.MREmployee == true)
                                Session["MREmployee"] = "1";
                            if (v.MRDetail == true)
                                Session["MRDetail"] = "1";

                            if (v.MRoster == true)
                                Session["MRoster"] = "1";

                            HelperClass.MyHelper.SaveAuditLog(v.UserID, (byte)MyEnums.FormName.LogIn, (byte)MyEnums.Operation.LogIn, DateTime.Now);
                            return RedirectToAction("AfterLogin");
                        }
                        else
                        {
                            int LoginCount = 0;
                            bool successOnConversion = int.TryParse(Session["LoginCount"] as string, out LoginCount);
                            if (successOnConversion == true)
                            {
                                LoginCount++;
                                Session["LoginCount"] = LoginCount + "";
                            }
                            else
                            {
                                Session["LoginCount"] = "1";
                            }


                        }
                    }
                }
                return RedirectToAction("index");

            }
            catch (Exception ex)
            {
                ViewBag.Message = "There seems to be a problem with the network. Please contact your network administrator";
                return RedirectToAction("Index");
            }
        }
        public ActionResult AfterLogin()
        {
            User LoggedInUser = Session["LoggedUser"] as User;
            var locationid = db.UserSections.FirstOrDefault(aa => aa.UserID == LoggedInUser.UserID).LocationID;
            var Ulocname = db.Locations.FirstOrDefault(aa => aa.LocID == locationid).LocName;
            // ViewBag.Location = new SelectList(db.Locations.OrderBy(s => s.LocName), "LocID", "LocName");
            ViewBag.Location = new SelectList(db.Locations.OrderByDescending(aa => aa.LocName == Ulocname).ToList(), "LocID", "LocName");
            try
            {
                if (Session["LogedUserID"] != null)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return View("Index");
            }
        }
        public ActionResult Logout()
        {
            try
            {
                int _userID = Convert.ToInt32(Session["LogedUserID"].ToString());
                HelperClass.MyHelper.SaveAuditLog(_userID, (byte)MyEnums.FormName.LogIn, (byte)MyEnums.Operation.LogOut, DateTime.Now);
                Session["LogedUserID"] = "";
                Session["LogedUserFullname"] = null;
                Session["LogedUserRegion"] = null;
                Session["LoggedUser"] = null;
                Session["MHR"] = null;
                Session["MDevice"] = null;
                Session["MLeave"] = null;
                Session["MEditAtt"] = null;
                Session["MUser"] = null;
                Session["MRDailyAtt"] = null;
                Session["MRLeave"] = null;
                Session["MRMonthly"] = null;
                Session["MRAudit"] = null;
                Session["CompanyName"] = null;
                Session["MRManualEditAtt"] = null;
                Session["MREmployee"] = null;
                Session["MRDetail"] = null;
                Session["MRSummary"] = null;
                Session["FiltersModel"] = new WMSLibrary.FiltersModel();
                Session["LoginCount"] = null;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Index");
            }
        }
        private string SerializeObject(object myObject)
        {
            var stream = new MemoryStream();
            var xmldoc = new XmlDocument();
            var serializer = new XmlSerializer(myObject.GetType());
            using (stream)
            {
                serializer.Serialize(stream, myObject);
                stream.Seek(0, SeekOrigin.Begin);
                xmldoc.Load(stream);
            }

            return xmldoc.InnerXml;
        }

        private object DeSerializeObject(object myObject, Type objectType)
        {
            var xmlSerial = new XmlSerializer(objectType);
            var xmlStream = new StringReader(myObject.ToString());
            return xmlSerial.Deserialize(xmlStream);
        }
        #region --Dashboard--
        [HttpPost]
        public ActionResult GetDahboard(int? Locationid, DateTime? Date)
        {
            TAS2013Entities db = new TAS2013Entities();
            User LoggedInUser = Session["LoggedUser"] as User;
            DashboardValues dv = new DashboardValues();
            List<Emp> emplist = new List<Emp>();
            if (Date == null)
                Date = DateTime.Today;
            //if (DateEnd == null)
            //    DateEnd = DateTime.Today;
            List<ViewAttData> ViewAttDataList = new List<ViewAttData>();
            List<ViewAttData> ViewAttDataListGH = new List<ViewAttData>();
            List<JobCardDetail> jcEmp = new List<JobCardDetail>();
            jcEmp = db.JobCardDetails.Where(aa => aa.Dated == Date).ToList();
            DateTime dts = new DateTime(Date.Value.Year, Date.Value.Month, 1);
            DateTime dte = new DateTime(Date.Value.Year, Date.Value.Month, DateTime.DaysInMonth(Date.Value.Year, Date.Value.Month));

            if (Locationid == 0)
            {
                emplist = db.Emps.Where(aa => aa.Status == true).ToList();
                ViewAttDataListGH = db.ViewAttDatas.Where(aa => aa.AttDate >= dts && aa.AttDate <= dte).ToList();
                ViewAttDataList = ViewAttDataListGH.Where(aa => aa.AttDate == Date && aa.DesigID != 24).ToList();
            }
            else
            {
                emplist = db.Emps.Where(aa => aa.LocID == Locationid && aa.Status == true).ToList();
                ViewAttDataListGH = db.ViewAttDatas.Where(aa => aa.LocID == Locationid && aa.AttDate >= dts && aa.AttDate <= dte).ToList();
                ViewAttDataList = ViewAttDataListGH.Where(aa => aa.LocID == Locationid && aa.AttDate == Date && aa.DesigID != 24).ToList();
                jcEmp = db.JobCardDetails.Where(aa => aa.Dated == Date).ToList();
            }
            dv.DateStart = (DateTime)Date;
            dv.LocationName = db.Locations.First(aa => aa.LocID == Locationid).LocName;
            //dv.DateEnd = (DateTime)DateEnd;
            dv.TotalEmps = ViewAttDataList.Where(aa => aa.DesigID != 24).Select(aa => aa.EmpID).Distinct().Count();
            dv.Present = ViewAttDataList.Where(aa => aa.StatusP == true).Count();
            dv.Absent = ViewAttDataList.Where(aa => aa.StatusAB == true).Count();
            dv.Leaves = ViewAttDataList.Where(aa => aa.StatusLeave == true).Count();
            dv.TotalEmpAllLoc = db.Emps.Where(aa => aa.Status == true).Count();
            dv.TotalPresent = db.AttDatas.Where(aa => aa.StatusP == true && aa.AttDate == Date).Count();
            dv.TotalAbsent = db.ViewAttDatas.Where(aa => aa.StatusAB == true && aa.AttDate == Date).Count();
            dv.TotalLeaves = db.AttDatas.Where(aa => aa.StatusLeave == true && aa.AttDate == Date).Count();
            dv.TotalVisitors = db.ViewAttDatas.Where(aa => aa.AttDate == Date && aa.TimeIn != null).Count();
            dv.JCTraining = jcEmp.Where(aa => aa.WrkCardID == 7).ToList().Count;
            dv.JCFieldTour = jcEmp.Where(aa => aa.WrkCardID == 5).ToList().Count;
            dv.JCOfficialDuty = jcEmp.Where(aa => aa.WrkCardID == 1).ToList().Count;
            dv.DashboardGraphObj = GetDahboardGraph(dts, dte, ViewAttDataListGH);
            if (HttpContext.Request.IsAjaxRequest())
                return View("DashboardContainer", dv);

            return RedirectToAction("Index");
        }

        private List<DashboardGraph> GetDahboardGraph(DateTime? dts, DateTime? dte, List<ViewAttData> ViewAttDataListGH)
        {
            List<DashboardGraph> vmList = new List<DashboardGraph>();
            while (dts < dte)
            {
                if (ViewAttDataListGH.Where(aa => aa.AttDate >= dts && aa.AttDate <= dte && aa.StatusP == true).Count() > 0)
                {

                    DashboardGraph obj = new DashboardGraph();
                    obj.XAxis = dts.Value.ToString("dd-MMM-yyyy");
                    obj.YAxis = ViewAttDataListGH.Where(aa => aa.AttDate == dts && aa.StatusP == true).Count();
                    vmList.Add(obj);
                }
                dts = dts.Value.AddDays(1);
            }
            return vmList;
        }

        #endregion
    }
    public class DashboardValues
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int TotalEmps { get; set; }
        public string LocationName { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Leaves { get; set; }
        public int JCFieldTour { get; set; }
        public int JCTraining { get; set; }
        public int JCOfficialDuty { get; set; }
        public int TotalEmpAllLoc { get; set; }
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalLeaves { get; set; }
        public int TotalVisitors { get; set; }
        public List<DashboardGraph> DashboardGraphObj { get; set; }
    }

    public class DashboardGraph
    {
        public string XAxis { get; set; }
        public int YAxis { get; set; }
    }
}