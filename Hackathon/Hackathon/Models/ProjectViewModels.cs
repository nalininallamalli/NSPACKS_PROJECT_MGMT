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
        [Required(ErrorMessage = "Please enter Project Name", AllowEmptyStrings = false)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter Description", AllowEmptyStrings = false)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "StartDate")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "EndDate")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime EndDate { get; set; }
    }

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Project> Projects { get; set; }
    }

}