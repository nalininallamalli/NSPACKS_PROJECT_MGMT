using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class Location
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
   //     public virtual Projects ProjectId { get; set; }
    }
}