//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HackathonPMA.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FundProject
    {
        public int Id { get; set; }
        public Nullable<int> FundId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<double> Amount { get; set; }
    
        public virtual Fund Fund { get; set; }
        public virtual Project Project { get; set; }
    }
}
