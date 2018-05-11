//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ViewJobCardApp
    {
        public short WorkCardID { get; set; }
        public string WorkCardName { get; set; }
        public Nullable<System.DateTime> DateEnded { get; set; }
        public Nullable<System.DateTime> DateStarted { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public int JobCardID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string DesignationName { get; set; }
        public string LocName { get; set; }
        public string TypeName { get; set; }
        public string ShiftName { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public Nullable<int> DesigID { get; set; }
        public Nullable<bool> Status { get; set; }
        public string SectionName { get; set; }
        public string DeptName { get; set; }
        public Nullable<byte> ShiftID { get; set; }
        public Nullable<short> LocID { get; set; }
        public Nullable<short> SecID { get; set; }
        public Nullable<byte> TypeID { get; set; }
        public int EmpID { get; set; }
        public short DeptID { get; set; }
        public string JobCardCriteria { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<int> CriteriaDate { get; set; }
        public Nullable<byte> Gender { get; set; }
    }
}
