using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;

namespace MySitterHub.Service.Rest
{
    public static class IoCConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Create the container builder
            var builder = new ContainerBuilder();
            
            // Register controllers
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // Build the container
            IContainer container = builder.Build();

            // Create the dependency resolver.
            var resolver = new AutofacWebApiDependencyResolver(container);
            
            // Configure Web API with the dependency resolver.
            config.DependencyResolver = resolver;
        }
    }
}
