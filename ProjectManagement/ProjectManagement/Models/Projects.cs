using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class Projects
    {
        public int ID { get; set; }
        public string ProjectName { get; set; }
        public string City { get; set; }
     //   public virtual Location LocationId { get; set; }
        public int FundId { get; set; }
        public int Proirity { get; set; }
        public string Category { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}