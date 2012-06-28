// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Services.Client;
using System.IO;
using ACS.Management;
using Common.ACS.Management;

namespace IdentityProviderSetup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateIdentityProviderWithRules();

            Console.ReadLine();
        }

        /// <summary>
        /// Add an Identity Provider
        /// </summary>
        private static Issuer CreateIdpManually(DateTime startDate, DateTime endDate, ManagementService svc0, string idpName, string idpDisplayName, string idpAddress, string idpKeyDisplayName)
        {
            var issuer = new Issuer
            {
                Name = idpName
            };

            // Check the Issuer does not exist previouly (if it exists, delete it)            
            var oldIssuer = svc0.Issuers.Where(ip => ip.Name == issuer.Name).FirstOrDefault();
            if (oldIssuer != null)
            {
                svc0.DeleteObject(oldIssuer);
                svc0.SaveChanges();
            }

            // Add Issuer
            svc0.AddToIssuers(issuer);
            svc0.SaveChanges(SaveChangesOptions.Batch);
            Console.WriteLine("Info: Issuer created: {0}", idpName);

            var idp = new IdentityProvider
            {
                DisplayName = idpDisplayName,
                LoginLinkName = idpDisplayName,
                WebSSOProtocolType = "WsFederation",
                IssuerId = issuer.Id
            };

            // Check the IP does not exist previouly (if it exists, delete it)            
            var oldIdentityProvider = svc0.IdentityProviders.Where(ip => ip.DisplayName == idp.DisplayName).FirstOrDefault();
            if (oldIdentityProvider != null)
            {
                svc0.DeleteObject(oldIdentityProvider);
                svc0.SaveChanges();
            }
            
            // Add the new IP to ACS
            svc0.AddObject("IdentityProviders", idp);

            // Console.WriteLine("Info: Identity Provider created: {0}", idp.Name);
            Console.WriteLine("Info: Identity Provider created: {0}", idp.DisplayName);

            // Identity provider public key to verify the signature
            var cert = File.ReadAllBytes(@"Resources\SelfSTS.cer");
            var key = new IdentityProviderKey
            {
                IdentityProvider = idp,
                DisplayName = idpKeyDisplayName,
                EndDate = endDate,
                StartDate = startDate,
                Type = "X509Certificate",
                Usage = "Signing",
                Value = cert
            };

            svc0.AddRelatedObject(idp, "IdentityProviderKeys", key);
            svc0.SaveChanges(SaveChangesOptions.Batch);

            Console.WriteLine("Info: Identity Provider Key added: {0}", idpKeyDisplayName);

            // WS-Federation sign-in URL
            var idpaSignIn = new IdentityProviderAddress
            {
                IdentityProviderId = idp.Id,
                EndpointType = "SignIn",
                Address = idpAddress
            };

            svc0.AddRelatedObject(idp, "IdentityProviderAddresses", idpaSignIn);
            svc0.SaveChanges(SaveChangesOptions.Batch);

            Console.WriteLine("Info: Identity Provider Address added: {0}", idpAddress);

            string labRelyingPartyName = "WebSiteAdvancedACS";

            // Relying Party related to the Identity Provider
            foreach (var existingRelyingParty in svc0.RelyingParties)
            {
                var rpid = new RelyingPartyIdentityProvider
                {
                    IdentityProviderId = idp.Id,
                    RelyingPartyId = existingRelyingParty.Id
                };
                existingRelyingParty.RelyingPartyIdentityProviders.Add(rpid);
                idp.RelyingPartyIdentityProviders.Add(rpid);
                svc0.AddToRelyingPartyIdentityProviders(rpid);
            }

            svc0.SaveChanges(SaveChangesOptions.Batch);

            Console.WriteLine("Info: Relying Party added to Identity Provider: {0}", labRelyingPartyName);

            return issuer;
        }
        
        /// <summary>
        /// Add the Rules into a Rule Group.
        /// </summary>
        private static void AddRulesToRuleGroup(string ruleGroupName, string issuerName)
        {
            ManagementService svc = ManagementServiceHelper.CreateManagementServiceClient();

            RuleGroup rg = svc.RuleGroups.AddQueryOption("$filter", "Name eq '" + ruleGroupName + "'").FirstOrDefault();

            Issuer issuer = svc.Issuers.Where(i => i.Name == issuerName).ToArray()[0];

            Rule namePassthroughRule = new Rule()
            {
                Issuer = issuer,
                IssuerId = issuer.Id,

                // InputClaimIssuerId = issuer.Id,
                InputClaimType = "http://www.theselfsts2.net/claims/nome",
                OutputClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                RuleGroup = rg,
                Description = "Passthrough \"nome\" claim from SelfSTS2 as \"name\""
            };

            svc.AddRelatedObject(rg, "Rules", namePassthroughRule);

            Rule emailPassthroughRule = new Rule()
            {
                Issuer = issuer,
                IssuerId = issuer.Id,
                InputClaimType = "http://www.theselfsts2.net/claims/postaelettronica",
                OutputClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
                RuleGroup = rg,
                Description = "Passthrough \"postaelettronica\" claim from SelfSTS2 as \"emailaddress\""
            };

            svc.AddRelatedObject(rg, "Rules", emailPassthroughRule);

            Rule goldenRule = new Rule()
            {
                Issuer = issuer,
                IssuerId = issuer.Id,
                InputClaimType = "http://www.theselfsts2.net/claims/gruppo",
                InputClaimValue = "Amministratori",
                OutputClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role",
                OutputClaimValue = "Gold",
                RuleGroup = rg,
                Description = "Map Gold Role SelfSTS2"
            };

            svc.AddRelatedObject(rg, "Rules", goldenRule);

            Rule silverRule = new Rule()
            {
                Issuer = issuer,
                IssuerId = issuer.Id,
                InputClaimType = "http://www.theselfsts2.net/claims/gruppo",
                InputClaimValue = "Utenti",
                OutputClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role",
                OutputClaimValue = "Silver",
                RuleGroup = rg,
                Description = "Map Silver Role SelfSTS2"
            };

            svc.AddRelatedObject(rg, "Rules", silverRule);

            svc.SaveChanges(SaveChangesOptions.Batch);

            Console.WriteLine();
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Info: Passthrough Rules:");
            Console.WriteLine("Info: Passthrough Name Rule created: {0}", namePassthroughRule.Description);
            Console.WriteLine("Info: Passthrough Email Rule created: {0}", emailPassthroughRule.Description);
            
            Console.WriteLine();
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Info: Roles Rules:");
            Console.WriteLine("Info: Golden Rule created: {0}", goldenRule.Description);
            Console.WriteLine("Info: Silver Rule created: {0}", silverRule.Description);
        }
        
        private static void CreateIdentityProviderWithRules()
        {
            ManagementService svc = ManagementServiceHelper.CreateManagementServiceClient();

            // Create Identity Provider
            var issuer = CreateIdpManually(DateTime.UtcNow, DateTime.UtcNow.AddYears(1), svc, "SelfSTS2", "SelfSTS2", "http://localhost:9000/STS/Issue/", "IdentityTKStsCertForSigning");

            // Add the Rules
            string ruleGroupname = "Default Rule Group for WebSiteAdvancedACS";
            AddRulesToRuleGroup(ruleGroupname, issuer.Name);

            Console.WriteLine("Done!");
        }

        internal class RuleTypes
        {
            public const string Simple = "Simple";
            public const string Passthrough = "Passthrough";
        }
    }
}
