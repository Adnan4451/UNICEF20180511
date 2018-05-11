using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMS.Models;
using WMS.Reports;

namespace WMS.CustomClass
{
    public class PermanentMonthly
    {
        TAS2013Entities context = new TAS2013Entities();
        List<AttData> EmpAttData = new List<AttData>();
        public VMAttMnDataPer processPermanentMonthlyAttSingle(DateTime startDate, DateTime endDate, EmpView _Emp, List<AttData> _EmpAttData)
        {
            //Get Attendance data of employee according to selected month
            _attMonth = new VMAttMnDataPer();
            try
            {
                EmpAttData = _EmpAttData;
            }
            catch (Exception ex)
            {

            }
            string EmpMonth = _Emp.EmpID + endDate.Date.Month.ToString();
            //Check for already processed data
            _attMonth.StartDate = startDate;
            _attMonth.EndDate = endDate;
            TDays = 0;
            WorkDays = 0;
            PresentDays = 0;
            AbsentDays = 0;
            LeaveDays = 0;
            RestDays = 0;
            GZDays = 0;
            EarlyIn = 0;
            EarlyOut = 0;
            LateIn = 0;
            LateOut = 0;
            WorkTime = 0;
            NOT = 0;
            GOT = 0;
            TDays = Convert.ToByte((endDate - startDate).Days + 1);
            CalculateMonthlyAttendanceSheet(EmpAttData);
            _attMonth.Period = endDate.Date.Month.ToString() + endDate.Date.Year.ToString();
            _attMonth.EmpMonth = EmpMonth;
            _attMonth.EmpID = _Emp.EmpID;
            _attMonth.EmpNo = _Emp.EmpNo;
            _attMonth.EmpName = _Emp.EmpName;
            return _attMonth;
        }

        VMAttMnDataPer _attMonth = new VMAttMnDataPer();
        byte TDays = 0;
        byte WorkDays = 0;
        byte PresentDays = 0;
        byte AbsentDays = 0;
        byte LeaveDays = 0;
        byte RestDays = 0;
        byte GZDays = 0;
        Int16 EarlyIn = 0;
        Int16 EarlyOut = 0;
        Int16 LateIn = 0;
        Int16 LateOut = 0;
        Int16 WorkTime = 0;
        Int16 NOT = 0;
        Int16 GOT = 0;
        Int16 ExpectedWorkMins = 0;
        Int16 OfficialVisit = 0;

