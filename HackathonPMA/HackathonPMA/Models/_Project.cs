using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;
using System;

namespace HackathonPMA.Models
{

    [MetadataType(typeof(Project.MetaData))]
    public partial class Project
    {
        private class MetaData {

            [Remote(
                "doesProjectNameExist", 
                "Projects",
                AdditionalFields = "oldName",
                ErrorMessage = "Project name already exists. Please enter a different product name.",
                HttpMethod = "POST"
            )]
            [Required]
            public string Name { get; set; }
        }
    }
}