using Autofac;
using Autofac.Integration.Mvc;
using PerformanceEvaluating.Business.Interfaces;
using PerformanceEvaluating.Business.Repositories;
using PerformanceEvaluating.Business.Services;
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

            builder.RegisterType<DomainRequestResultRepository>().As<IDomainRequestResultRepository>()
                   .WithParameter("context", new ApplicationDbContext());
            builder.RegisterType<ChildRequestResultRepository>().As<IChildRequestResultRepository>()
                   .WithParameter("context", new ApplicationDbContext());
            builder.RegisterType<PerformanceEvaluatingService>().As<IPerformanceEvaluatingService>();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}