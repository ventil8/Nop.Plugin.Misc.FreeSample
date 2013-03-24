using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DataAnnotationsExtensions;
using Nop.Web.Framework;

namespace Nop.Plugin.Misc.FreeSample.Models
{
    public class HomeInstallationQuoteModel
    {
        public HomeInstallationQuoteModel()
        {
            ProjectStages = new List<SelectListItem>();
            ProjectStages.Add(new SelectListItem()
            {
                Text = "Just looking for price",
                Value = "1"
            });
            ProjectStages.Add(new SelectListItem()
            {
                Text = "Within next 4 weeks",
                Value = "2"
            });
            ProjectStages.Add(new SelectListItem()
            {
                Text = "Within next 3 months",
                Value = "3"
            });
            ProjectStages.Add(new SelectListItem()
            {
                Text = "Within next 6 months",
                Value = "4"
            });
        }

        [Required]
        [NopResourceDisplayName("Plugin.Misc.FreeSample.ProductName")]
        public string ProductName { get; set; }

        [Required]
        [NopResourceDisplayName("Plugin.Misc.FreeSample.Name")]
        public string Name { get; set; }

        [Required]
        [Email]
        [NopResourceDisplayName("Plugin.Misc.FreeSample.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Plugin.Misc.FreeSample.Phone")]
        public string Phone { get; set; }

        [Required]
        [NopResourceDisplayName("Plugin.Misc.FreeSample.PostCode")]
        public string PostCode { get; set; }

        [Required]
        [NopResourceDisplayName("Plugin.Misc.FreeSample.Rooms")]
        public int? Rooms { get; set; }

        [Required]
        [NopResourceDisplayName("Plugin.Misc.FreeSample.Flooring")]
        public decimal? Flooring { get; set; }

        [NopResourceDisplayName("Plugin.Misc.FreeSample.ProjectStageId")]
        public int ProjectStageId { get; set; }

        public IList<SelectListItem> ProjectStages { get; set; }

        [NopResourceDisplayName("Plugin.Misc.FreeSample.AdditionalInfo")]
        public string AdditionalInfo { get; set; }

        [NopResourceDisplayName("Plugin.Misc.FreeSample.FreeCredit")]
        public bool FreeCredit { get; set; }

    }
}
