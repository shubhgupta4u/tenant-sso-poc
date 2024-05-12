using System.Collections.Generic;
using System.Threading.Tasks;
using tenant_sso_poc.data;
using tenant_sso_poc.models;
using tenant_sso_poc.repository.Interfaces;

namespace tenant_sso_poc.repository.DataSeeder
{
    public class IdentitySettingSeed
    {
        IIdentitySettingRepository _identitySettingRepository;
        public IdentitySettingSeed(IIdentitySettingRepository identitySettingRepository)
        {
            _identitySettingRepository = identitySettingRepository;
        }

        public async Task StartAsync()
        {
            List<IdentitySetting> settings = await GetDummyEntitiesAsync();
            Parallel.ForEach(settings, async (item) =>
            {
                await _identitySettingRepository.CreateAsync(item);
            });
        }
        public async Task<List<IdentitySetting>> GetDummyEntitiesAsync()
        {
            return await Task.Factory.StartNew<List<IdentitySetting>>(() =>
            {
                List<IdentitySetting> settings = new List<IdentitySetting>();
                settings.Add(new IdentitySetting() { Id = Constants.GOT_AUTHORITYID, AuthorityUrl= "https://dev-35555547.okta.com", ClientId = "0oah0xwjr6nroTkkv5d7", ClientSecret = "fMRcRngQXtGP5O-_Zq46bUO3EYoS0j8-hjlCjAEmeLv4CrLgO-qzDgOIMieTrFhE", SupportLocale=true });
                settings.Add(new IdentitySetting() { Id = Constants.MORDOR_AUTHORITYID, AuthorityUrl = "https://dev-35555547.okta.com", ClientId = "0oah0xsjup3bsxJUU5d7", ClientSecret = "QFhhK8fCeNNPhN2WWgqwEv1NpSFwu2t7k9lVjgLwNrDKZ4i53JELM-L0BrqupYNh", SupportLocale = true });
                return settings;
            });

        }
    }
}
