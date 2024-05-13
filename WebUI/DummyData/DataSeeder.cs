using System.Collections.Generic;
using tenant_sso_poc.models;
using tenant_sso_poc.repository;
using tenant_sso_poc.repository.DataSeeder;
using tenant_sso_poc.repository.Interfaces;

namespace tenant_sso_poc.WebUI.DummyData
{
    public class DataSeeder
    {
        public static IEnumerable<Tenant> CreateTenent()
        {
            ITenantRepository tenantRepository = new TenantRepository();
            TenantSeed tenantSeed = new TenantSeed(tenantRepository);
            tenantSeed.StartAsync().Wait();

            return tenantRepository.GetAllAsync().Result;
        }
        public static IEnumerable<IdentitySetting> CreateIdentitySetting()
        {
            IIdentitySettingRepository identitySettingRepository = new IdentitySettingRespository();
            IdentitySettingSeed settingSeed = new IdentitySettingSeed(identitySettingRepository);
            settingSeed.StartAsync().Wait();
            return identitySettingRepository.GetAllAsync().Result;
        }
    }
}