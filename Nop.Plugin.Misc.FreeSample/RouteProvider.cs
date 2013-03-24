using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Misc.FreeSample
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Misc.FreeSample.OrderFreeSample",
                "order-free-sample-form",
                new { controller = "OrderFreeSample", action = "OrderFreeSample" },
                new[] { "Nop.Plugin.Misc.FreeSample.Controllers" }
            );

            routes.MapRoute("Plugin.Misc.FreeSample.OrderFreeSampleSuccessful",
                "order-free-sample-form-successful",
                new
                {
                    controller = "OrderFreeSample",
                    action = "OrderFreeSampleSuccessful"
                },
                new[] { "Nop.Plugin.Misc.FreeSample.Controllers" }
            );

            routes.MapRoute("Plugin.Misc.FreeSample.HomeInstallationQuote",
                "home-installation-quote-form",
                new { controller = "HomeInstallationQuote", action = "HomeInstallationQuote" },
                new[] { "Nop.Plugin.Misc.FreeSample.Controllers" }
            );

            routes.MapRoute("Plugin.Misc.FreeSample.HomeInstallationQuoteSuccessful",
                "home-installation-quote-form-successful",
                new
                {
                    controller = "HomeInstallationQuote",
                    action = "HomeInstallationQuoteSuccessful"
                },
                new[] { "Nop.Plugin.Misc.FreeSample.Controllers" }
            );

            routes.MapRoute("Plugin.Misc.FreeSample.SupplyDeliveryQuote",
                "supply-delivery-quote-form",
                new { controller = "SupplyDeliveryQuote", action = "SupplyDeliveryQuote" },
                new[] { "Nop.Plugin.Misc.FreeSample.Controllers" }
            );

            routes.MapRoute("Plugin.Misc.FreeSample.SupplyDeliveryQuoteSuccessful",
                "supply-delivery-quote-form-successful",
                new
                {
                    controller = "SupplyDeliveryQuote",
                    action = "SupplyDeliveryQuoteSuccessful"
                },
                new[] { "Nop.Plugin.Misc.FreeSample.Controllers" }
            );
        }

        public int Priority
        {
            get { return 0; }
        }

    }
}