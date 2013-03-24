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
    public class HomeInstallationQuoteController : Controller
    {
        private const string HOME_INSTALLATION_QUOTE_VIEW =
            "Nop.Plugin.Misc.FreeSample.Views.HomeInstallationQuote";
        private const string HOME_INSTALLATION_QUOTE_SUCCESSFUL_VIEW =
            "Nop.Plugin.Misc.FreeSample.Views.HomeInstallationQuoteSuccessful";

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

        public HomeInstallationQuoteController(
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


        public ActionResult HomeInstallationQuote(int productId)
        {
            Product product = _productService.GetProductById(productId);
            HomeInstallationQuoteModel model = new HomeInstallationQuoteModel();

            if (product != null)
                model.ProductName = product.Name;
            
            return View(HOME_INSTALLATION_QUOTE_VIEW, model);
        }

        [HttpPost]
        public ActionResult HomeInstallationQuote(HomeInstallationQuoteModel Model)
        {
            if (ModelState.IsValid)
            {
                bool successfullySent = SendFreeSampleMessage(Model);

                if (successfullySent)
                    return RedirectToAction("HomeInstallationQuoteSuccessful");
                else
                {
                    ModelState.AddModelError("", _localizationService
                        .GetResource("Plugin.Misc.FreeSample.ErrorOccured"));
                }
            }

            return View(HOME_INSTALLATION_QUOTE_VIEW, Model);
        }

        public ActionResult HomeInstallationQuoteSuccessful()
        {
            return View(HOME_INSTALLATION_QUOTE_SUCCESSFUL_VIEW);
        }

        #region Helpers

        private bool SendFreeSampleMessage(HomeInstallationQuoteModel Model)
        {
            MessageTemplate messageTemplate =
                GetLocalizedActiveMessageTemplate("HomeInstallationQuote.Form");

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

        private IList<Token> GenerateTokens(HomeInstallationQuoteModel Model)
        {
            IList<Token> tokens = new List<Token>();

            string additionalMessage = string.IsNullOrWhiteSpace(Model.AdditionalInfo) ?
                "N/A" : Model.AdditionalInfo;
            string phone = string.IsNullOrWhiteSpace(Model.Phone) ?
                "N/A" : Model.Phone;
            
            string projectStage = GetProjectStageToken(Model);
            projectStage = string.IsNullOrWhiteSpace(projectStage) ? "N/A" : projectStage;

            _messageTokenProvider.AddStoreTokens(tokens);
            tokens.Add(new Token("InstallationQuote.ProductName", Model.ProductName));
            tokens.Add(new Token("InstallationQuote.Name", Model.Name));
            tokens.Add(new Token("InstallationQuote.Email", Model.Email));
            tokens.Add(new Token("InstallationQuote.Phone", phone));
            tokens.Add(new Token("InstallationQuote.PostCode", Model.PostCode));
            tokens.Add(new Token("InstallationQuote.Rooms", Model.Rooms.ToString()));
            tokens.Add(new Token("InstallationQuote.Flooring", Model.Flooring.ToString()));
            tokens.Add(new Token("InstallationQuote.ProjectStage", projectStage));
            tokens.Add(new Token("InstallationQuote.AdditionalInfo", additionalMessage));

            if (Model.FreeCredit)
                tokens.Add(new Token("InstallationQuote.FreeCredit", "Yes"));
            else
                tokens.Add(new Token("InstallationQuote.FreeCredit", "No"));

            return tokens;
        }

        private string GetProjectStageToken(HomeInstallationQuoteModel Model)
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
