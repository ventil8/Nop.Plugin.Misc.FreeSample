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
    public class OrderFreeSampleController : Controller
    {
        private const string ORDER_FREE_SAMPLE_VIEW =
            "Nop.Plugin.Misc.FreeSample.Views.OrderFreeSample";
        private const string ORDER_FREE_SAMPLE_SUCCESSFUL_VIEW =
            "Nop.Plugin.Misc.FreeSample.Views.OrderFreeSampleSuccessful";

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

        public OrderFreeSampleController(
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

        public ActionResult OrderFreeSample(int productId)
        {
            Product product = _productService.GetProductById(productId);
            OrderFreeSampleModel model = new OrderFreeSampleModel();

            if (product != null)
                model.ProductName = product.Name;

            return View(ORDER_FREE_SAMPLE_VIEW, model);
        }

        [HttpPost]
        public ActionResult OrderFreeSample(OrderFreeSampleModel Model)
        {
            if (ModelState.IsValid)
            {
                bool successfullySent = SendFreeSampleMessage(Model);

                if (successfullySent)
                    return RedirectToAction("OrderFreeSampleSuccessful");
                else
                {
                    ModelState.AddModelError("", _localizationService
                        .GetResource("Plugin.Misc.FreeSample.ErrorOccured"));
                }
            }

            return View(ORDER_FREE_SAMPLE_VIEW, Model);
        }

        public ActionResult OrderFreeSampleSuccessful()
        {
            return View(ORDER_FREE_SAMPLE_SUCCESSFUL_VIEW);
        }

        #region Helpers

        private bool SendFreeSampleMessage(OrderFreeSampleModel Model)
        {
            MessageTemplate messageTemplate =
                GetLocalizedActiveMessageTemplate("OrderFreeSample.Form");

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

        private IList<Token> GenerateTokens(OrderFreeSampleModel Model)
        {
            IList<Token> tokens = new List<Token>();
            string additionalMessage = string.IsNullOrWhiteSpace(Model.CommentEnquiry) ?
                "N/A" : Model.CommentEnquiry;

            string company = string.IsNullOrWhiteSpace(Model.Company) ?
                "N/A" : Model.Company;

            _messageTokenProvider.AddStoreTokens(tokens);
            tokens.Add(new Token("FreeSample.ProductName", Model.ProductName));
            tokens.Add(new Token("FreeSample.Name", Model.Name));
            tokens.Add(new Token("FreeSample.Company", company));
            tokens.Add(new Token("FreeSample.Address", Model.Address));
            tokens.Add(new Token("FreeSample.PostCode", Model.PostCode));
            tokens.Add(new Token("FreeSample.Phone", Model.Phone));
            tokens.Add(new Token("FreeSample.Email", Model.Email));
            tokens.Add(new Token("FreeSample.CommentEnquiry", additionalMessage));

            /*if (Model.CHECKED)
                tokens.Add(new Token("FreeSample.CommentEnquiry", "CHEKBOX CHECKED"));
            else
                tokens.Add(new Token("FreeSample.CommentEnquiry", "CHEKBOX NOT CHECKED"));*/

            return tokens;
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
