using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace HackathonPMA.Models
{

    [MetadataType(typeof(Inventory.MetaData))]
    public partial class Inventory
    {
        private class MetaData
        {

            [Remote(
                "doesInventoryNameExist",
                "Inventories",
                AdditionalFields = "oldName",
                ErrorMessage = "Inventory name already exists. Please enter a different product name.",
                HttpMethod = "POST"
            )]
            [Required]
            public string Name { get; set; }
        }

    }
}