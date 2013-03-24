using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.FreeSample {
    public class DependencyRegistrar : IDependencyRegistrar
    {   
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            
        }

        /// <summary>
        /// Gets the order.
        /// </summary>
        public int Order
        {
            get { return 0; }
        }
    }
}