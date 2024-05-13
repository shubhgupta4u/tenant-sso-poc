﻿using System.Collections.Generic;

namespace tenant_sso_poc.WebUI.Models.Oauth
{
    public class WsdlConfig
    {
        public string issuer { get; set; }
        public string authorization_endpoint { get; set; }
        public string token_endpoint { get; set; }
        public string userinfo_endpoint { get; set; }
        public string registration_endpoint { get; set; }
        public string jwks_uri { get; set; }
        public List<string> response_types_supported { get; set; }
        public List<string> response_modes_supported { get; set; }
        public List<string> grant_types_supported { get; set; }
        public List<string> subject_types_supported { get; set; }
        public List<string> id_token_signing_alg_values_supported { get; set; }
        public List<string> scopes_supported { get; set; }
        public List<string> token_endpoint_auth_methods_supported { get; set; }
        public List<string> claims_supported { get; set; }
        public List<string> code_challenge_methods_supported { get; set; }
        public string introspection_endpoint { get; set; }
        public List<string> introspection_endpoint_auth_methods_supported { get; set; }
        public string revocation_endpoint { get; set; }
        public List<string> revocation_endpoint_auth_methods_supported { get; set; }
        public string end_session_endpoint { get; set; }
        public bool request_parameter_supported { get; set; }
        public List<string> request_object_signing_alg_values_supported { get; set; }
        public string device_authorization_endpoint { get; set; }
        public List<string> dpop_signing_alg_values_supported { get; set; }
    }
}