﻿using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WMS.CustomClass;
using WMS.HelperClass;
using WMS.Models;
using WMSLibrary;

namespace WMS.Reports
{
    public partial class ReportContainer : System.Web.UI.Page
    {
        String title = "";
        string _dateFrom = "";
        List<Option> companyimage = new List<Option>();
        protected void Page_Load(object sender, EventArgs e)
        {
            String reportName = Request.QueryString["reportname"];
            String type = Request.QueryString["type"];
            if (!Page.IsPostBack)
            {
                List<string> list = Session["ReportSession"] as List<string>;
                FiltersModel fm = Session["FiltersModel"] as FiltersModel;
                CreateDataTable();
                User LoggedInUser = HttpContext.Current.Session["LoggedUser"] as User;
                TAS2013Entities db = new TAS2013Entities();
                QueryBuilder qb = new QueryBuilder();
                string query = qb.MakeCustomizeQueryForEmp(LoggedInUser);
                _dateFrom = list[0];
                string _dateTo = list[1];
                CreateEmpSummaryDataTable();
                companyimage = GetCompanyImages(fm);
                string PathString = "";
                string consolidatedMonth = "";
                switch (reportName)
                {
                    #region --Employee record --
                    case "emp_record": DataTable dt = qb.GetValuesfromDB("select * from EmpView " + query);
                        List<EmpView> _ViewList = dt.ToList<EmpView>();
                        List<EmpView> _TempViewList = new List<EmpView>();
                        title = "Employee Record Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/Employee.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/Employee.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList, _ViewList), _dateFrom + " TO " + _dateTo);
                        break;
                    case "emp_record_active": dt = qb.GetValuesfromDB("select * from EmpView where Status=1");
                        _ViewList = dt.ToList<EmpView>();
                        _TempViewList = new List<EmpView>();
                        title = "Active Employees Record Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/Employee.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/Employee.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList, _ViewList), _dateFrom + " TO " + _dateTo);

                        break;
                    case "emp_record_inactive": dt = qb.GetValuesfromDB("select * from EmpView where Status=0");
                        _ViewList = dt.ToList<EmpView>();
                        _TempViewList = new List<EmpView>();
                        title = "Inactive Employees Record Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/Employee.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/Employee.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList, _ViewList), _dateFrom + " TO " + _dateTo);
                        break;
                    case "emp_detail_excel": DataTable dt1 = qb.GetValuesfromDB("select * from EmpView " + query);
                        List<EmpView> _ViewList1 = dt1.ToList<EmpView>();
                        List<EmpView> _TempViewList1 = new List<EmpView>();
                        title = "Employee Detail Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/EmployeeDetail.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/EmployeeDetail.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList1, _ViewList1), _dateFrom + " TO " + _dateTo);

                        break;
                    #endregion
                    case "leave_application": dt1 = qb.GetValuesfromDB("select * from ViewLvApplication " + query + " and (FromDate >= '" + _dateFrom + "' and ToDate <= '" + _dateTo + "' )");
                        List<ViewLvApplication> _ViewListLvApp = dt1.ToList<ViewLvApplication>();
                        List<ViewLvApplication> _TempViewListLvApp = new List<ViewLvApplication>();
                        title = "Leave Application Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRLeave.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRLeave.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewListLvApp, _ViewListLvApp), _dateFrom + " TO " + _dateTo);

                        break;
                    #region -- Daily Reports--
                    case "detailed_att": DataTable dt2 = qb.GetValuesfromDB("select * from ViewDetailAttData  where(AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )");
                        List<ViewDetailAttData> _ViewList2 = dt2.ToList<ViewDetailAttData>();
                        List<ViewDetailAttData> _TempViewList2 = new List<ViewDetailAttData>();
                        _ViewList2 = AssistantQuery.GetReportsEmpsDetails(_ViewList2, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());
                        title = "Detailed Attendence";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRdetailed.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRdetailed.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList2, _ViewList2), _dateFrom + " TO " + _dateTo);

                        break;
                    case "present": DataTable datatable = qb.GetValuesfromDB("select * from ViewAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )" + " and StatusP = 1 ");
                        List<ViewAttData> _ViewList4 = datatable.ToList<ViewAttData>();
                        List<ViewAttData> _TempViewList4 = new List<ViewAttData>();
                        _ViewList4 = AssistantQuery.GetReportsEmps(_ViewList4, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());
                        title = "Present Employee Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRPresent.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRPresent.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList4, _ViewList4), _dateFrom + " TO " + _dateTo);

                        break;
                    case "absent": DataTable dt5 = qb.GetValuesfromDB("select * from ViewAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )" + " and StatusAB = 1 ");
                        List<ViewAttData> _ViewList5 = dt5.ToList<ViewAttData>();
                        List<ViewAttData> _TempViewList5 = new List<ViewAttData>();
                        _ViewList5 = AssistantQuery.GetReportsEmps(_ViewList5, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());

                        title = "Absent Employee Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRAbsent.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRAbsent.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList5, _ViewList5), _dateFrom + " TO " + _dateTo);

                        break;
                    case "lv_application": DataTable dt6 = qb.GetValuesfromDB("select * from ViewAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                + _dateTo + "'" + " )" + " and (StatusLeave=1 OR StatusHL=1)");
                        List<ViewAttData> _ViewList6 = dt6.ToList<ViewAttData>();
                        List<ViewAttData> _TempViewList6 = new List<ViewAttData>();
                        _ViewList6 = AssistantQuery.GetReportsEmps(_ViewList6, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());
                        title = "Leave Attendence Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRAbsent.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRAbsent.rdlc";

                        //LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList6, _ViewList6), _dateFrom + " TO " + _dateTo);
                        //To-do Develop Leave Attendance Report
                        break;
                    case "short_lv": DataTable dt7 = qb.GetValuesfromDB("select * from ViewAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )" + " and StatusSL=1");
                        List<ViewAttData> _ViewList7 = dt7.ToList<ViewAttData>();
                        List<ViewAttData> _TempViewList7 = new List<ViewAttData>();
                        _ViewList7 = AssistantQuery.GetReportsEmps(_ViewList7, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());
                        title = "Short Leave Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRShortLeave.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRShortLeave.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList7, _ViewList7), _dateFrom + " TO " + _dateTo);

                        break;
                    case "late_in": DataTable dt8 = qb.GetValuesfromDB("select * from ViewAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                    + _dateTo + "'" + " )" + " and StatusLI=1 ");
                        List<ViewAttData> _ViewList8 = dt8.ToList<ViewAttData>();
                        List<ViewAttData> _TempViewList8 = new List<ViewAttData>();
                        _ViewList8 = AssistantQuery.GetReportsEmps(_ViewList8, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());
                        title = "Late In Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRLateIn.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRLateIn.rdlc";

                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;

                    case "late_out": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                            + _dateTo + "'" + " )" + " and StatusLO=1 ");
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        _ViewList8 = AssistantQuery.GetReportsEmps(_ViewList8, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());
                        title = "Late Out Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRLateOut.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRLateOut.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;

                    case "early_in": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                           + _dateTo + "'" + " )" + " and StatusEI=1 ");
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        _ViewList8 = AssistantQuery.GetReportsEmps(_ViewList8, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());
                       
                        title = "Early In Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DREarlyIn.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DREarlyIn.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;
                    case "early_out": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                             + _dateTo + "'" + " )" + " and StatusEO=1 ");
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        _ViewList8 = AssistantQuery.GetReportsEmps(_ViewList8, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());
                       
                        title = "Early Out Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DREarlyOut.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DREarlyOut.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;
                    case "overtime": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                         + _dateTo + "'" + " )" + " and StatusOT=1 ");
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        _ViewList8 = AssistantQuery.GetReportsEmps(_ViewList8, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());
                       
