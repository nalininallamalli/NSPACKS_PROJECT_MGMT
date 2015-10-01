using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace Hackathon.Models
{
    public class ProjectViewModels
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "StartDate")]
        public string StartDate { get; set; }

        [Required]
        [Display(Name = "EndDate")]
        public string EndDate { get; set; }
    }

    public class Project
    {
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

}