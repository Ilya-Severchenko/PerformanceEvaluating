using Autofac;
using Autofac.Integration.Mvc;
using PerformanceEvaluating.Business.Interfaces;
using PerformanceEvaluating.Business.Repositories;
using PerformanceEvaluating.Data;
using System.Web.Mvc;

namespace PerformanceEvaluating.DependencyInjection
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.RegisterType<RequestResultRepository>().As<IRequestResultRepository>()
                .WithParameter("context", new ApplicationDbContext());

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}