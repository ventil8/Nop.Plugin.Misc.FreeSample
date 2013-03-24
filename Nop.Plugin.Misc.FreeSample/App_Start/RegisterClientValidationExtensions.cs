using DataAnnotationsExtensions.ClientValidation;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Nop.Plugin.Misc.FreeSample.App_Start.RegisterClientValidationExtensions), "Start")]
 
namespace Nop.Plugin.Misc.FreeSample.App_Start {
    public static class RegisterClientValidationExtensions {
        public static void Start() {
            DataAnnotationsModelValidatorProviderExtensions.RegisterValidationExtensions();            
        }
    }
}