using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DataAnnotationsExtensions;
using Nop.Web.Framework;

namespace Nop.Plugin.Misc.FreeSample.Models
{
    public class OrderFreeSampleModel
    {
        [Required]
        [NopResourceDisplayName("Plugin.Misc.FreeSample.ProductName")]
        public string ProductName { get; set; }

        [Required]
        [NopResourceDisplayName("Plugin.Misc.FreeSample.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Plugin.Misc.FreeSample.Company")]
        public string Company { get; set; }

        [Required]
        [NopResourceDisplayName("Plugin.Misc.FreeSample.Address")]
        public string Address { get; set; }

        [Required]
        [NopResourceDisplayName("Plugin.Misc.FreeSample.PostCode")]
        public string PostCode { get; set; }

        [Required]
        [NopResourceDisplayName("Plugin.Misc.FreeSample.Phone")]
        public string Phone { get; set; }

        [Required]
        [Email]
        [NopResourceDisplayName("Plugin.Misc.FreeSample.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Plugin.Misc.FreeSample.CommentEnquiry")]
        public string CommentEnquiry { get; set; }
    }
}
