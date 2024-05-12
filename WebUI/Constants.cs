namespace tenant_sso_poc.WebUI
{
    public class Constants
    {
        public const string DEFAULT_TENANT_ALIAS = "mordor";
        public const string TENANT_ROUTE_KEY = "tenantalias";
        internal const string DefaultAuthenticationType = "OpenIdConnect";
        //internal const string APIType = "/restapi";
        internal const string LoginProvider = "Okta";
        internal const double RefreshTokenLife = 30;
        internal const int Buffer = 3;
    }
}