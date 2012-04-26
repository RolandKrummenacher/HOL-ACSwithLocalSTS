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

//-----------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.Services.Client;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using ACSConfigurationTool.ACS.Management;

namespace ACSConfigurationTool
{
    /// <summary>
    /// This class obtains a SWT token and adds it to the HTTP authorize header 
    /// for every request to the management service.
    /// </summary>
    public class ManagementServiceHelper
    {
        static string cachedSwtToken;

        string serviceNamespace;
        string acsHostUrl;
        string acsManagementServicesRelativeUrl;
        string managementServiceIdentityName;
        string managementServiceIdentityKey;

        public ManagementServiceHelper(string serviceNamespace, string acsHostUrl, string acsManagementServicesRelativeUrl, string managementServiceIdentityName, string managementServiceIdentityKey)
        {
            this.serviceNamespace = serviceNamespace;
            this.acsHostUrl = acsHostUrl;
            this.acsManagementServicesRelativeUrl = acsManagementServicesRelativeUrl;
            this.managementServiceIdentityName = managementServiceIdentityName;
            this.managementServiceIdentityKey = managementServiceIdentityKey;
        }

        /// <summary>
        /// Creates and returns a ManagementService object. This is the only 'interface' used by other classes.
        /// </summary>
        /// <returns>An instance of the ManagementService.</returns>
        public ManagementService CreateManagementServiceClient()
        {
            string managementServiceEndpoint = String.Format(CultureInfo.CurrentCulture, "https://{0}.{1}/{2}", this.serviceNamespace, this.acsHostUrl, this.acsManagementServicesRelativeUrl);
            ManagementService managementService = new ManagementService(new Uri(managementServiceEndpoint));

            managementService.SendingRequest += GetTokenWithWritePermission;
            
            return managementService;
        }

        /// <summary>
        /// Event handler for getting a token from ACS.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="args">Event arguments.</param>
        public void GetTokenWithWritePermission( object sender, SendingRequestEventArgs args )
        {
            GetTokenWithWritePermission( (HttpWebRequest)args.Request );
        }

        /// <summary>
        /// Helper function for the event handler above, adding the SWT token to the HTTP 'Authorization' header. 
        /// The SWT token is cached so that we don't need to obtain a token on every request.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        void GetTokenWithWritePermission( HttpWebRequest args )
        {
            if ( cachedSwtToken == null )
            {
                cachedSwtToken = GetTokenFromACS();
            }

            args.Headers.Add(HttpRequestHeader.Authorization, "OAuth " + cachedSwtToken);
        }

      
        /// <summary>
        /// Updates the redirect address on the service identity with the specified value.
        /// </summary>
        /// <param name="name">The name of the service identity.</param>
        /// <param name="redirectAddress">The redirect address to set.</param>
        public void UpdateServiceIdentityRedirectAddress(string name, string redirectAddress)
        {
            ManagementService svc = CreateManagementServiceClient();

            ServiceIdentity serviceIdentity = svc.ServiceIdentities.Where(si => si.Name == name).FirstOrDefault();

            if (serviceIdentity == null)
            {
                throw new ConfigurationErrorsException( "The service identity of name " + name + " could not be found." );
            }

            serviceIdentity.RedirectAddress = redirectAddress;
            svc.UpdateObject(serviceIdentity);
            svc.SaveChanges();
        }

        public Issuer AddIssuer(string identityProvider)
        {
            ManagementService svc = CreateManagementServiceClient();

            Issuer issuer = svc.Issuers.Where(i => i.Name == identityProvider).FirstOrDefault();

            if (issuer == null)
            {
                issuer = new Issuer() { Name = identityProvider };
                svc.AddToIssuers(issuer);
                svc.SaveChanges();
                return issuer;
            }

            return null;
        }

        /// <summary>
        /// Update the relying party rules to pass through any claim issued by an identity provider.
        /// </summary>
        /// <param name="relyingPartyName">The name of the relying party.</param>
        /// <param name="identityProvider">The identity provider whose issued claims must be passed through.</param>
        public  void UpdateRelyingPartyRule(string relyingPartyName, string identityProvider)
        {
            ManagementService svc = CreateManagementServiceClient();

            RelyingParty rp = svc.RelyingParties.Where(r => r.Name == relyingPartyName).FirstOrDefault();

            if (rp == null)
            {
                throw new ConfigurationErrorsException("The relying party of name " + relyingPartyName + " could not be found.");
            }

            Issuer newIssuer = AddIssuer(identityProvider);

            if (newIssuer != null)
            {
                RelyingPartyRuleGroup rpRuleGroup = svc.RelyingPartyRuleGroups.Where(m => m.RelyingPartyId == rp.Id).FirstOrDefault();

                Rule rule = new Rule() { IssuerId = newIssuer.Id, RuleGroupId = rpRuleGroup.RuleGroupId };

                svc.AddToRules(rule);

                svc.SaveChanges(SaveChangesOptions.Batch);
            }
        }

        /// <summary>
        /// Obtains a SWT token from ACSv2. 
        /// </summary>
        /// <returns>A token  from ACS.</returns>
        string GetTokenFromACS()
        {
            // request a token from ACS
            WebClient client = new WebClient();
            client.BaseAddress = string.Format(CultureInfo.CurrentCulture, "https://{0}.{1}", serviceNamespace, acsHostUrl);

            NameValueCollection values = new NameValueCollection();
            values.Add("grant_type", "password");
            values.Add("client_id", managementServiceIdentityName);
            values.Add("username", managementServiceIdentityName);
            values.Add("client_secret", managementServiceIdentityKey);
            values.Add("password", managementServiceIdentityKey);

            byte[] responseBytes = client.UploadValues("/v2/OAuth2-10/rp/AccessControlManagement", "POST", values);

            string response = Encoding.UTF8.GetString(responseBytes);

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            Dictionary<string, object> decodedDictionary = serializer.DeserializeObject(response) as Dictionary<string, object>;

            return decodedDictionary["access_token"] as string;
        }
    }
}

