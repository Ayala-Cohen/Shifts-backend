//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Constraints
    {
        public string Employee_Id { get; set; }
        public int Shift_Id { get; set; }
        public string Day { get; set; }
    
        public virtual Employees Employees { get; set; }
        public virtual Shifts Shifts { get; set; }
    }
}