        private void CalculateMonthlyAttendanceSheet(List<AttData> _EmpAttData)
        {
            foreach (var item in _EmpAttData)
            {
                try
                {
                    Int16 OverTime = 0;
                    //current day is GZ holiday
                    if (item.StatusGZ == true && item.DutyCode == "G")
                    {
                        Marksheet(item.AttDate.Value.Day, "G");
                        GZDays++;
                    }
                    //if current day is Rest day
                    if (item.StatusDO == true && item.DutyCode == "R")
                    {
                        Marksheet(item.AttDate.Value.Day, "R");
                        RestDays++;
                    }
                    //current day is leave
                    if (item.StatusLeave == true)
                    {
                        if (item.Remarks.Contains("[SL]"))
                            Marksheet(item.AttDate.Value.Day, "S");
                        if (item.Remarks.Contains("[CL]"))
                            Marksheet(item.AttDate.Value.Day, "C");
                        if (item.Remarks.Contains("[AL]"))
                            Marksheet(item.AttDate.Value.Day, "/");
                        if (item.Remarks.Contains("[HAL]"))
                            Marksheet(item.AttDate.Value.Day, "4/");
                        if (item.Remarks.Contains("[HSL]"))
                            Marksheet(item.AttDate.Value.Day, "4/s");
                        if (item.Remarks.Contains("[HCL]"))
                            Marksheet(item.AttDate.Value.Day, "4/c");
                        LeaveDays++;
                    }
                    //if current day is absent
                    if (item.StatusAB == true && item.DutyCode == "D")
                    {
                        if (item.TimeIn == null && item.TimeOut == null)
                        {
                            Marksheet(item.AttDate.Value.Day, "A");
                            AbsentDays++;
                        }
                        if (item.TimeIn != null && item.TimeOut != null)
                        {
                            if (item.TimeIn.Value.TimeOfDay == item.TimeOut.Value.TimeOfDay)
                            {
                                Marksheet(item.AttDate.Value.Day, "A");
                                AbsentDays++;
                            }
                        }

                    }
                    //currentday is present
                    if (item.TimeIn != null && item.TimeOut != null)
                    {
                        if (item.DutyCode == "D")
                        {
                            Marksheet(item.AttDate.Value.Day, "P");
                            if (item.StatusDO != true && item.StatusGZ != true)
                            {
                                PresentDays++;
                            }
                            if (item.OTMin != null && item.OTMin > 0)
                            {
                                OverTime = (Int16)(OverTime + Convert.ToInt16(item.OTMin));
                            }
                            if (item.EarlyIn != null && item.EarlyIn > 0)
                            {
                                //OverTime = (Int16)(OverTime + Convert.ToInt16(item.EarlyIn));
                            }
                        }
                    }
                    //Manual 
                    if (item.StatusMN == true && item.DutyCode == "D")
                    {
                        if (item.TimeIn == null && item.TimeOut == null)
                        {
                            if (item.StatusP == true)
                            {
                                if (item.StatusDO != true && item.StatusAB != true)
                                {
                                    if (item.Remarks.Contains("Offical Duty"))
                                        Marksheet(item.AttDate.Value.Day, "O");
                                    if (item.Remarks.Contains("Field Visit"))
                                        Marksheet(item.AttDate.Value.Day, "F");
                                    if (item.Remarks.Contains("Training"))
                                        Marksheet(item.AttDate.Value.Day, "T");
                                    PresentDays++;

                                }
                            }
                        }
                    }
                    if (item.Remarks != null)
                    {
                        if (item.Remarks.Contains("Offical Duty"))
                            Marksheet(item.AttDate.Value.Day, "O");
                        if (item.Remarks.Contains("Field Visit"))
                            Marksheet(item.AttDate.Value.Day, "F");
                        if (item.Remarks.Contains("Training"))
                            Marksheet(item.AttDate.Value.Day, "T");
                        PresentDays++;
                        OfficialVisit++;
                        if (item.Remarks.Contains("[Badli]"))
                        {
                            if (!item.Remarks.Contains("[Official Duty]"))
                            {
                                Marksheet(item.AttDate.Value.Day, "B");
                                PresentDays++;
                            }
                        }
                    }
                    //Missing Attendance
                    if ((item.TimeIn == null && item.TimeOut != null) || (item.TimeIn != null && item.TimeOut == null))
                    {
                        Marksheet(item.AttDate.Value.Day, "I");
                    }
                    //Sum EarlyIn/Out, LateIn/Out, WorkTime, NOT, GOT
                    if (item.EarlyIn != null && item.EarlyIn > 0)
                        EarlyIn = (Int16)(EarlyIn + Convert.ToInt16(item.EarlyIn));
                    if (item.EarlyOut != null && item.EarlyOut > 0)
                        EarlyOut = (Int16)(EarlyOut + Convert.ToInt16(item.EarlyOut));
                    if (item.LateIn != null && item.LateIn > 0)
                        LateIn = (Int16)(LateIn + Convert.ToInt16(item.LateIn));
                    if (item.OTMin != null && item.OTMin > 0)
                        NOT = (Int16)(NOT + Convert.ToInt16(item.OTMin));
                    if (item.GZOTMin != null && item.GZOTMin > 0)
                        GOT = (Int16)(GOT + Convert.ToInt16(item.GZOTMin));
                    if (item.WorkMin != null && item.WorkMin > 0)
                        WorkTime = (Int16)(WorkTime + Convert.ToInt16(item.WorkMin));
                    if (item.ShifMin > 0)
                        ExpectedWorkMins = (short)(ExpectedWorkMins + item.ShifMin);
                }
                catch (Exception ex)
                {

                }
            }
            //           

        }

        public void Marksheet(int day, string _Code)
        {
            switch (day)
            {
                case 1:
                    _attMonth.D1 = _Code;
                    break;
                case 2:
                    _attMonth.D2 = _Code;
                    break;
                case 3:
                    _attMonth.D3 = _Code;
                    break;
                case 4:
                    _attMonth.D4 = _Code;
                    break;
                case 5:
                    _attMonth.D5 = _Code;
                    break;
                case 6:
                    _attMonth.D6 = _Code;
                    break;
                case 7:
                    _attMonth.D7 = _Code;
                    break;
                case 8:
                    _attMonth.D8 = _Code;
                    break;
                case 9:
                    _attMonth.D9 = _Code;
                    break;
                case 10:
                    _attMonth.D10 = _Code;
                    break;
                case 11:
                    _attMonth.D11 = _Code;
                    break;
                case 12:
                    _attMonth.D12 = _Code;
                    break;
                case 13:
                    _attMonth.D13 = _Code;
                    break;
                case 14:
                    _attMonth.D14 = _Code;
                    break;
                case 15:
                    _attMonth.D15 = _Code;
                    break;
                case 16:
                    _attMonth.D16 = _Code;
                    break;
                case 17:
                    _attMonth.D17 = _Code;
                    break;
                case 18:
                    _attMonth.D18 = _Code;
                    break;
                case 19:
                    _attMonth.D19 = _Code;
                    break;
                case 20:
                    _attMonth.D20 = _Code;
                    break;
                case 21:
                    _attMonth.D21 = _Code;
                    break;
                case 22:
                    _attMonth.D22 = _Code;
                    break;
                case 23:
                    _attMonth.D23 = _Code;
                    break;
                case 24:
                    _attMonth.D24 = _Code;
                    break;
                case 25:
                    _attMonth.D25 = _Code;
                    break;
                case 26:
                    _attMonth.D26 = _Code;
                    break;
                case 27:
                    _attMonth.D27 = _Code;
                    break;
                case 28:
                    _attMonth.D28 = _Code;
                    break;
                case 29:
                    _attMonth.D29 = _Code;
                    break;
                case 30:
                    _attMonth.D30 = _Code;
                    break;
                case 31:
                    _attMonth.D31 = _Code;
                    break;
            }
        }

        //For OT
    }
}