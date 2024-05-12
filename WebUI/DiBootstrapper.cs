using Autofac;
using System;
using System.Threading.Tasks;
using tenant_sso_poc.repository;
using tenant_sso_poc.repository.Interfaces;

namespace tenant_sso_poc.WebUI
{
    public class DiBootstrapper
    {
        private static readonly Lazy<DiBootstrapper> lazy = new Lazy<DiBootstrapper>(() => new DiBootstrapper());

        public static DiBootstrapper Instance { get { return lazy.Value; } }

        private DiBootstrapper()
        {
        }

        public async Task<IContainer> RegisterDependenciesAsync()
        {
            var builder = await Task.Factory.StartNew<IContainer>(() => {return RegisterDependencies(); }); ;
            return builder;
        }
        private IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TenantRepository>().As<ITenantRepository>();
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            builder.RegisterType<IdentitySettingRespository>().As<IIdentitySettingRepository>();
            return builder.Build();
        }
    }
}