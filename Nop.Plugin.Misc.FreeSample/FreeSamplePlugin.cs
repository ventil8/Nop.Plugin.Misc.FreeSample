using Nop.Core.Domain.Messages;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Messages;
using System.Text;
using System.Web.Routing;

namespace Nop.Plugin.Misc.FreeSample
{
    public class FreeSamplePlugin : BasePlugin, IMiscPlugin
    {
        private readonly IMessageTemplateService _messageTemplateService;

        public FreeSamplePlugin(IMessageTemplateService messageTemplateService)
        {
            _messageTemplateService = messageTemplateService;
        }

        public override void Install()
        {
            MessageTemplate template1 =
                _messageTemplateService.GetMessageTemplateByName("OrderFreeSample.Form");

            MessageTemplate template2 =
                _messageTemplateService.GetMessageTemplateByName("HomeInstallationQuote.Form");

            MessageTemplate template3 =
                _messageTemplateService.GetMessageTemplateByName("SupplyDeliveryQuote.Form");

            if (template1 == null)
                template1 = new MessageTemplate()
                {
                    Name = "OrderFreeSample.Form",
                    Subject = "New Free Sample Order",
                    Body = GetMessageTemplateBody1(),
                    IsActive = true
                };

            if (template2 == null)
                template2 = new MessageTemplate()
                {
                    Name = "HomeInstallationQuote.Form",
                    Subject = "New Home Installation Quote",
                    Body = GetMessageTemplateBody2(),
                    IsActive = true
                };

            if (template3 == null)
                template3 = new MessageTemplate()
                {
                    Name = "SupplyDeliveryQuote.Form",
                    Subject = "New Supply and Delivery Quote",
                    Body = GetMessageTemplateBody3(),
                    IsActive = true
                };
            _messageTemplateService.InsertMessageTemplate(template1);
            _messageTemplateService.InsertMessageTemplate(template2);
            _messageTemplateService.InsertMessageTemplate(template3);

            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.OrderForm",
                "Order Free Sample Form");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.HomeInstallationQuote",
                "Home Installation Quote Form");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.SupplyDeliveryQuote",
                "Supply and Delivery Quote Form");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.ProductName",
                "Product Name");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.Name",
                "Name");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.Company",
                "Company");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.Address",
                "Address");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.PostCode",
                "PostCode");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.Phone",
                "Tel No");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.Email",
                "E-mail");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.Rooms",
                "Number of Rooms");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.Flooring",
                "Area of Flooring in M2");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.ProjectStageId",
                "Stage of Project");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.AdditionalInfo",
                "Please provide any additional information using the space below");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.FreeCredit",
               "Interest Free Credit?");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.CommentEnquiry",
                "Please provide any additional comment or enquiry using the space below");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.Submit",
                "Submit");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.ErrorOccured",
                "Error occured. Please try again");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.SubmitFormSuccessful",
                "Thank you, your order has been submitted.");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.HomeInstallationQuoteSuccessful",
                "Thank you, your request for home installation quote has been submitted.");
            this.AddOrUpdatePluginLocaleResource("Plugin.Misc.FreeSample.SupplyDeliveryQuoteSuccessful",
                "Thank you, your request for Supply and Deliver quote has been submitted.");

            base.Install();
        }

        public override void Uninstall()
        {
            this.DeletePluginLocaleResource("Plugin.Misc.FreeSample.SubmitForm");
            this.DeletePluginLocaleResource("Plugin.Misc.FreeSample.Submit");
            this.DeletePluginLocaleResource("Plugin.Misc.FreeSample.ErrorOccured");
            this.DeletePluginLocaleResource("Plugin.Misc.FreeSample.SubmitFormSuccessful");

            base.Uninstall();
        }

       public void GetConfigurationRoute(out string actionName, out string controllerName,
            out RouteValueDictionary routeValues)
        {
            actionName = null;
            controllerName = null;
            routeValues = new RouteValueDictionary 
            { 
                { "Namespaces", null }, 
                { "area", null } 
            };
        }

        private string GetMessageTemplateBody1()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("<p>Hi Admin,</p>");
            body.AppendLine("<p>A new customer has ordered free sample:</p>");
            body.Append("<p>Product name: %FreeSample.ProductName% ");
            body.Append("<p>Name: %FreeSample.Name%</p>");
            body.Append("Company: %FreeSample.Company%</p>");
            body.Append("<p>Address: %FreeSample.Address%</p>");
            body.Append("<p>PostCode: %FreeSample.PostCode%</p>");
            body.Append("<p>Tel No: %FreeSample.Phone%</p>");
            body.Append("<p>E-mail: %FreeSample.Email%</p>");                    
            body.AppendLine("<p>=== Comments / Enquiries ===</p>");
            body.AppendLine("<p>%FreeSample.CommentEnquiry%</p>");

            return body.ToString();
        }

        private string GetMessageTemplateBody2()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("<p>Hi Admin,</p>");
            body.AppendLine("<p>A new customer has requested for a Home Installation Quote:</p>");
            body.Append("<p>Product name: %InstallationQuote.ProductName% ");
            body.Append("<p>Name: %InstallationQuote.Name%</p>");
            body.Append("<p>E-mail: %InstallationQuote.Email%</p>");
            body.Append("<p>Tel No: %InstallationQuote.Phone%</p>");
            body.Append("<p>PostCode: %InstallationQuote.PostCode%</p>");
            body.Append("<p>Number of Rooms: %InstallationQuote.Rooms%</p>");
            body.Append("<p>Area of Flooring in M2: %InstallationQuote.Flooring%</p>");
            body.Append("<p>Stage of Project: %InstallationQuote.ProjectStage%</p>");
            body.AppendLine("<p>=== Additional Information ===</p>");
            body.Append("<p>%InstallationQuote.AdditionalInfo%</p>");
            body.Append("<p>Interest Free Credit: %InstallationQuote.FreeCredit%</p>");

            return body.ToString();
        }

        private string GetMessageTemplateBody3()
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine("<p>Hi Admin,</p>");
            body.AppendLine("<p>A new customer has requested for a Supply and Delivery Quote:</p>");
            body.Append("<p>Product name: %InstallationQuote.ProductName% ");
            body.Append("<p>Name: %InstallationQuote.Name%</p>");
            body.Append("<p>E-mail: %InstallationQuote.Email%</p>");
            body.Append("<p>Tel No: %InstallationQuote.Phone%</p>");
            body.Append("<p>PostCode: %InstallationQuote.PostCode%</p>");
            body.Append("<p>Number of Rooms: %InstallationQuote.Rooms%</p>");
            body.Append("<p>Area of Flooring in M2: %InstallationQuote.Flooring%</p>");
            body.Append("<p>Stage of Project: %InstallationQuote.ProjectStage%</p>");
            body.AppendLine("<p>=== Additional Information ===</p>");
            body.Append("<p>%InstallationQuote.AdditionalInfo%</p>");
            body.Append("<p>Interest Free Credit: %InstallationQuote.FreeCredit%</p>");

            return body.ToString();
        }

    }
}