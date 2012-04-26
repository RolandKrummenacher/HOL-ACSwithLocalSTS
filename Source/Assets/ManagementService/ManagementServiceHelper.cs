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

//---------------------------------------------------------------------------------
// Copyright 2010 Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License"); 
// You may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, 
// INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR 
// CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
// MERCHANTABLITY OR NON-INFRINGEMENT. 

// See the Apache 2 License for the specific language governing 
// permissions and limitations under the License.
//---------------------------------------------------------------------------------

namespace ACS.Management
{
    using System;
    using System.Collections.Specialized;
    using System.Data.Services.Client;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using Common.ACS.Management;    

    /// <summary>
    /// This class obtains a SWT token and adds it to the HTTP authorize header 
    /// for every request to the management service.
    /// </summary>
    public class ManagementServiceHelper
    {
        static string serviceIdentityUsernameForManagement = "ManagementClient";
        static string serviceIdentityPasswordForManagement = "{yourManagementServiceKey}";

        static string serviceNamespace = "{yourServiceNamespace}";
        static string acsHostName = "accesscontrol.windows.net";

        static string cachedSwtToken;
 
        /// <summary>
        /// Creates and returns a ManagementService object. This is the only 'interface' used by other classes.
        /// </summary>
        public static ManagementService CreateManagementServiceClient()
        {
            string managementServiceHead = "v2/mgmt/service/";
            string managementServiceEndpoint  = string.Format("https://{0}.{1}/{2}", serviceNamespace, acsHostName, managementServiceHead);
            ManagementService managementService     = new ManagementService(new Uri(managementServiceEndpoint));

            managementService.SendingRequest += GetTokenWithWritePermission;

            return managementService;
        }

        /// <summary>
        /// Event handler for getting a token from ACS
        /// </summary>
        public static void GetTokenWithWritePermission(object sender, SendingRequestEventArgs args)
        {
            GetTokenWithWritePermission((HttpWebRequest)args.Request);
        }

        /// <summary>
        /// Helper function for the event handler above, adding the SWT token to the HTTP 'Authorization' header. 
        /// The SWT token is cached so that we don't need to obtain a token on every request.
        /// </summary>
        private static void GetTokenWithWritePermission(HttpWebRequest args)
        {
            if (cachedSwtToken == null)
            {
                cachedSwtToken = GetTokenFromACS();
            }

            args.Headers.Add(HttpRequestHeader.Authorization, "WRAP access_token=\"" + HttpUtility.UrlDecode(cachedSwtToken) + "\"");
        }

        /// <summary>
        /// Helper function for the event handler above, adding the SWT token to the HTTP 'Authorization' header (Via WebClient). 
        /// The SWT token is cached so that we don't need to obtain a token on every request.
        /// </summary>
        public static void AttachTokenWithWritePermissions(WebClient client)
        {
            if (cachedSwtToken == null)
            {
                cachedSwtToken = GetTokenFromACS();
            }

            client.Headers.Add(HttpRequestHeader.Authorization, "WRAP access_token=\"" + HttpUtility.UrlDecode(cachedSwtToken) + "\"");
        }

        /// <summary>
        /// Obtains a SWT token from ACSv2. 
        /// </summary>
        private static string GetTokenFromACS()
        {
            // request a token from ACS
            WebClient client = new WebClient();
            client.BaseAddress = string.Format("https://{0}.{1}", serviceNamespace, acsHostName);

            NameValueCollection values = new NameValueCollection();
            values.Add("wrap_name", serviceIdentityUsernameForManagement);
            values.Add("wrap_password", serviceIdentityPasswordForManagement);

            // The scope is 'mgmt' instead of 'mgmt/service'
            values.Add("wrap_scope", string.Format("https://{0}.{1}/v2/mgmt/service", serviceNamespace, acsHostName));

            byte[] responseBytes = client.UploadValues("WRAPv0.9/", "POST", values);

            string response = Encoding.UTF8.GetString(responseBytes);

            // Extract the SWT token and return it.
            return response
                .Split('&')
                .Single(value => value.StartsWith("wrap_access_token=", StringComparison.OrdinalIgnoreCase))
                .Split('=')[1];
        }
    }
}