                        title = "OverTime Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DROverTime.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DROverTime.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;
                    case "missing_attendance": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                    + _dateTo + "'" + " )" + " and ((TimeIn is null and TimeOut is not null) or (TimeIn is not null and TimeOut is null)) ");

                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        _ViewList8 = AssistantQuery.GetReportsEmps(_ViewList8, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());
                       
                        title = "Missing Attendence Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRMissingAtt.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRMissingAtt.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom + " TO " + _dateTo);

                        break;
                    case "multiple_in_out": dt = qb.GetValuesfromDB("select * from ViewMultipleInOut " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'"
                                                     + _dateTo + "'" + " )" + " and (Tin1 is not null or TOut1 is not null)");
                        //change query for multiple_in_out
                        List<ViewMultipleInOut> _ViewList9 = dt.ToList<ViewMultipleInOut>();
                        List<ViewMultipleInOut> _TempViewList9 = new List<ViewMultipleInOut>();
                        title = "Multiple In/Out Report";
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/DRMultipleInOut.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/DRMultipleInOut.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList9, _ViewList9), _dateFrom + " TO " + _dateTo);

                        break;

                    #endregion
                    case "monthly_21-20_flexy":
                        dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1 ");
                        _ViewList = dt.ToList<EmpView>();
                        _TempViewList = new List<EmpView>();
                        _ViewList = AssistantQuery.GetFilteredEmps(_ViewList, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());
                       
                        title = "Employee Record Report";

                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/MRFlexyDetailExcelP.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/MRFlexyDetailExcelP.rdlc";
                        LoadReport(LoadPermanentMonthlyDT(ReportsFilterImplementation(fm, _TempViewList, _ViewList), Convert.ToDateTime(_dateFrom), Convert.ToDateTime(_dateTo)), PathString, _dateFrom + " TO " + _dateTo);
                        break;
                    case "emp_att": dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " and Status=1)");
                        title = "Employee Attendance";
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        _ViewList8 = AssistantQuery.GetReportsEmps(_ViewList8, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/EmpAttSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/EmpAttSummary.rdlc";
                        LoadReport(PathString, ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), Convert.ToDateTime(_dateFrom).ToString("dd-MMM-yyyy") + " TO " + Convert.ToDateTime(_dateTo).ToString("dd-MMM-yyyy"));
                        break;
                    case "emp_summary": dt = qb.GetValuesfromDB("select * from EmpView " + query + " and Status=1");
                        _ViewList = dt.ToList<EmpView>();
                        _TempViewList = new List<EmpView>();
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/EmpSummary.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/EmpSummary.rdlc";
                        //LoadReport(PathString, CalculateEmpSummary(ReportsFilterImplementation(fm, _TempViewList8, _ViewList8), _dateFrom, _dateTo), _dateFrom + " TO " + _dateTo);
                        LoadReport(PathString, CalculateEmpSummary(ReportsFilterImplementation(fm, _TempViewList, _ViewList), Convert.ToDateTime(_dateFrom), Convert.ToDateTime(_dateTo)), Convert.ToDateTime(_dateFrom), Convert.ToDateTime(_dateTo));
                        break;


                    #region-------Graphicaly Reports-----------
                    //Entry Time Graph Report
                    case "gtimein":
                        dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " and Status=1)");
                        title = "Attendance graph for Entry Time";
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        _ViewList8 = AssistantQuery.GetReportsEmps(_ViewList8, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());
                        List<VMTimeAverage> vmlist = AddTimesInVMModal();
                        _ViewList8 = ReportsFilterImplementation(fm, _TempViewList8, _ViewList8); 
                        foreach (var item in vmlist)
                        {
                            TimeSpan TM = vmlist[0].TimeStart;
                            item.Totals = _ViewList8.Where(aa => aa.TimeIn != null && aa.TimeIn.Value.TimeOfDay >= TM && aa.TimeIn.Value.TimeOfDay <= item.TimeEnd).Count().ToString();
                            item.Actuals = _ViewList8.Where(aa => aa.TimeIn != null && aa.TimeIn.Value.TimeOfDay >= item.TimeStart && aa.TimeIn.Value.TimeOfDay <= item.TimeEnd).Count().ToString();
                        }
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/GTimeINOUT.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/GTimeINOUT.rdlc";
                        LoadReport(PathString, vmlist, _dateFrom + " TO " + _dateTo, title);
                        break;
                    //Exit Time Graph Report
                    case "gtimeout":
                        dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " and Status=1)");
                        title = "Attendance graph for Exit Time";
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        _ViewList8 = AssistantQuery.GetReportsEmps(_ViewList8, db.UserSections.Where(aa => aa.UserID == LoggedInUser.UserID).ToList());
                        List<VMTimeAverage> vmlistout = AddTimesOutVMModal();
                        _ViewList8 = ReportsFilterImplementation(fm, _TempViewList8, _ViewList8);
                        foreach (var item in vmlistout)
                        {
                            TimeSpan TM = vmlistout[0].TimeStart;
                            item.Totals = _ViewList8.Where(aa => aa.TimeOut != null && aa.TimeOut.Value.TimeOfDay >= TM && aa.TimeOut.Value.TimeOfDay <= item.TimeEnd).Count().ToString();
                            item.Actuals = _ViewList8.Where(aa => aa.TimeOut != null && aa.TimeOut.Value.TimeOfDay >= item.TimeStart && aa.TimeOut.Value.TimeOfDay <= item.TimeEnd).Count().ToString();
                        }
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/GTimeINOUT.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/GTimeINOUT.rdlc";
                        LoadReport(PathString, vmlistout, _dateFrom + " TO " + _dateTo, title);
                        break;
                    //Entry Time Graph Report Department Wise
                    case "gtimeOutDeptwise":
                        dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " and Status=1)");
                        title = "Section Wise Attendance graph for Exit Time";
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        List<VMTimeAverage> vmlistoutDept = AddTimesOutVMModal();
                        _ViewList8 = ReportsFilterImplementation(fm, _TempViewList8, _ViewList8);
                        List<VMTimeAverageDeptWise> vmList = new List<VMTimeAverageDeptWise>();
                        foreach (var item in vmlistoutDept)
                        {
                            foreach (var dept in _ViewList8.Select(aa => new { DeptID = aa.SecID, DeptName = aa.SectionName }).Distinct())
                            {
                                VMTimeAverageDeptWise vm = new VMTimeAverageDeptWise();
                                vm.Time = item.Time;
                                vm.TimeEnd = item.TimeEnd;
                                vm.TimeStart = item.TimeStart;
                                vm.DeptName = dept.DeptName;
                                TimeSpan TM = vmlistoutDept[0].TimeStart;
                                vm.DeptTotals = _ViewList8.Where(aa => aa.SecID == dept.DeptID && aa.TimeOut != null && aa.TimeOut.Value.TimeOfDay >= TM && aa.TimeOut.Value.TimeOfDay <= item.TimeEnd).Count().ToString();
                                vm.DeptActuals = _ViewList8.Where(aa => aa.SecID == dept.DeptID && aa.TimeOut != null && aa.TimeOut.Value.TimeOfDay >= item.TimeStart && aa.TimeOut.Value.TimeOfDay <= item.TimeEnd).Count().ToString();
                                vmList.Add(vm);
                            }
                        }
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/GTimeINOUTDept.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/GTimeINOUTDept.rdlc";
                        LoadReport(PathString, vmList, _dateFrom + " TO " + _dateTo, title);
                        break;


                    case "gtimeInDeptwise":
                        dt = qb.GetValuesfromDB("select * from ViewAttData " + query + " and (AttDate >= " + "'" + _dateFrom + "'" + " and AttDate <= " + "'" + _dateTo + "'" + " and Status=1)");
                        title = "Section Wise Attendance graph for Entry Time";
                        _ViewList8 = dt.ToList<ViewAttData>();
                        _TempViewList8 = new List<ViewAttData>();
                        List<VMTimeAverage> vmlistInDept = AddTimesInVMModal();
                        _ViewList8 = ReportsFilterImplementation(fm, _TempViewList8, _ViewList8);
                        List<VMTimeAverageDeptWise> vmListIn = new List<VMTimeAverageDeptWise>();
                        foreach (var item in vmlistInDept)
                        {
                            foreach (var dept in _ViewList8.Select(aa => new { DeptID = aa.SecID, DeptName = aa.SectionName }).Distinct())
                            {
                                VMTimeAverageDeptWise vm = new VMTimeAverageDeptWise();
                                vm.Time = item.Time;
                                vm.TimeEnd = item.TimeEnd;
                                vm.TimeStart = item.TimeStart;
                                vm.DeptName = dept.DeptName;
                                TimeSpan TM = vmlistInDept[0].TimeStart;
                                vm.DeptTotals = _ViewList8.Where(aa => aa.SecID == dept.DeptID && aa.TimeIn != null && aa.TimeIn.Value.TimeOfDay >= TM && aa.TimeIn.Value.TimeOfDay <= item.TimeEnd).Count().ToString();
                                vm.DeptActuals = _ViewList8.Where(aa => aa.SecID == dept.DeptID && aa.TimeIn != null && aa.TimeIn.Value.TimeOfDay >= item.TimeStart && aa.TimeIn.Value.TimeOfDay <= item.TimeEnd).Count().ToString();
                                vmListIn.Add(vm);
                            }
                        }
                        //Change the Paths
                        if (GlobalVariables.DeploymentType == false)
                            PathString = "/Reports/RDLC/GTimeINOUTDept.rdlc";
                        else
                            PathString = "/WMS/Reports/RDLC/GTimeINOUTDept.rdlc";
                        LoadReport(PathString, vmListIn, _dateFrom + " TO " + _dateTo, title);
                        break;
                    #endregion

                }
            }
        }

        private void LoadReport(string PathString, List<VMTimeAverageDeptWise> vmList, string date, string title)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<VMTimeAverageDeptWise> ie;
            ie = vmList.AsQueryable();
            IEnumerable<Option> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Title", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
        }

        private List<VMTimeAverage> AddTimesOutVMModal()
        {
            List<VMTimeAverage> vmt = new List<VMTimeAverage>();
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(16, 0, 0);
                vm.TimeEnd = new TimeSpan(16, 15, 0);
                vm.Time = "04:15";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(16, 15, 0);
                vm.TimeEnd = new TimeSpan(16, 30, 0);
                vm.Time = "04:30";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(16, 30, 0);
                vm.TimeEnd = new TimeSpan(16, 45, 0);
                vm.Time = "04:45";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(16, 45, 0);
                vm.TimeEnd = new TimeSpan(17, 0, 0);
                vm.Time = "05:00";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(17, 00, 0);
                vm.TimeEnd = new TimeSpan(17, 15, 0);
                vm.Time = "05:15";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(17, 15, 0);
                vm.TimeEnd = new TimeSpan(17, 30, 0);
                vm.Time = "05:30";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(17, 30, 0);
                vm.TimeEnd = new TimeSpan(17, 45, 0);
                vm.Time = "05:45";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(17, 45, 0);
                vm.TimeEnd = new TimeSpan(18, 0, 0);
                vm.Time = "06:00";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(18, 00, 0);
                vm.TimeEnd = new TimeSpan(18, 15, 0);
                vm.Time = "06:15";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(18, 15, 0);
                vm.TimeEnd = new TimeSpan(18, 30, 0);
                vm.Time = "06:30";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(18, 30, 0);
                vm.TimeEnd = new TimeSpan(18, 45, 0);
                vm.Time = "06:45";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(18, 45, 0);
                vm.TimeEnd = new TimeSpan(19, 0, 0);
                vm.Time = "07:00";
                vmt.Add(vm);
            }
            return vmt;
        }

        private void LoadReport(string PathString, List<VMTimeAverage> vmlist, string date, string Header)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<VMTimeAverage> ie;
            ie = vmlist.AsQueryable();
            IEnumerable<Option> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Title", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private List<VMTimeAverage> AddTimesInVMModal()
        {
            List<VMTimeAverage> vmt = new List<VMTimeAverage>();
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(07, 0, 0);
                vm.TimeEnd = new TimeSpan(07, 15, 0);
                vm.Time = "07:15";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(07, 15, 0);
                vm.TimeEnd = new TimeSpan(07, 30, 0);
                vm.Time = "07:30";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(07, 30, 0);
                vm.TimeEnd = new TimeSpan(07, 45, 0);
                vm.Time = "07:45";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(07, 45, 0);
                vm.TimeEnd = new TimeSpan(08, 0, 0);
                vm.Time = "08:00";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(08, 00, 0);
                vm.TimeEnd = new TimeSpan(08, 15, 0);
                vm.Time = "08:15";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(08, 15, 0);
                vm.TimeEnd = new TimeSpan(08, 30, 0);
                vm.Time = "08:30";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(08, 30, 0);
                vm.TimeEnd = new TimeSpan(08, 45, 0);
                vm.Time = "08:45";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(08, 45, 0);
                vm.TimeEnd = new TimeSpan(09, 0, 0);
                vm.Time = "09:00";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(09, 00, 0);
                vm.TimeEnd = new TimeSpan(09, 15, 0);
                vm.Time = "09:15";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(09, 15, 0);
                vm.TimeEnd = new TimeSpan(09, 30, 0);
                vm.Time = "09:30";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(09, 30, 0);
                vm.TimeEnd = new TimeSpan(09, 45, 0);
                vm.Time = "09:45";
                vmt.Add(vm);
            }
            {
                VMTimeAverage vm = new VMTimeAverage();
                vm.TimeStart = new TimeSpan(09, 45, 0);
                vm.TimeEnd = new TimeSpan(10, 0, 0);
                vm.Time = "10:00";
                vmt.Add(vm);
            }
            return vmt;
        }
        private void CreateEmpSummaryDataTable()
        {
            EmpSummaryDT.Columns.Add("EmpNo", typeof(string));
            EmpSummaryDT.Columns.Add("EmpName", typeof(string));
            EmpSummaryDT.Columns.Add("Unit", typeof(string));
            EmpSummaryDT.Columns.Add("Group", typeof(string));
            EmpSummaryDT.Columns.Add("DateStart", typeof(DateTime));
            EmpSummaryDT.Columns.Add("DateEnd", typeof(DateTime));
            EmpSummaryDT.Columns.Add("AvgTimeIn", typeof(string));
            EmpSummaryDT.Columns.Add("AvgTimeOut", typeof(string));
            EmpSummaryDT.Columns.Add("AvgWorkSpend", typeof(string));
            EmpSummaryDT.Columns.Add("TotalWorkDays", typeof(string));
            EmpSummaryDT.Columns.Add("TotalPresent", typeof(string));
            EmpSummaryDT.Columns.Add("TotalAbsent", typeof(string));
            EmpSummaryDT.Columns.Add("TotalLateIn", typeof(string));
            EmpSummaryDT.Columns.Add("LateInPercent", typeof(string));
            EmpSummaryDT.Columns.Add("EmpID", typeof(int));
            EmpSummaryDT.Columns.Add("Presentpercent", typeof(string));
            EmpSummaryDT.Columns.Add("AbsentPercent", typeof(string));
            EmpSummaryDT.Columns.Add("EarlyOutCount", typeof(string));
            EmpSummaryDT.Columns.Add("EarlyOutPercent", typeof(string));
            EmpSummaryDT.Columns.Add("LeaveCount", typeof(string));
            EmpSummaryDT.Columns.Add("LeavePercent", typeof(string));

        }

        private void LoadReport(string PathString, DataTable dataTable, DateTime dts, DateTime dte)
        {
            string _Header = "Employees Summary Report";
            this.ReportViewer1.LocalReport.DisplayName = "Employee Summary Report";
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            string Date = dts.Date.ToString("dd-MMM-yyy") + " TO " + dte.Date.ToString("dd-MMM-yyy");
            string WorkDays = CalculateWorkDays(dts, dte, dataTable);
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", dataTable);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.HyperlinkTarget = "_blank";
            IEnumerable<Option> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            ReportParameter rp1 = new ReportParameter("Date", Date, false);
            ReportParameter rp2 = new ReportParameter("WorkDays", "Working Days: " + WorkDays, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1, rp2 });
            ReportViewer1.LocalReport.Refresh();
        }


        private string CalculateWorkDays(DateTime dts, DateTime dte, DataTable dataTable)
        {
            TAS2013Entities db = new TAS2013Entities();
            string workDays = "0";
            int largeWorkDays = 0;
            int totalDays = 0;
            while (dts <= dte)
            {
                totalDays = totalDays + 1;
                if (db.Holidays.Where(hol => hol.HolDate.Month == dts.Month && hol.HolDate.Day == dts.Day).Count() > 0)
                {
                    largeWorkDays = largeWorkDays + 1;
                }
                else
                {
                    if (dts.DayOfWeek == DayOfWeek.Sunday || dts.DayOfWeek == DayOfWeek.Saturday)
                    {
                        largeWorkDays = largeWorkDays + 1;
                    }
                }

                dts = dts.AddDays(1);
            }
            return (totalDays - largeWorkDays).ToString("00"); ;
        }
        public void AddValuesInEmpSummaryDT(string EmpNo, string EmpName, string unit, string Group, DateTime DateStart, DateTime DateEnd, string AvgTimeIn,
            string AvgTimeOut, string AvgWorkSpend, int TotalWD, string TotalP, string TotalA, string TotalLI, string LIPercent, int EmpID, string PresentPercent, string AbsentPercent, string EarlyOutCount, string EarlyOutPrecent, string LeaveCount, string LeavePercent)
        {
            EmpSummaryDT.Rows.Add(EmpNo, EmpName, unit, Group, DateStart, DateEnd, AvgTimeIn, AvgTimeOut, AvgWorkSpend, TotalWD, TotalP, TotalA, TotalLI, LIPercent, EmpID, PresentPercent, AbsentPercent, EarlyOutCount, EarlyOutPrecent, LeaveCount, LeavePercent);
        }
        DataTable EmpSummaryDT = new DataTable();
        private DataTable CalculateEmpSummary(List<EmpView> emps, DateTime dateFrom, DateTime dateTo)
        {
            using (var db = new TAS2013Entities())
            {
                List<AttData> attDatas = new List<AttData>();
                attDatas = db.AttDatas.Where(aa => aa.AttDate >= dateFrom && aa.AttDate <= dateTo).ToList();
                foreach (var emp in emps)
                {
                    string AvgTimeIn = "";
                    string AvgTimeOut = "";
                    int TimeInMins = 0;
                    int TimeOutMins = 0;
                    int TimeInCount = 0;
                    int TimeOutCount = 0;
                    string AvgWorkSpend = "";
                    int workMins = 0;
                    float TotalPresent = 0;
                    string PresentPercent = "";
                    int TotalWorkDays = 0;
                    float TotalAbsent = 0;
                    string AbsentPercent = "";
                    int TotalLateIn = 0;
                    int TotalWorkMins = 0;
                    int TotalWorkCount = 0;
                    string LateInPercent = "";
                    int EarlyOutCount = 0;
                    string EarlyOutPrecent = "";
                    float LeaveCount = 0;
                    string LeavePrecent = "";
                    List<AttData> attdata = attDatas.Where(aa => aa.EmpID == emp.EmpID).ToList();
                    foreach (var ad in attdata)
                    {
                        if (ad.DutyCode != "R" && ad.DutyCode != "G")
                        {
                            TotalWorkDays = TotalWorkDays + 1;
                            if (ad.PDays != null)
                                TotalPresent = (float)(ad.PDays + TotalPresent);
                            if (ad.ABDays != null)
                                TotalAbsent = (float)(ad.ABDays + TotalAbsent);
                            if (ad.LeaveDays != null)
                                LeaveCount = (float)(ad.LeaveDays + LeaveCount);
                            //if (ad.StatusAB != true)
                            //{
                            //    if (ad.StatusHL == true)
                            //    {
                            //        LeaveCount = (float)(LeaveCount + 0.5);
                            //        if(ad.WorkMin>10)
                            //            TotalPresent = (float)(TotalPresent + 0.5);
                            //        else
                            //        TotalAbsent = (float)(TotalAbsent + 0.5);
                            //    }
                            //    if ((ad.WorkMin >0 && ad.StatusHL != true)|| ad.DutyCode=="O" || ad.StatusP==true)
                            //    {
                            //        TotalPresent = TotalPresent + 1;
                            //    }
                            //}
                            //else
                            //{
                            //    if (ad.StatusHL == true)
                            //        TotalAbsent = (float)(TotalAbsent + 0.5);
                            //    else
                            //        TotalAbsent = TotalAbsent + 1;
                            //}
                            if (ad.StatusLI == true)
                                TotalLateIn = TotalLateIn + 1;
                            if (ad.StatusEO == true)
                                EarlyOutCount = EarlyOutCount + 1;
                            if (ad.WorkMin > 0)
                                workMins = (int)(workMins + ad.WorkMin);
                            if (ad.TimeIn != null)
                            {
                                TimeInCount = TimeInCount + 1;
                                TimeInMins = (int)(TimeInMins + ad.TimeIn.Value.TimeOfDay.TotalMinutes);
                            }
                            else
                            {
                                if (ad.StatusLI == true)
                                {
                                    TotalLateIn = TotalLateIn + 1;
                                }
                            }
                            if (ad.TimeOut != null)
                            {
                                TimeOutCount = TimeOutCount + 1;
                                TimeOutMins = (int)(TimeOutMins + ad.TimeOut.Value.TimeOfDay.TotalMinutes);
                            }
                            if (ad.WorkMin > 0)
                            {
                                TotalWorkCount = TotalWorkCount + 1;
                                TotalWorkMins = (int)(TotalWorkMins + ad.WorkMin);
                            }
                            //if (ad.StatusLeave ==true)
                            //{
                            //    if (ad.StatusHL == true)
                            //        LeaveCount = (float)(LeaveCount + 0.5);
                            //    else
                            //        LeaveCount = LeaveCount + 1;
                            //}
                        }
                    }
                    if (TotalPresent > 0)
                    {
                        LateInPercent = ((TotalLateIn * 100) / TotalPresent).ToString("00") + "%";
                        EarlyOutPrecent = ((EarlyOutCount * 100) / TotalPresent).ToString("00") + "%";
                        LeavePrecent = ((LeaveCount * 100) / TotalWorkDays).ToString("00") + "%";
                    }
                    TimeSpan AvgTIN = new TimeSpan();
                    TimeSpan AvgTOut = new TimeSpan();
                    if (TimeInCount > 0)
                    {
                        int min = TimeInMins / TimeInCount;
                        AvgTIN = new TimeSpan(0, min, 0);
                        AvgTimeIn = AvgTIN.Hours.ToString("00") + ":" + AvgTIN.Minutes.ToString("00");
                    }
                    if (TimeOutCount > 0)
                    {
                        int min = TimeOutMins / TimeOutCount;
                        AvgTOut = new TimeSpan(0, min, 0);
                        AvgTimeOut = AvgTOut.Hours.ToString("00") + ":" + AvgTOut.Minutes.ToString("00");
                    }
                    if (TotalWorkCount > 0)
                    {
                        int min = TotalWorkMins / TotalWorkCount;
                        TimeSpan tt = new TimeSpan(0, min, 0);
                        AvgWorkSpend = tt.Hours.ToString("00") + ":" + tt.Minutes.ToString("00");
                    }
                    if (TotalAbsent > 0)
                    {
                        AbsentPercent = ((TotalAbsent * 100) / TotalWorkDays).ToString("00") + "%";
                    }
                    if (TotalPresent > 0)
                    {
                        PresentPercent = ((TotalPresent * 100) / TotalWorkDays).ToString("00") + "%";
                    }
                    TimeSpan avw = AvgTOut - AvgTIN;
                    AvgWorkSpend = avw.Hours.ToString("00") + ":" + avw.Minutes.ToString("00");
                    AddValuesInEmpSummaryDT(emp.EmpNo, emp.EmpName, emp.SectionName, emp.DeptName, dateFrom, dateTo, AvgTimeIn, AvgTimeOut, AvgWorkSpend,
                        TotalWorkDays, TotalPresent.ToString(), TotalAbsent.ToString(), TotalLateIn.ToString(), LateInPercent, emp.EmpID, PresentPercent, AbsentPercent, EarlyOutCount.ToString(), EarlyOutPrecent, LeaveCount.ToString(), LeavePrecent);
                }
            }
            return EmpSummaryDT;
        }
        #region -- AttSummaryReport--

        private void LoadReport(List<VMAttMnDataPer> dataTable, string PathString, string p)
        {
            string _Header = "Flexy Monthly Sheet";
            //string Date = Convert.ToDateTime(_dateFrom).Date.ToString("dd-MMM-yyyy");
            this.ReportViewer1.LocalReport.DisplayName = "Flexy Monthly Sheet";
            string Date = p;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", dataTable);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.HyperlinkTarget = "_blank";
            IEnumerable<Option> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            ReportParameter rp1 = new ReportParameter("Date", Date, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private List<VMAttMnDataPer> LoadPermanentMonthlyDT(List<EmpView> list, DateTime datefrom, DateTime dateto)
        {
            TAS2013Entities db = new TAS2013Entities();
            List<AttData> attData = new List<AttData>();
            List<AttData> _EmpAttData = new List<AttData>();
            List<VMAttMnDataPer> vMAttMnDataPers = new List<VMAttMnDataPer>();
            List<LvData> lvDatas = db.LvDatas.Where(aa => aa.AttDate >= datefrom && aa.AttDate <= dateto).ToList();
            string year = datefrom.Year.ToString();
            List<LvConsumed> lvConsumeds = db.LvConsumeds.Where(aa => aa.LvYear == year).ToList();
            attData = db.AttDatas.Where(aa => aa.AttDate >= datefrom && aa.AttDate <= dateto).ToList();
            foreach (EmpView emp in list)
            {
                try
                {
                    PermanentMonthly cmp = new PermanentMonthly();
                    _EmpAttData = attData.Where(aa => aa.EmpID == emp.EmpID).ToList();
                    VMAttMnDataPer vMAttMnDataPer = new VMAttMnDataPer();
                    vMAttMnDataPer = cmp.processPermanentMonthlyAttSingle((DateTime)datefrom, (DateTime)dateto, emp, _EmpAttData);
                    if (lvConsumeds.Where(aa => aa.EmpID == emp.EmpID && aa.LeaveTypeID == "B").Count() > 0)
                    {
                        vMAttMnDataPer.NewBalane = (float)lvConsumeds.First(aa => aa.EmpID == emp.EmpID && aa.LeaveTypeID == "B").GrandTotalRemaining;
                        List<LvData> tempLvDatas = lvDatas.Where(aa => aa.EmpID == emp.EmpID && aa.LvCode == "B").ToList();
                        vMAttMnDataPer.OldAnnualBalance = (float)lvConsumeds.First(aa => aa.EmpID == emp.EmpID && aa.LeaveTypeID == "B").GrandTotalRemaining + (tempLvDatas.Where(aa => aa.HalfLeave == true).Count() / 2) + tempLvDatas.Where(aa => aa.HalfLeave != true).Count();

                        vMAttMnDataPer.NewBalane = (float)db.LvConsumeds.First(aa => aa.EmpID == emp.EmpID && aa.LeaveTypeID == "B").GrandTotalRemaining;
                        vMAttMnDataPer.Credited = (float)emp.ALIncrement;
                        float halfLeavesCount = tempLvDatas.Where(aa => aa.HalfLeave == true).ToList().Count;
                        float HalfLeaves = (halfLeavesCount / 2);
                        float FullLeaves = tempLvDatas.Where(aa => aa.HalfLeave != true).Count();
                        vMAttMnDataPer.Taken = HalfLeaves + FullLeaves;
                    }
                    vMAttMnDataPer.SickLeave = lvDatas.Where(aa => aa.LvCode == "C" && aa.EmpID == emp.EmpID).Count();

                    vMAttMnDataPers.Add(vMAttMnDataPer);
                }
                catch (Exception ex)
                {

                }
            }
            return vMAttMnDataPers;
        }
        #endregion
        private List<DailySummary> ReportsFilterImplementation(FiltersModel fm, string dateFrom, string dateTo, string Criteria)
        {
            List<DailySummary> ViewDS = new List<DailySummary>();
            List<DailySummary> TempDS = new List<DailySummary>();
            QueryBuilder qb = new QueryBuilder();
            DataTable dt = new DataTable();
            switch (Criteria)
            {
                case "C":
                    //for company
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    //if (fm.CompanyFilter.Count > 0)
                    //{
                    //    foreach (var comp in fm.CompanyFilter)
                    //    {
                    //        short _compID = Convert.ToInt16(comp.ID);
                    //        TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _compID && aa.Criteria == Criteria).ToList());
                    //    }
                    //    ViewDS = TempDS.ToList();
                    //}
                    //else
                    //    TempDS = ViewDS.ToList();
                    //TempDS.Clear();
                    break;
                case "L":
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.LocationFilter.Count > 0)
                    {
                        foreach (var loc in fm.LocationFilter)
                        {
                            short _locID = Convert.ToInt16(loc.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _locID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;
                case "D":
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.DepartmentFilter.Count > 0)
                    {
                        foreach (var dept in fm.DepartmentFilter)
                        {
                            short _deptID = Convert.ToInt16(dept.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _deptID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;
                case "E":
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.SectionFilter.Count > 0)
                    {
                        foreach (var sec in fm.SectionFilter)
                        {
                            short _secID = Convert.ToInt16(sec.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _secID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;

                case "S":
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.ShiftFilter.Count > 0)
                    {
                        foreach (var shift in fm.ShiftFilter)
                        {
                            short _shiftID = Convert.ToInt16(shift.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _shiftID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;
                case "T":
                    dt = qb.GetValuesfromDB("select * from DailySummary " + " where Criteria = '" + Criteria + "' and (Date >= " + "'" + dateFrom + "'" + " and Date <= " + "'"
                                                     + dateTo + "'" + " )");
                    ViewDS = dt.ToList<DailySummary>();
                    if (fm.TypeFilter.Count > 0)
                    {
                        foreach (var type in fm.TypeFilter)
                        {
                            short _typeID = Convert.ToInt16(type.ID);
                            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _typeID && aa.Criteria == Criteria).ToList());
                        }
                        ViewDS = TempDS.ToList();
                    }
                    else
                        TempDS = ViewDS.ToList();
                    TempDS.Clear();
                    break;
                //case "A":
                //    if (fm.CompanyFilter.Count > 0)
                //    {
                //        foreach (var comp in fm.CompanyFilter)
                //        {
                //            short _compID = Convert.ToInt16(comp.ID);
                //            TempDS.AddRange(ViewDS.Where(aa => aa.CriteriaValue == _compID && aa.Criteria == Criteria).ToList());
                //        }
                //        ViewDS = TempDS.ToList();
                //    }
                //    else
                //        TempDS = ViewDS.ToList();
                //    TempDS.Clear();
                //    break;
            }
            return ViewDS;
        }

        private void LoadReport(string PathString, List<AttDeptSummary> AttDept, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<AttDeptSummary> ie;
            ie = AttDept.AsQueryable();
            IEnumerable<Option> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Title", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string PathString, List<ViewMultipleInOut> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(PathString);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewMultipleInOut> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<Option> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private List<ViewMultipleInOut> ReportsFilterImplementation(FiltersModel fm, List<ViewMultipleInOut> _TempViewList, List<ViewMultipleInOut> _ViewList)
        {

            //for company
            if (fm.GenderFilter.Count > 0)
            {
                foreach (var comp in fm.GenderFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.Gender == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            //if (fm.CrewFilter.Count > 0)
            //{
            //    foreach (var cre in fm.CrewFilter)
            //    {
            //        short _crewID = Convert.ToInt16(cre.ID);
            //        _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
            //    }
            //    _ViewList = _TempViewList.ToList();
            //}
            //else
            //    _TempViewList = _ViewList.ToList();
            //_TempViewList.Clear();





            //for division
            //if (fm.DivisionFilter.Count > 0)
            //{
            //    foreach (var div in fm.DivisionFilter)
            //    {
            //        short _divID = Convert.ToInt16(div.ID);
            //        _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
            //    }
            //    _ViewList = _TempViewList.ToList();
            //}
            //else
            //    _TempViewList = _ViewList.ToList();
            //_TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }

        private List<ViewLvApplication> ReportsFilterImplementation(FiltersModel fm, List<ViewLvApplication> _TempViewList, List<ViewLvApplication> _ViewList)
        {


            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();
            if (fm.GenderFilter.Count > 0)
            {
                foreach (var comp in fm.GenderFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.Gender == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            //if (fm.CrewFilter.Count > 0)
            //{
            //    foreach (var cre in fm.CrewFilter)
            //    {
            //        short _crewID = Convert.ToInt16(cre.ID);
            //        _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
            //    }
            //    _ViewList = _TempViewList.ToList();
            //}
            //else
            //    _TempViewList = _ViewList.ToList();
            //_TempViewList.Clear();





            //for division
            //if (fm.DivisionFilter.Count > 0)
            //{
            //    foreach (var div in fm.DivisionFilter)
            //    {
            //        short _divID = Convert.ToInt16(div.ID);
            //        _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
            //    }
            //    _ViewList = _TempViewList.ToList();
            //}
            //else
            //    _TempViewList = _ViewList.ToList();
            //_TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }
        //EmpView

        public List<EmpView> ReportsFilterImplementation(FiltersModel fm, List<EmpView> _TempViewList, List<EmpView> _ViewList)
        {
            if (fm.GenderFilter.Count > 0)
            {
                foreach (var comp in fm.GenderFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.Gender == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            //if (fm.CrewFilter.Count > 0)
            //{
            //    //foreach (var cre in fm.CrewFilter)
            //    //{
            //    //    short _crewID = Convert.ToInt16(cre.ID);
            //    //    _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
            //    //}
            //    _ViewList = _TempViewList.ToList();
            //}
            //else
            //    _TempViewList = _ViewList.ToList();
            //_TempViewList.Clear();





            //for division
            //if (fm.DivisionFilter.Count > 0)
            //{
            //    //foreach (var div in fm.DivisionFilter)
            //    //{
            //    //    short _divID = Convert.ToInt16(div.ID);
            //    //    _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
            //    //}
            //    _ViewList = _TempViewList.ToList();
            //}
            //else
            //    _TempViewList = _ViewList.ToList();
            //_TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                //foreach (var dept in fm.DepartmentFilter)
                //{
                //    short _deptID = Convert.ToInt16(dept.ID);
                //    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                //}
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }

        //ViewAttData
        public List<ViewAttData> ReportsFilterImplementation(FiltersModel fm, List<ViewAttData> _TempViewList, List<ViewAttData> _ViewList)
        {


            if (fm.GenderFilter.Count > 0)
            {
                foreach (var comp in fm.GenderFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.Gender == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            //if (fm.CrewFilter.Count > 0)
            //{
            //    foreach (var cre in fm.CrewFilter)
            //    {
            //        short _crewID = Convert.ToInt16(cre.ID);
            //        _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
            //    }
            //    _ViewList = _TempViewList.ToList();
            //}
            //else
            //    _TempViewList = _ViewList.ToList();
            //_TempViewList.Clear();





            //for division
            //if (fm.DivisionFilter.Count > 0)
            //{
            //    foreach (var div in fm.DivisionFilter)
            //    {
            //        short _divID = Convert.ToInt16(div.ID);
            //        _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
            //    }
            //    _ViewList = _TempViewList.ToList();
            //}
            //else
            //    _TempViewList = _ViewList.ToList();
            //_TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }

        public List<Option> GetCompanyImages(FiltersModel fm)
        {
            TAS2013Entities ctx = new TAS2013Entities();
            companyimage = new List<Option>();

            companyimage.Add(ctx.Options.Where(aa => aa.ID == 1).First());

            return companyimage;

        }

        //ViewAttData
        public List<ViewDetailAttData> ReportsFilterImplementation(FiltersModel fm, List<ViewDetailAttData> _TempViewList, List<ViewDetailAttData> _ViewList)
        {


            if (fm.GenderFilter.Count > 0)
            {
                foreach (var comp in fm.GenderFilter)
                {
                    short _compID = Convert.ToInt16(comp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.Gender == _compID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            //if (fm.CrewFilter.Count > 0)
            //{
            //    foreach (var cre in fm.CrewFilter)
            //    {
            //        short _crewID = Convert.ToInt16(cre.ID);
            //        _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
            //    }
            //    _ViewList = _TempViewList.ToList();
            //}
            //else
            //    _TempViewList = _ViewList.ToList();
            //_TempViewList.Clear();





            //for division
            //if (fm.DivisionFilter.Count > 0)
            //{
            //    foreach (var div in fm.DivisionFilter)
            //    {
            //        short _divID = Convert.ToInt16(div.ID);
            //        _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
            //    }
            //    _ViewList = _TempViewList.ToList();
            //}
            //else
            //    _TempViewList = _ViewList.ToList();
            //_TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }

        private void LoadReport(string path, List<ViewDetailAttData> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewDetailAttData> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<Option> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);

            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        //ViewMonthlyData
        public List<ViewMonthlyData> ReportsFilterImplementation(FiltersModel fm, List<ViewMonthlyData> _TempViewList, List<ViewMonthlyData> _ViewList)
        {



            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            //if (fm.CrewFilter.Count > 0)
            //{
            //    foreach (var cre in fm.CrewFilter)
            //    {
            //        short _crewID = Convert.ToInt16(cre.ID);
            //        _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
            //    }
            //    _ViewList = _TempViewList.ToList();
            //}
            //else
            //    _TempViewList = _ViewList.ToList();
            //_TempViewList.Clear();





            //for division
            //if (fm.DivisionFilter.Count > 0)
            //{
            //    foreach (var div in fm.DivisionFilter)
            //    {
            //        short _divID = Convert.ToInt16(div.ID);
            //        _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
            //    }
            //    _ViewList = _TempViewList.ToList();
            //}
            //else
            //    _TempViewList = _ViewList.ToList();
            //_TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }

        private void LoadReport(string path, List<ViewMonthlyData> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewMonthlyData> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<Option> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);
            date = "";
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        //ViewMonthlyDataPer
        public List<ViewMonthlyDataPer> ReportsFilterImplementation(FiltersModel fm, List<ViewMonthlyDataPer> _TempViewList, List<ViewMonthlyDataPer> _ViewList)
        {

            //for location
            if (fm.LocationFilter.Count > 0)
            {
                foreach (var loc in fm.LocationFilter)
                {
                    short _locID = Convert.ToInt16(loc.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.LocID == _locID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for shifts
            if (fm.ShiftFilter.Count > 0)
            {
                foreach (var shift in fm.ShiftFilter)
                {
                    short _shiftID = Convert.ToInt16(shift.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.ShiftID == _shiftID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();


            _TempViewList.Clear();

            //for type
            if (fm.TypeFilter.Count > 0)
            {
                foreach (var type in fm.TypeFilter)
                {
                    short _typeID = Convert.ToInt16(type.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.TypeID == _typeID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for crews
            //if (fm.CrewFilter.Count > 0)
            //{
            //    foreach (var cre in fm.CrewFilter)
            //    {
            //        short _crewID = Convert.ToInt16(cre.ID);
            //        _TempViewList.AddRange(_ViewList.Where(aa => aa.CrewID == _crewID).ToList());
            //    }
            //    _ViewList = _TempViewList.ToList();
            //}
            //else
            //    _TempViewList = _ViewList.ToList();
            //_TempViewList.Clear();





            //for division
            //if (fm.DivisionFilter.Count > 0)
            //{
            //    foreach (var div in fm.DivisionFilter)
            //    {
            //        short _divID = Convert.ToInt16(div.ID);
            //        _TempViewList.AddRange(_ViewList.Where(aa => aa.DivID == _divID).ToList());
            //    }
            //    _ViewList = _TempViewList.ToList();
            //}
            //else
            //    _TempViewList = _ViewList.ToList();
            //_TempViewList.Clear();

            //for department
            if (fm.DepartmentFilter.Count > 0)
            {
                foreach (var dept in fm.DepartmentFilter)
                {
                    short _deptID = Convert.ToInt16(dept.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.DeptID == _deptID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //for sections
            if (fm.SectionFilter.Count > 0)
            {
                foreach (var sec in fm.SectionFilter)
                {
                    short _secID = Convert.ToInt16(sec.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.SecID == _secID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();

            //Employee
            if (fm.EmployeeFilter.Count > 0)
            {
                foreach (var emp in fm.EmployeeFilter)
                {
                    int _empID = Convert.ToInt32(emp.ID);
                    _TempViewList.AddRange(_ViewList.Where(aa => aa.EmpID == _empID).ToList());
                }
                _ViewList = _TempViewList.ToList();
            }
            else
                _TempViewList = _ViewList.ToList();
            _TempViewList.Clear();


            return _ViewList;
        }

        private void LoadReport(string path, List<ViewMonthlyDataPer> _Employee, String date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;

            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewMonthlyDataPer> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<Option> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            ReportParameter rp1 = new ReportParameter("Date", date, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string path, DataTable _LvSummary)
        {
            string _Header = "Year wise Leaves Summary";
            this.ReportViewer1.LocalReport.DisplayName = "Leave Summary Report";
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", _LvSummary);
            IEnumerable<Option> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });
            ReportViewer1.LocalReport.Refresh();
        }
        ///
        private void LoadReport(string path, List<EmpView> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<EmpView> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<Option> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.HyperlinkTarget = "_blank";


            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string path, List<ViewAttData> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewAttData> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<Option> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.HyperlinkTarget = "_blank";


            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Date", date, false);
            ReportParameter rp1 = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }

        private void LoadReport(string path, DataTable _LvSummary, int i)
        {
            string _Header = "Monthly Sheet Contactual Employees";
            string Date = Convert.ToDateTime(_dateFrom).Date.ToString("dd-MMM-yyyy");
            this.ReportViewer1.LocalReport.DisplayName = "Leave Balance Report";
            string _Date = "Month: " + Convert.ToDateTime(_dateFrom).Date.ToString("MMMM") + " , " + Convert.ToDateTime(_dateFrom).Year.ToString();
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", _LvSummary);
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.HyperlinkTarget = "_blank";
            IEnumerable<Option> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            ReportParameter rp1 = new ReportParameter("Date", Date, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp, rp1 });
            ReportViewer1.LocalReport.Refresh();
        }
        private void LoadReport(string path, List<ViewLvApplication> _Employee, string date)
        {
            string _Header = title;
            this.ReportViewer1.LocalReport.DisplayName = title;
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = Server.MapPath(path);
            System.Security.PermissionSet sec = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            ReportViewer1.LocalReport.SetBasePermissionsForSandboxAppDomain(sec);
            IEnumerable<ViewLvApplication> ie;
            ie = _Employee.AsQueryable();
            IEnumerable<Option> companyImage;
            companyImage = companyimage.AsQueryable();
            ReportDataSource datasource1 = new ReportDataSource("DataSet1", ie);
            ReportDataSource datasource2 = new ReportDataSource("DataSet2", companyImage);

            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.EnableExternalImages = true;
            ReportViewer1.LocalReport.DataSources.Add(datasource1);
            ReportViewer1.LocalReport.DataSources.Add(datasource2);
            ReportParameter rp = new ReportParameter("Header", _Header, false);
            this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rp });
            ReportViewer1.LocalReport.Refresh();
        }
        private DataTable GYL(List<EmpView> _Emp)
        {
            TAS2013Entities context = new TAS2013Entities();
            List<LvConsumed> leaveQuota = new List<LvConsumed>();
            List<LvConsumed> tempLeaveQuota = new List<LvConsumed>();
            leaveQuota = context.LvConsumeds.ToList();
            foreach (var emp in _Emp)
            {
                int EmpID;
                string EmpNo = ""; string EmpName = "";
                float TotalAL = 0; float BalAL = 0; float TotalCL = 0; float BalCL = 0; float TotalSL = 0; float BalSL = 0;
                float JanAL = 0; float JanCL = 0; float JanSL = 0; float FebAL = 0; float FebCL = 0; float FebSL = 0;
                float MarchAL = 0; float MarchCL = 0; float MarchSL = 0;
                float AprilAL = 0; float AprilCL = 0; float AprilSL = 0;
                float MayAL = 0; float MayCL = 0; float MaySL = 0;
                float JunAL = 0; float JunCL = 0; float JunSL = 0;
                float JullyAL = 0; float JullyCL = 0; float JullySL = 0;
                float AugAL = 0; float AugCL = 0; float AugSL = 0;
                float SepAL = 0; float SepCL = 0; float SepSL = 0;
                float OctAL = 0; float OctCL = 0; float OctSL = 0;
                float NovAL = 0; float NovCL = 0; float NovSL = 0;
                float DecAL = 0; float DecCL = 0; float DecSL = 0;
                string Remarks = ""; string DeptName; short DeptID; string LocationName; short LocationID; string SecName; short SecID; string DesgName; short DesigID; string CrewName; short CrewID; string CompanyName; short CompanyID;
                tempLeaveQuota = leaveQuota.Where(aa => aa.EmpID == emp.EmpID).ToList();
                foreach (var leave in tempLeaveQuota)
                {
                    EmpID = emp.EmpID;
                    EmpNo = emp.EmpNo;
                    EmpName = emp.EmpName;
                    //DeptID = (short)emp.DeptID;
                    DeptName = emp.DeptName;
                    LocationName = emp.LocName;
                    LocationID = (short)emp.LocID;
                    SecName = emp.SectionName;
                    SecID = (short)emp.SecID;
                    DesgName = emp.DesignationName;
                    DesigID = (short)emp.DesigID;
                    //CrewName = emp.CrewName;
                    //CrewID = (short)emp.CrewID;
                    switch (leave.LeaveTypeID)
                    {
                        case "A"://Casual
                            JanCL = (float)leave.JanConsumed;
                            FebCL = (float)leave.FebConsumed;
                            MarchCL = (float)leave.MarchConsumed;
                            AprilCL = (float)leave.AprConsumed;
                            MayCL = (float)leave.MayConsumed;
                            JunCL = (float)leave.JuneConsumed;
                            JullyCL = (float)leave.JulyConsumed;
                            AugCL = (float)leave.AugustConsumed;
                            SepCL = (float)leave.SepConsumed;
                            OctCL = (float)leave.OctConsumed;
                            NovCL = (float)leave.NovConsumed;
                            DecCL = (float)leave.DecConsumed;
                            TotalCL = (float)leave.TotalForYear;
                            BalCL = (float)leave.YearRemaining;
                            break;
                        case "B"://Anual
                            JanAL = (float)leave.JanConsumed;
                            FebAL = (float)leave.FebConsumed;
                            MarchAL = (float)leave.MarchConsumed;
                            AprilAL = (float)leave.AprConsumed;
                            MayAL = (float)leave.MayConsumed;
                            JunAL = (float)leave.JuneConsumed;
                            JullyAL = (float)leave.JulyConsumed;
                            AugAL = (float)leave.AugustConsumed;
                            SepAL = (float)leave.SepConsumed;
                            OctAL = (float)leave.OctConsumed;
                            NovAL = (float)leave.NovConsumed;
                            DecAL = (float)leave.DecConsumed;
                            TotalAL = (float)leave.TotalForYear;
                            BalAL = (float)leave.YearRemaining;
                            break;
                        case "C"://Sick
                            JanSL = (float)leave.JanConsumed;
                            FebSL = (float)leave.FebConsumed;
                            MarchSL = (float)leave.MarchConsumed;
                            AprilSL = (float)leave.AprConsumed;
                            MaySL = (float)leave.MayConsumed;
                            JunSL = (float)leave.JuneConsumed;
                            JullySL = (float)leave.JulyConsumed;
                            AugSL = (float)leave.AugustConsumed;
                            SepSL = (float)leave.SepConsumed;
                            OctSL = (float)leave.OctConsumed;
                            NovSL = (float)leave.NovConsumed;
                            DecSL = (float)leave.DecConsumed;
                            TotalSL = (float)leave.TotalForYear;
                            BalSL = (float)leave.YearRemaining;
                            break;
                    }
                    //AddDataToDT(EmpID, EmpNo, EmpName, TotalAL, BalAL, TotalCL, BalCL, TotalSL, BalSL, JanAL, JanCL, JanSL, FebAL, FebCL, FebSL, MarchAL, MarchCL, MarchSL, AprilAL, AprilCL, AprilSL, MayAL, MayCL, MaySL, JunAL, JunCL, JunSL, JullyAL, JullyCL, JullySL, AugAL, AugCL, AugSL, SepAL, SepCL, SepSL, OctAL, OctCL, OctSL, NovAL, NovCL, NovSL, DecAL, DecCL, DecSL, Remarks, DeptName, (short)DeptID, LocationName, (short)LocationID, SecName, (short)SecID, DesgName, DesigID, CrewName, CrewID);
                }
            }
            return MYLeaveSummaryDT;
        }
        DataTable MYLeaveSummaryDT = new DataTable();
        public void CreateDataTable()
        {
            MYLeaveSummaryDT.Columns.Add("EmpID", typeof(int));
            MYLeaveSummaryDT.Columns.Add("EmpNo", typeof(string));
            MYLeaveSummaryDT.Columns.Add("EmpName", typeof(string));

            MYLeaveSummaryDT.Columns.Add("TotalAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("BalAL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("TotalCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("BalCL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("TotalSL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("BalSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("JanAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JanCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JanSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("FebAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("FebCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("FebSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("MarchAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("MarchCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("MarchSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("AprilAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("AprilCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("AprilSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("MayAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("MayCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("MaySL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("JunAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JunCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JunSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("JulyAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JulyCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("JulySL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("AugAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("AugCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("AugSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("SepAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("SepCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("SepSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("OctAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("OctCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("OctSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("NovAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("NovCL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("NovSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("DecAL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("DecL", typeof(float));
            MYLeaveSummaryDT.Columns.Add("DecSL", typeof(float));

            MYLeaveSummaryDT.Columns.Add("Remarks", typeof(string));
            MYLeaveSummaryDT.Columns.Add("DeptName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("DeptID", typeof(short));
            MYLeaveSummaryDT.Columns.Add("SecName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("SecID", typeof(short));
            MYLeaveSummaryDT.Columns.Add("DesgName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("DesgID", typeof(short));
            MYLeaveSummaryDT.Columns.Add("CrewName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("CrewID", typeof(short));
            MYLeaveSummaryDT.Columns.Add("CompanyName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("CompanyID", typeof(short));
            MYLeaveSummaryDT.Columns.Add("LocationName", typeof(string));
            MYLeaveSummaryDT.Columns.Add("LocationID", typeof(short));
            LvSummaryMonth.Columns.Add("EmpNo", typeof(string));
            LvSummaryMonth.Columns.Add("EmpName", typeof(string));
            LvSummaryMonth.Columns.Add("Designation", typeof(string));
            LvSummaryMonth.Columns.Add("Section", typeof(string));
            LvSummaryMonth.Columns.Add("Department", typeof(string));
            LvSummaryMonth.Columns.Add("EmpType", typeof(string));
            LvSummaryMonth.Columns.Add("Category", typeof(string));
            LvSummaryMonth.Columns.Add("Location", typeof(string));
            LvSummaryMonth.Columns.Add("TotalCL", typeof(float));
            LvSummaryMonth.Columns.Add("TotalSL", typeof(float));
            LvSummaryMonth.Columns.Add("TotalAL", typeof(float));
            LvSummaryMonth.Columns.Add("ConsumedCL", typeof(float));
            LvSummaryMonth.Columns.Add("ConsumedSL", typeof(float));
            LvSummaryMonth.Columns.Add("ConsumedAL", typeof(float));
            LvSummaryMonth.Columns.Add("BalanceCL", typeof(float));
            LvSummaryMonth.Columns.Add("BalanceSL", typeof(float));
            LvSummaryMonth.Columns.Add("BalanceAL", typeof(float));
            LvSummaryMonth.Columns.Add("Remarks", typeof(string));
            LvSummaryMonth.Columns.Add("Month", typeof(string));
        }


        public void AddDataToDT(int EmpID, string EmpNo, string EmpName, float TotalAL, float BalAL,
            float TotalCL, float BalCL, float TotalSL, float BalSL,
            float JanAL, float JanCL, float JanSL,
            float FebAL, float FebCL, float FebSL,
            float MarchAL, float MarchCL, float MarchSL,
            float AprilAL, float AprilCL, float AprilSL,
            float MayAL, float MayCL, float MaySL,
            float JunAL, float JunCL, float JunSL,
            float JullyAL, float JullyCL, float JullySL,
            float AugAL, float AugCL, float AugSL,
            float SepAL, float SepCL, float SepSL,
            float OctAL, float OctCL, float OctSL,
            float NovAL, float NovCL, float NovSL,
            float DecAL, float DecCL, float DecSL,
            string Remarks, string DeptName, short DeptID, string LocationName, short LocationID, string SecName, short SecID, string DesgName, short DesgID, string CrewName, short CrewID)
        {
            MYLeaveSummaryDT.Rows.Add(EmpID, EmpNo, EmpName, TotalAL, BalAL, TotalCL, BalCL, TotalSL, BalSL, JanAL, JanCL, JanSL, FebAL, FebCL, FebSL, MarchAL, MarchCL, MarchSL,
                AprilAL, AprilCL, AprilSL, MayAL, MayCL, MaySL, JunAL, JunCL, JunSL, JullyAL, JullyCL, JullySL, AugAL, AugCL, AugSL,
                SepAL, SepCL, SepSL, OctAL, OctCL, OctSL, NovAL, NovCL, NovSL, DecAL, DecCL, DecSL, Remarks, DeptName, DeptID, LocationName, LocationID, CrewName, CrewID, SecName, SecID);
        }

        #region --Leave Process--
        private DataTable GetLV(List<EmpView> _Emp, int month)
        {
            using (var ctx = new TAS2013Entities())
            {

                List<LvConsumed> _lvConsumed = new List<LvConsumed>();
                LvConsumed _lvTemp = new LvConsumed();
                _lvConsumed = ctx.LvConsumeds.ToList();
                List<LvType> _lvTypes = ctx.LvTypes.ToList();
                foreach (var emp in _Emp)
                {
                    float BeforeCL = 0, UsedCL = 0, BalCL = 0;
                    float BeforeSL = 0, UsedSL = 0, BalSL = 0;
                    float BeforeAL = 0, UsedAL = 0, BalAL = 0;
                    string _month = "";
                    List<LvConsumed> entries = ctx.LvConsumeds.Where(aa => aa.EmpID == emp.EmpID).ToList();
                    LvConsumed eCL = entries.FirstOrDefault(lv => lv.LeaveTypeID == "A");
                    LvConsumed eSL = entries.FirstOrDefault(lv => lv.LeaveTypeID == "C");
                    LvConsumed eAL = entries.FirstOrDefault(lv => lv.LeaveTypeID == "B");
                    if (entries.Count > 0)
                    {
                        switch (month)
                        {
                            case 1:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear;
                                UsedCL = (float)eCL.JanConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear;
                                UsedSL = (float)eSL.JanConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear;
                                UsedAL = (float)eAL.JanConsumed;
                                _month = "January";
                                break;
                            case 2:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - (float)eCL.JanConsumed;
                                UsedCL = (float)eCL.FebConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - (float)eSL.JanConsumed;
                                UsedSL = (float)eSL.FebConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - (float)eAL.JanConsumed;
                                UsedAL = (float)eAL.FebConsumed;
                                break;
                                _month = "Febu";
                            case 3:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed);
                                UsedCL = (float)eCL.MarchConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed);
                                UsedSL = (float)eSL.MarchConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed);
                                UsedAL = (float)eAL.MarchConsumed;
                                break;
                            case 4:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed);
                                UsedCL = (float)eCL.AprConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed);
                                UsedSL = (float)eSL.AprConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed);
                                UsedAL = (float)eAL.AprConsumed;
                                break;
                            case 5:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed);
                                UsedCL = (float)eCL.MayConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed);
                                UsedSL = (float)eSL.MayConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed);
                                UsedAL = (float)eAL.MayConsumed;
                                break;
                            case 6:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed);
                                UsedCL = (float)eCL.JuneConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed);
                                UsedSL = (float)eSL.JuneConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed);
                                UsedAL = (float)eAL.JuneConsumed;
                                break;
                            case 7:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed);
                                UsedCL = (float)eCL.JulyConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed);
                                UsedSL = (float)eSL.JulyConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed);
                                UsedAL = (float)eAL.JulyConsumed;
                                break;
                            case 8:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed);
                                UsedCL = (float)eCL.AugustConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed);
                                UsedSL = (float)eSL.AugustConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed);
                                UsedAL = (float)eAL.AugustConsumed;
                                break;
                            case 9:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed);
                                UsedCL = (float)eCL.SepConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed);
                                UsedSL = (float)eSL.SepConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed);
                                UsedAL = (float)eAL.SepConsumed;
                                break;
                            case 10:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed + (float)eCL.SepConsumed);
                                UsedCL = (float)eCL.OctConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed + (float)eSL.SepConsumed);
                                UsedSL = (float)eSL.OctConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed + (float)eAL.AugustConsumed);
                                UsedAL = (float)eAL.SepConsumed;
                                break;
                            case 11:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed + (float)eCL.SepConsumed + (float)eCL.OctConsumed);
                                UsedCL = (float)eCL.NovConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed + (float)eSL.SepConsumed + (float)eSL.OctConsumed);
                                UsedSL = (float)eSL.NovConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed + (float)eAL.AugustConsumed + (float)eAL.OctConsumed);
                                UsedAL = (float)eAL.NovConsumed;
                                break;
                            case 12:
                                // casual
                                BeforeCL = (float)eCL.TotalForYear - ((float)eCL.JanConsumed + (float)eCL.FebConsumed + (float)eCL.MarchConsumed + (float)eCL.AprConsumed + (float)eCL.MayConsumed + (float)eCL.JuneConsumed + (float)eCL.JulyConsumed + (float)eCL.AugustConsumed + (float)eCL.SepConsumed + (float)eCL.OctConsumed + (float)eCL.NovConsumed);
                                UsedCL = (float)eCL.DecConsumed;
                                //Sick
                                BeforeSL = (float)eSL.TotalForYear - ((float)eSL.JanConsumed + (float)eSL.FebConsumed + (float)eSL.MarchConsumed + (float)eSL.AprConsumed + (float)eSL.MayConsumed + (float)eSL.JuneConsumed + (float)eSL.JulyConsumed + (float)eSL.AugustConsumed + (float)eSL.SepConsumed + (float)eSL.OctConsumed + (float)eSL.NovConsumed);
                                UsedSL = (float)eSL.DecConsumed;
                                //Anual
                                BeforeAL = (float)eAL.TotalForYear - ((float)eAL.JanConsumed + (float)eAL.FebConsumed + (float)eAL.MarchConsumed + (float)eAL.AprConsumed + (float)eAL.MayConsumed + (float)eAL.JuneConsumed + (float)eAL.JulyConsumed + (float)eAL.AugustConsumed + (float)eAL.AugustConsumed + (float)eAL.OctConsumed + (float)eAL.NovConsumed);
                                UsedAL = (float)eAL.DecConsumed;
                                break;

                        }
                        BalCL = (float)(BeforeCL - UsedCL);
                        BalSL = (float)(BeforeSL - UsedSL);
                        BalAL = (float)(BeforeAL - UsedAL);
                        //AddDataToDT(emp.EmpNo, emp.EmpName, emp.DesignationName, emp.SectionName, emp.DeptName, emp.TypeName, emp.CatName, emp.LocName, BeforeCL, BeforeSL, BeforeAL, UsedCL, UsedSL, UsedAL, BalCL, BalSL, BalAL, _month);
                    }

                }
            }
            return LvSummaryMonth;
        }
        public void AddDataToDT(string EmpNo, string EmpName, string Designation, string Section,
                                 string Department, string EmpType, string Category, string Location,
                                 float TotalCL, float TotalSL, float TotalAL,
                                 float ConsumedCL, float ConsumedSL, float ConsumedAL,
                                 float BalanaceCL, float BalanaceSL, float BalananceAL, string Month)
        {
            LvSummaryMonth.Rows.Add(EmpNo, EmpName, Designation, Section, Department, EmpType, Category, Location,
                TotalCL, TotalSL, TotalAL, ConsumedCL, ConsumedSL, ConsumedAL,
                BalanaceCL, BalanaceSL, BalananceAL, Month);
        }
        DataTable LvSummaryMonth = new DataTable();


        #endregion



    }
    public partial class VMAttMnDataPer : AttMnDataPer
    {
        public float OldAnnualBalance { get; set; }
        public float Credited { get; set; }
        public float Taken { get; set; }
        public float NewBalane { get; set; }
        public float SickLeave { get; set; }
    }
}