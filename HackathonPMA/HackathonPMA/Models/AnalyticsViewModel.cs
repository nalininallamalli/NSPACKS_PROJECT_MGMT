using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;


namespace HackathonPMA.Models
{
    public class ByProjectViewModel
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        [Display(Name = "Projects")]
        public SelectList ProjectList { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }
    }

    public class ByFundsViewModel
    {
        [Display(Name = "Project Id")]
        public int ProjectId { get; set; }

        public double Fund { get; set; }
    }

    public class CustomViewModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        public double Fund { get; set; }

    }
}







