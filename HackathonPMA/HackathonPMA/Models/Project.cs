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
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    
    public partial class Project
    {
        public Project()
        {
            this.EmployeeProjects = new HashSet<EmployeeProject>();
            this.FundProjects = new HashSet<FundProject>();

            this.applicationDbContext = new ApplicationDbContext();
            this.userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.applicationDbContext));
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string City { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public ProjectState State { get; set; }
        public bool IsParent { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public double TotalAllocatedAmount { get; set; }
        public double TotalSpentAmount { get; set; }
        public string SubProjectIds { get; set; }
        public int ParentProjectId { get; set; }
        public Nullable<Int32> TotalSubProjects { get; set; }
        public string SpendingDetails { get; set; }

        /// <summary>
        /// Application DB context
        /// </summary>
        protected ApplicationDbContext applicationDbContext { get; set; }

        /// <summary>
        /// User manager - attached to application DB context
        /// </summary>
        protected UserManager<ApplicationUser> userManager { get; set; }

        public virtual ICollection<EmployeeProject> EmployeeProjects { get; set; }
        public virtual ICollection<FundProject> FundProjects { get; set; }
    }
}
