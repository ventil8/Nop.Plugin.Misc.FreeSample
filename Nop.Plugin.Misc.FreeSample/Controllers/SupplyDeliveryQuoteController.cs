using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.FreeSample.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.UI.Captcha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.FreeSample.Controllers
{
    public class SupplyDeliveryQuoteController : Controller
    {
        private const string SUPPLY_DELIVERY_QUOTE_VIEW =
            "Nop.Plugin.Misc.FreeSample.Views.SupplyDeliveryQuote";
        private const string SUPPLY_DELIVERY_QUOTE_SUCCESSFUL_VIEW =
            "Nop.Plugin.Misc.FreeSample.Views.SupplyDeliveryQuoteSuccessful";

        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly ITokenizer _tokenizer;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IProductService _productService;

        private readonly CaptchaSettings _captchaSettings;
        private readonly EmailAccountSettings _emailAccountSettings;

        public SupplyDeliveryQuoteController(
            IWorkContext workContext,
            ISettingService settingService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IMessageTokenProvider messageTokenProvider,
            IEmailAccountService emailAccountService,
            IEventPublisher eventPublisher,
            IMessageTemplateService messageTemplateService,
            ITokenizer tokenizer,
            IQueuedEmailService queuedEmailService,
            IProductService productService,

            CaptchaSettings captchaSettings,
            EmailAccountSettings emailAccountSettings)
        {
            _workContext = workContext;
            _settingService = settingService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _messageTokenProvider = messageTokenProvider;
            _emailAccountService = emailAccountService;
            _eventPublisher = eventPublisher;
            _messageTemplateService = messageTemplateService;
            _tokenizer = tokenizer;
            _queuedEmailService = queuedEmailService;
            _productService = productService;
            _captchaSettings = captchaSettings;
            _emailAccountSettings = emailAccountSettings;
        }


        public ActionResult SupplyDeliveryQuote(int productId)
        {
            Product product = _productService.GetProductById(productId);
            SupplyDeliveryQuoteModel model = new SupplyDeliveryQuoteModel();

            if (product != null)
                model.ProductName = product.Name;

            return View(SUPPLY_DELIVERY_QUOTE_VIEW, model);
        }

        [HttpPost]
        public ActionResult SupplyDeliveryQuote(SupplyDeliveryQuoteModel Model)
        {
            if (ModelState.IsValid)
            {
                bool successfullySent = SendFreeSampleMessage(Model);

                if (successfullySent)
                    return RedirectToAction("SupplyDeliveryQuoteSuccessful");
                else
                {
                    ModelState.AddModelError("", _localizationService
                        .GetResource("Plugin.Misc.FreeSample.ErrorOccured"));
                }
            }

            return View(SUPPLY_DELIVERY_QUOTE_VIEW, Model);
        }

        public ActionResult SupplyDeliveryQuoteSuccessful()
        {
            return View(SUPPLY_DELIVERY_QUOTE_SUCCESSFUL_VIEW);
        }

        #region Helpers

        private bool SendFreeSampleMessage(SupplyDeliveryQuoteModel Model)
        {
            MessageTemplate messageTemplate =
                GetLocalizedActiveMessageTemplate("SupplyDeliveryQuote.Form");

            if (messageTemplate == null)
                return false;

            EmailAccount sendTo = GetEmailAccountOfMessageTemplate(messageTemplate,
                _workContext.WorkingLanguage.Id);
            IList<Token> tokens = GenerateTokens(Model);

            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);
            return 0 != SendMessage(messageTemplate, Model.Name, Model.Email, sendTo,
                _workContext.WorkingLanguage.Id, tokens);
        }

        private MessageTemplate GetLocalizedActiveMessageTemplate(string messageTemplateName)
        {
            var messageTemplate =
                _messageTemplateService.GetMessageTemplateByName(messageTemplateName);

            if (messageTemplate == null)
                return null;

            var isActive = messageTemplate.IsActive;

            if (!isActive)
                return null;

            return messageTemplate;
        }

        private IList<Token> GenerateTokens(SupplyDeliveryQuoteModel Model)
        {
            IList<Token> tokens = new List<Token>();

            string additionalMessage = string.IsNullOrWhiteSpace(Model.AdditionalInfo) ?
                "N/A" : Model.AdditionalInfo;
            string phone = string.IsNullOrWhiteSpace(Model.Phone) ?
                "N/A" : Model.Phone;
            
            string projectStage = GetProjectStageToken(Model);
            projectStage = string.IsNullOrWhiteSpace(projectStage) ? "N/A" : projectStage;

            _messageTokenProvider.AddStoreTokens(tokens);
            tokens.Add(new Token("SupplyDeliveryQuote.ProductName", Model.ProductName));
            tokens.Add(new Token("SupplyDeliveryQuote.Name", Model.Name));
            tokens.Add(new Token("SupplyDeliveryQuote.Email", Model.Email));
            tokens.Add(new Token("SupplyDeliveryQuote.Phone", phone));
            tokens.Add(new Token("SupplyDeliveryQuote.PostCode", Model.PostCode));
            tokens.Add(new Token("SupplyDeliveryQuote.Rooms", Model.Rooms.ToString()));
            tokens.Add(new Token("SupplyDeliveryQuote.Flooring", Model.Flooring.ToString()));
            tokens.Add(new Token("SupplyDeliveryQuote.ProjectStage", projectStage));
            tokens.Add(new Token("SupplyDeliveryQuote.AdditionalInfo", additionalMessage));

            if (Model.FreeCredit)
                tokens.Add(new Token("SupplyDeliveryQuote.FreeCredit", "Yes"));
            else
                tokens.Add(new Token("SupplyDeliveryQuote.FreeCredit", "No"));

            return tokens;
        }

        private string GetProjectStageToken(SupplyDeliveryQuoteModel Model)
        {
            if (Model.ProjectStageId == 1)
                return ("Just looking for price");
            if (Model.ProjectStageId == 2)
                return ("Within next 4 weeks");
            if (Model.ProjectStageId == 3)
                return ("Within next 3 months");
            if (Model.ProjectStageId == 4)
                return ("Within next 6 months");
            else
                return null;
        }

        private EmailAccount GetEmailAccountOfMessageTemplate(MessageTemplate messageTemplate,
            int languageId)
        {
            int emailAccounId = messageTemplate.GetLocalized(mt => mt.EmailAccountId, languageId);
            EmailAccount emailAccount = _emailAccountService.GetEmailAccountById(emailAccounId);

            if (emailAccount == null)
                emailAccount = _emailAccountService
                    .GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);

            if (emailAccount == null)
                emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();

            return emailAccount;
        }

        private int SendMessage(MessageTemplate messageTemplate, string fromName, string fromEmail,
            EmailAccount to, int languageId, IEnumerable<Token> tokens)
        {
            //retrieve localized message template data
            var bcc = messageTemplate.GetLocalized((mt) => mt.BccEmailAddresses, languageId);
            var subject = messageTemplate.GetLocalized((mt) => mt.Subject, languageId);
            var body = messageTemplate.GetLocalized((mt) => mt.Body, languageId);

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, false);

            var email = new QueuedEmail()
            {
                Priority = 5,
                From = fromEmail,
                FromName = fromName,
                To = to.Email,
                ToName = to.DisplayName,
                CC = string.Empty,
                Bcc = bcc,
                Subject = subjectReplaced,
                Body = bodyReplaced,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = to.Id
            };

            _queuedEmailService.InsertQueuedEmail(email);

            return email.Id;
        }

        #endregion
    }
}
