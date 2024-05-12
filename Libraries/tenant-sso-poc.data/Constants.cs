using System;

namespace tenant_sso_poc.data
{
    public class Constants
    {
        public const string CONNECTION_STRING = "Data Source=TenantSSO.db;";
        public const string CREATE_IDENTITYSETTING_QUERY = @"
                    CREATE TABLE IF NOT EXISTS
                    IdentitySettings(
                    Id TEXT NOT NULL PRIMARY KEY,
                    ClientId TEXT,
                    ClientSecret TEXT,
                    AuthorityUrl TEXT,
                    GrantType TEXT,
                    Audience TEXT,
                    SupportLocale BOOLEAN
                );";
        public const string CREATE_TENANTS_QUERY = @"
                    CREATE TABLE IF NOT EXISTS
                    Tenants(
                    Id TEXT NOT NULL PRIMARY KEY,
                    Name TEXT,
                    Alias TEXT,
                    Locale TEXT,
                    IdentityId TEXT
                );";
        public const string CREATE_USERS_QUERY = @"
                    CREATE TABLE IF NOT EXISTS
                    Users(
                    Id TEXT NOT NULL PRIMARY KEY,
                    UserName TEXT,
                    FirstName TEXT,
                    LastName TEXT,
                    Email TEXT,
                    SSN TEXT,
                    TenantId TEXT
                );";
        public const string GOT_TENANTID = "2a80c9e6-a3d9-4a7a-9ffc-457eba217e3a";
        public const string MORDOR_TENANTID = "1f62feb0-c44b-448d-846a-a58311dfabf0";
        public const string GOT_AUTHORITYID = "e39f82c0-b139-4be3-8ea3-47e93cbdc864";
        public const string MORDOR_AUTHORITYID = "15083bdb-7a96-4872-b247-e0690aef5900";
    }
}
