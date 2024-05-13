using System.Collections.Generic;
using System.Web.Http.Results;
using System.Web.Mvc;
using tenant_sso_poc.models;
using tenant_sso_poc.repository;
using tenant_sso_poc.repository.Interfaces;
using tenant_sso_poc.WebUI.DummyData;

namespace tenant_sso_poc.WebUI.ApiControllers
{
    public class DataSeederController : System.Web.Http.ApiController
    {
        [HttpGet]
        [Route("Tenant")]
        public JsonResult<IEnumerable<Tenant>> GetTenant()
        {
            ITenantRepository tenantRepository = new TenantRepository();
            var result = tenantRepository.GetAllAsync().Result;
            return Json<IEnumerable<Tenant>>(result);
        }
        
        [HttpGet]
        [Route("Identity")]
        public JsonResult<IEnumerable<IdentitySetting>> GetIdentity()
        {
            IIdentitySettingRepository identitySettingRepository = new IdentitySettingRespository();
            var result = identitySettingRepository.GetAllAsync().Result;
            return Json<IEnumerable<IdentitySetting>>(result);
        }
        
        [HttpGet]
        [Route("SeedTenant")]
        public JsonResult<IEnumerable<Tenant>> SeedTenant()
        {
            var result = DataSeeder.CreateTenent();
            return Json<IEnumerable<Tenant>>(result);
        }
        
        [HttpGet]
        [Route("SeedIdentity")]
        public JsonResult<IEnumerable<IdentitySetting>> SeedIdentity()
        {
            var result = DataSeeder.CreateIdentitySetting();
            return Json<IEnumerable<IdentitySetting>>(result);
        }
    }
}
